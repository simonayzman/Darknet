// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

// Christian Diaz
//I grabbed this script online, it works perfectly but I dont remember where I got it from.
//Its a basic camera script. Camera follows an object, using dampTime to cause a smooth transition. 

using UnityEngine;
using System.Collections;

public class CamFollowObj : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	//Creates a public variable of type transform. This will be what the camera will follow.
	//You can easily drag the object that is intended to be the target into the "target" section 
	//in the inspection section of this component. 
	private Transform target;

	void Start() {  
		GameObject world = GameObject.FindGameObjectWithTag ("World");
		if(world != null){
			while(true){
				GCtrller daScript = world.GetComponent<GCtrller>();
				if(daScript != null){
					GameObject player = daScript.da_player;
					target = player.transform;
					break;
				} else {
					Debug.Log("No script detected");
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (target)
		{   //Create a vector of the camera's point of view
			Vector3 point = camera.WorldToViewportPoint(target.position);
			//Position the camera away from the target along the z axis 
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10.4f)); //(new Vector3(0.5, 0.5, point.z));
			//Im not really sure
			Vector3 destination = transform.position + delta;
			//Transform this objects new position to the targets movements with the variables set above.  
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
			//Debug.Log("There is target.");
			//Debug.Log(target.position);
		}
	}
}