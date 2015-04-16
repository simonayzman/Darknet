using UnityEngine;
using System.Collections;
//I grabbed this script online, it works perfectly but I dont remember where I got it from.
//Its a basic camera script. Camera follows an object, using dampTime to cause a smooth transition. 


public class CamFollowObj : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	//Creates a public variable of type transform. This will be what the camera will follow.
	//You can easily drag the object that is intended to be the target into the "target" section 
	//in the inspection section of this component. 
	public Transform target;

	void Start() {
		target = GameObject.FindWithTag("Player").transform;
	}

	// Update is called once per frame
	void Update ()
	{
		if (target)
		{   //Create a vector of the camera's point of view
			Vector3 point = camera.WorldToViewportPoint(target.position);
			//Position the camera away from the target along the z axis 
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			//Im not really sure
			Vector3 destination = transform.position + delta;
			//Transform this objects new position to the targets movements with the variables set above.  
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}