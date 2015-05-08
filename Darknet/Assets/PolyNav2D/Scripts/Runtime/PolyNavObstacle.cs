using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
[AddComponentMenu("Navigation/PolyNavObstacle")]
///Place on a game object to act as an obstacle
public class PolyNavObstacle : MonoBehaviour {

	public enum ShapeType
	{
		Polygon,
		Box
	}

	///Inverts the polygon (done automatically if collider already exists due to a sprite)
	public bool invertPolygon = false;
	public ShapeType shapeType = ShapeType.Polygon;
	public float extraOffset;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Vector3 lastScale;
	private Transform _transform;
	private Collider2D _collider;

	new private Collider2D collider{
		get
		{
			if (_collider == null)
				_collider = GetComponent<Collider2D>();
			return _collider;
		}
	}

	///The polygon points of the obstacle
	public Vector2[] points{
		get
		{
			if (collider is BoxCollider2D){
				var box = (BoxCollider2D)collider;
				var tl = box.center + new Vector2(-box.size.x, box.size.y)/2;
				var tr = box.center + new Vector2(box.size.x, box.size.y)/2;
				var br = box.center + new Vector2(box.size.x, -box.size.y)/2;
				var bl = box.center + new Vector2(-box.size.x, -box.size.y)/2;
				return new Vector2[]{tl, tr, br, bl};
			}

			if (collider is PolygonCollider2D){
				Vector2[] tempPoints = (collider as PolygonCollider2D).points;				
				if (invertPolygon)
					System.Array.Reverse(tempPoints);
				return tempPoints;			
			}

			return null;
		}
	}

	private PolyNav2D polyNav{
		get {return PolyNav2D.current;}
	}

	void Reset(){
		
		if (collider == null)
			gameObject.AddComponent<PolygonCollider2D>();
		if (collider is PolygonCollider2D)
			shapeType = ShapeType.Polygon;
		if (collider is BoxCollider2D)
			shapeType = ShapeType.Box;

		collider.isTrigger = true;
		if (GetComponent<SpriteRenderer>() != null)
			invertPolygon = true;
	}

	void OnEnable(){

		if (polyNav)
			polyNav.AddObstacle(this);
		
		lastPos = transform.position;
		lastRot = transform.rotation;
		lastScale = transform.localScale;
		_transform = transform;
	}

	void OnDisable(){

		if (polyNav)
			polyNav.RemoveObstacle(this);
	}

	void Update(){
		
		if (!Application.isPlaying || !polyNav || !polyNav.generateOnUpdate){
			if (shapeType == ShapeType.Polygon && !(collider is PolygonCollider2D) ){
				DestroyImmediate(collider);
				gameObject.AddComponent<PolygonCollider2D>();
			}

			if (shapeType == ShapeType.Box && !(collider is BoxCollider2D) ){
				DestroyImmediate(collider);
				gameObject.AddComponent<BoxCollider2D>();
			}
			return;
		}

		if (_transform.position != lastPos || _transform.rotation != lastRot || _transform.localScale != lastScale)
			polyNav.regenerateFlag = true;

		lastPos = _transform.position;
		lastRot = _transform.rotation;
		lastScale = _transform.localScale;
	}
}
