using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

//[RequireComponent(typeof(PolygonCollider2D))]
[AddComponentMenu("Navigation/Poly|Nav2D")]
///Singleton. Main class for map generation and navigation
public class PolyNav2D : MonoBehaviour {

	///If true map will recalculate whenever an obstacle changes position, rotation or scale.
	public bool generateOnUpdate = true;
	///The list of obstacles for the navigation
	public List<PolyNavObstacle> navObstacles = new List<PolyNavObstacle>();
	///The radious from the edges to offset the agents.
	public float inflateRadius = 0.1f;

	///A Flag to tell PolyNav to regenerate the map
	public bool regenerateFlag;


	private PolyMap map;
	private List<PathNode> nodes = new List<PathNode>();
	private PathNode[] tempNodes;

	private Queue<PathRequest> pathRequests = new Queue<PathRequest>();
	private PathRequest currentRequest;
	private bool isProcessingPath;
	private PathNode startNode;
	private PathNode endNode;


	private Collider2D _masterCollider;
	private Collider2D masterCollider{
		get
		{
			if (_masterCollider == null)
				_masterCollider = GetComponent<Collider2D>();
			return _masterCollider;
		}
	}


	///The current instance of PolyNav2D
	private static PolyNav2D _current;
	public static PolyNav2D current{
		get
		{
			if (_current == null || !Application.isPlaying)
				_current = FindObjectOfType(typeof(PolyNav2D)) as PolyNav2D;
			
			return _current;
		}
	}

	///The total nodes count of the map
	public int nodesCount{
		get {return nodes.Count;}
	}

	///is the pathfinder currently processing a pathfinding request?
	public bool pendingRequest{
		get {return isProcessingPath;}
	}


	//some initializing
	void Reset(){
		gameObject.name = "@PolyNav2D";
		gameObject.AddComponent<PolygonCollider2D>();
	}

	//some initializing
	void Awake(){
		_current = this;
		masterCollider.enabled = false;
		GenerateMap(true);
	}

	///Adds a PolyNavObstacle to the map.
	public void AddObstacle( PolyNavObstacle navObstacle ){
		if (!navObstacles.Contains(navObstacle)){
			navObstacles.Add(navObstacle);
			regenerateFlag = true;
		}
	}

	///Removes a PolyNavObstacle to the map.
	public void RemoveObstacle ( PolyNavObstacle navObstacle ){
		navObstacles.Remove(navObstacle);
		regenerateFlag = true;
	}

	void LateUpdate(){
		if (regenerateFlag == true){
			regenerateFlag = false;
			GenerateMap(false);
		}
	}


	///Find a path 'from' and 'to', providing a callback for when path is ready containing the path.
	public void FindPath(Vector2 start, Vector2 end, System.Action<Vector2[], bool> callback){

		if (CheckLOS(start, end)){
			callback( new Vector2[]{start, end}, true );
			return;
		}

		pathRequests.Enqueue( new PathRequest(start, end, callback) );
		TryNextFindPath();
	}

	//Pathfind next request
	void TryNextFindPath(){

		if (isProcessingPath || pathRequests.Count <= 0)
			return;

		currentRequest = pathRequests.Dequeue();
		isProcessingPath = true;

		if (!PointIsValid(currentRequest.start))
			currentRequest.start = GetCloserEdgePoint(currentRequest.start);

		//create start & end as temp nodes
		startNode = new PathNode(currentRequest.start);
		endNode = new PathNode(currentRequest.end);

		nodes.Add(startNode);
		LinkStart(startNode, nodes);

		nodes.Add(endNode);
		LinkEnd(endNode, nodes);

		StartCoroutine( AStar.CalculatePath(startNode, endNode, nodes.ToArray(), RequestDone) );
	}


	//Pathfind request finished (path found or not)
	void RequestDone(Vector2[] path, bool success){

		try 
		{
			//Remove temp start and end nodes
			for (int i = 0; i < endNode.links.Count; i++){
				nodes[ endNode.links[i] ].links.Remove(nodes.IndexOf(endNode));
			}
			nodes.Remove(endNode);
			nodes.Remove(startNode);
			//			
		}
		catch{/*TODO: Find out why */}


		isProcessingPath = false;
		currentRequest.callback(path, success);
		TryNextFindPath();
	}







	///Generate the map
	public void GenerateMap(bool generateMaster){
		CreatePolyMap(generateMaster);
		CreateNodes();
		LinkNodes(nodes);
	}

	//helper function
	Vector2[] TransformPoints ( Vector2[] points, Transform t ){
		for (int i = 0; i < points.Length; i++)
			points[i] = t.TransformPoint(points[i]);
		return points;
	}

	//takes all colliders points and convert them to usable stuff
	void CreatePolyMap(bool generateMaster){

		var masterPolys = new List<Polygon>();
		var obstaclePolys = new List<Polygon>();

		//create a polygon object for each obstacle
		for (int i = 0; i < navObstacles.Count; i++){
			var obstacle = navObstacles[i];
			var transformedPoints = TransformPoints(obstacle.points, obstacle.transform);
			var inflatedPoints = InflatePolygon(transformedPoints, Mathf.Max(0.01f, inflateRadius + obstacle.extraOffset) );
			obstaclePolys.Add(new Polygon(inflatedPoints));
		}

		if (generateMaster){

			if (masterCollider is PolygonCollider2D){

				var polyCollider = (PolygonCollider2D)masterCollider;
				//invert the main polygon points so that we save checking for inward/outward later (for Inflate)
				var reversed = new List<Vector2>();
				
				for (int i = 0; i < polyCollider.pathCount; ++i){

					for (int p = 0; p < polyCollider.GetPath(i).Length; ++p)
						reversed.Add( polyCollider.GetPath(i)[p] );
					
					reversed.Reverse();

					var transformed = TransformPoints(reversed.ToArray(), polyCollider.transform);
					var inflated = InflatePolygon(transformed, Mathf.Max(0.01f, inflateRadius) );
				
					masterPolys.Add(new Polygon(inflated));
					reversed.Clear();
				}

			} else if (masterCollider is BoxCollider2D){
				var box = (BoxCollider2D)masterCollider;
				var tl = box.center + new Vector2(-box.size.x, box.size.y)/2;
				var tr = box.center + new Vector2(box.size.x, box.size.y)/2;
				var br = box.center + new Vector2(box.size.x, -box.size.y)/2;
				var bl = box.center + new Vector2(-box.size.x, -box.size.y)/2;
				var transformed = TransformPoints(new Vector2[]{tl, bl, br, tr}, masterCollider.transform);
				var inflated = InflatePolygon(transformed, Mathf.Max(0.01f, inflateRadius));
				masterPolys.Add(new Polygon(inflated) );
			}
		
		} else {

			masterPolys = map.masterPolygons.ToList();
		}

		//create the main polygon map (based on inverted) also containing the obstacle polygons
		map = new PolyMap(masterPolys.ToArray(), obstaclePolys.ToArray());

		//
		//The colliders are never used again after this point. They are simply a drawing method.
		//
	}

	//Create Nodes at convex points (since master poly is inverted, it will be concave for it) if they are valid
	void CreateNodes (){

		nodes.Clear();

		for (int p = 0; p < map.allPolygons.Length; p++){
			var poly = map.allPolygons[p];
			//Inflate even more for nodes, by a marginal value to allow CheckLOS between them
			Vector2[] inflatedPoints = InflatePolygon(poly.points, 0.05f);
			for (int i = 0; i < inflatedPoints.Length; i++){

				//if point is concave dont create a node
				if (PointIsConcave(inflatedPoints, i))
					continue;

				//if point is not in valid area dont create a node
				if (!PointIsValid(inflatedPoints[i]))
					continue;

				nodes.Add(new PathNode(inflatedPoints[i]));
			}
		}
	}

	//link the nodes provided
	void LinkNodes(List<PathNode> nodeList){

		for (int a = 0; a < nodeList.Count; a++){

			nodeList[a].links.Clear();

			for (int b = 0; b < nodeList.Count; b++){
				
				if (nodeList[a] == nodeList[b])
					continue;

				if (b > a)
					continue;

				if (CheckLOS(nodeList[a].pos, nodeList[b].pos)){
					nodeList[a].links.Add(b);
					nodeList[b].links.Add(a);
				}
			}
		}
	}
	
	//Link the start node in
	void LinkStart(PathNode start, List<PathNode> toNodes){
		for (int i = 0; i < toNodes.Count; i++){
			if (CheckLOS(start.pos, toNodes[i].pos)){
				start.links.Add(i);
			}			
		}
	}

	//Link the end node in
	void LinkEnd(PathNode end, List<PathNode> toNodes){
		for (int i = 0; i < toNodes.Count; i++){
			if (CheckLOS(end.pos, toNodes[i].pos)){
				end.links.Add(i);
				toNodes[i].links.Add(toNodes.IndexOf(end));
			}			
		}
	}


	///Determine if 2 points see each other.
	public bool CheckLOS (Vector2 posA, Vector2 posB){

		if ( (posA - posB).sqrMagnitude < Mathf.Epsilon )
			return true;
		
		Polygon poly = null;
		for (int i = 0; i < map.allPolygons.Length; i++){
			poly = map.allPolygons[i];
			for (int j = 0; j < poly.points.Length; j++){
				if (SegmentsCross(posA, posB, poly.points[j], poly.points[(j + 1) % poly.points.Length]))
					return false;
			}
		}
		return true;
	}

	///determine if a point is within a valid (walkable) area.
	public bool PointIsValid (Vector2 point){

		for (int i = 0; i < map.allPolygons.Length; i++){
			if (i == 0? !PointInsidePolygon(map.allPolygons[i].points, point) : PointInsidePolygon(map.allPolygons[i].points, point))
				return false;
		}
		return true;
	}

	///Kind of scales a polygon based on it's vertices average normal.
	public static Vector2[] InflatePolygon(Vector2[] points, float dist){

		var inflatedPoints = new Vector2[points.Length];

		for (int i = 0; i < points.Length; i++){

			Vector2 ab = (points[(i + 1) % points.Length] - points[i]).normalized;
			Vector2 ac = (points[i == 0? points.Length - 1 : i - 1] - points[i]).normalized;
			Vector2 mid = (ab + ac).normalized;
			
			mid *= (!PointIsConcave(points, i)? -dist : dist);
			inflatedPoints[i] = (points[i] + mid);
		}

		return inflatedPoints;
	}

	///Check if or not a point is concave to the polygon points provided
	public static bool PointIsConcave(Vector2[] points, int point){

		Vector2 current = points[point];
		Vector2 next = points[(point + 1) % points.Length];
		Vector2 previous =  points[point == 0? points.Length - 1 : point - 1];

		Vector2 left = new Vector2(current.x - previous.x, current.y - previous.y);
		Vector2 right = new Vector2(next.x - current.x, next.y - current.y);

		float cross = (left.x * right.y) - (left.y * right.x);

		return cross > 0;
	}

	///Check intersection of two segments, each defined by two vectors.
	public static bool SegmentsCross (Vector2 a, Vector2 b, Vector2 c, Vector2 d){

		float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));

		if (denominator == 0)
			return false;

	    float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
	    float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

	    if (numerator1 == 0 || numerator2 == 0)
	    	return false;

	    float r = numerator1 / denominator;
	    float s = numerator2 / denominator;

	    return (r > 0 && r < 1) && (s > 0 && s < 1);
	}

	///Is a point inside a polygon?
	public static bool PointInsidePolygon(Vector2[] polyPoints, Vector2 point){

		float xMin = 0;
		for (int i = 0; i < polyPoints.Length; i++)
			xMin = Mathf.Min(xMin, polyPoints[i].x);

		Vector2 origin = new Vector2(xMin - 0.1f, point.y);
		int intersections = 0;

		for (int i = 0; i < polyPoints.Length; i++){

			Vector2 pA = polyPoints[i];
			Vector2 pB = polyPoints[(i + 1) % polyPoints.Length];

			if (SegmentsCross(origin, point, pA, pB))
				intersections ++;
		}

		return (intersections & 1) == 1;
	}

	///Finds the closer edge point to the navigation valid area
	public Vector2 GetCloserEdgePoint ( Vector2 point ){

		var possiblePoints= new List<Vector2>();
		var closerVertex = Vector2.zero;
		var closerVertexDist = Mathf.Infinity;

		Polygon poly = null;
		Vector2[] inflatedPoints = null;
		for (int p = 0; p < map.allPolygons.Length; p++){

			poly = map.allPolygons[p];
			inflatedPoints = InflatePolygon(poly.points, 0.01f);

			for (int i = 0; i < inflatedPoints.Length; i++){

				Vector2 a = inflatedPoints[i];
				Vector2 b = inflatedPoints[(i + 1) % inflatedPoints.Length];

				Vector2 originalA = poly.points[i];
				Vector2 originalB = poly.points[(i + 1) % poly.points.Length];
				
				Vector2 proj = (Vector2)Vector3.Project( (point - a), (b - a) ) + a;

				if (SegmentsCross(point, proj, originalA, originalB) && PointIsValid(proj))
					possiblePoints.Add(proj);

				float dist = (point - inflatedPoints[i]).sqrMagnitude;
				if ( dist < closerVertexDist && PointIsValid(inflatedPoints[i])){
					closerVertexDist = dist;
					closerVertex = inflatedPoints[i];
				}
			}
		}

		possiblePoints.Add(closerVertex);
		//possiblePoints = possiblePoints.OrderBy(vector => (point - vector).sqrMagnitude).ToArray(); //Not supported in iOS?
		//return possiblePoints[0];

		var closerDist = Mathf.Infinity;
		var index = 0;
		for (int i = 0; i < possiblePoints.Count; i++){
			var dist = (point - possiblePoints[i]).sqrMagnitude;
			if (dist < closerDist){
				closerDist = dist;
				index = i;
			}
		}
		Debug.DrawLine(point, possiblePoints[index]);
		return possiblePoints[index];
	}



////////////////////////////////////////
///////////GUI AND EDITOR STUFF/////////
////////////////////////////////////////
#if UNITY_EDITOR
	
	void OnDrawGizmos (){

		if (!Application.isPlaying)
			CreatePolyMap(true);

		//the original drawn polygons
		if (masterCollider is PolygonCollider2D){
			var polyCollider = (PolygonCollider2D)masterCollider;
			for ( int i = 0; i < polyCollider.pathCount; ++i ) {
	            for ( int p = 0; p < polyCollider.GetPath(i).Length; ++p )
	                DebugDrawPolygon( TransformPoints( polyCollider.GetPath(i), polyCollider.transform ), Color.green );
	        }
        
        } else if (masterCollider is BoxCollider2D){
        	var box = masterCollider as BoxCollider2D;
			var tl = box.center + new Vector2(-box.size.x, box.size.y)/2;
			var tr = box.center + new Vector2(box.size.x, box.size.y)/2;
			var br = box.center + new Vector2(box.size.x, -box.size.y)/2;
			var bl = box.center + new Vector2(-box.size.x, -box.size.y)/2;
        	DebugDrawPolygon(TransformPoints(new Vector2[]{tl, tr, br, bl}, masterCollider.transform), Color.green);
        }

		foreach(PolyNavObstacle o in navObstacles)
			DebugDrawPolygon(TransformPoints(o.points, o.transform), new Color(1, 0.7f, 0.7f));
        //


		//the inflated actualy used polygons
        foreach (Polygon pathPoly in map.masterPolygons)
        	DebugDrawPolygon(pathPoly.points, new Color(1f,1f,1f,0.2f));

		foreach(Polygon poly in map.obstaclePolygons)
			DebugDrawPolygon(poly.points, new Color(1, 0.7f, 0.7f, 0.1f));
		//
	}

	//helper debug function
	void DebugDrawPolygon(Vector2[] points, Color color){
		for (int i = 0; i < points.Length; i++)
			Debug.DrawLine(points[i], points[(i + 1) % points.Length], color);
	}

	[UnityEditor.MenuItem("GameObject/Create Other/PolyNav2D")]
	public static void CreatePolyNav2D (){
		if (FindObjectOfType(typeof(PolyNav2D)) == null){
			var newNav = new GameObject("@PolyNav2D").AddComponent<PolyNav2D>();
			UnityEditor.Selection.activeObject = newNav;
		}
	}

#endif


	//defines a polygon
	class Polygon{
		public Vector2[] points;
		public Polygon(Vector2[] points){
			this.points = points;
		}
	}

	//defines the main navigation polygon and its sub obstacle polygons
	class PolyMap{

		public Polygon[] masterPolygons;
		public Polygon[] obstaclePolygons;
		public Polygon[] allPolygons;


		public PolyMap(Polygon[] masterPolys, Polygon[] obstaclePolys){
			masterPolygons = masterPolys;
			obstaclePolygons = obstaclePolys;
			var temp = new List<Polygon>();
			temp.AddRange(masterPolys);
			temp.AddRange(obstaclePolys);
			allPolygons = temp.ToArray();
		}
	}

	struct PathRequest{
		public Vector2 start;
		public Vector2 end;
		public Action<Vector2[], bool> callback;

		public PathRequest(Vector2 start, Vector2 end, Action<Vector2[], bool> callback){
			this.start = start;
			this.end = end;
			this.callback = callback;
		}
	}

	//defines a node for A*
	public class PathNode : IHeapItem<PathNode>{

		public Vector2 pos;
		public List<int> links= new List<int>();
		public float gCost = 1;
		public float hCost;
		public PathNode parent = null;
		private int _heapIndex;

		public PathNode ( Vector2 pos  ){
			this.pos = pos;
		}

		public float fCost{
			get { return gCost + hCost; }
		}

		public int heapIndex{
			get {return _heapIndex;}
			set {_heapIndex = value;}
		}

		public int CompareTo ( PathNode other ){
			int compare = fCost.CompareTo(other.fCost);
			if (compare == 0)
				compare = hCost.CompareTo(other.hCost);
			return -compare;
		}
	}
}