using UnityEngine;
using System.Collections;

public class FollowCube : MonoBehaviour {

	public Transform target;
	public float zOffset;
	
	private Animator animator;
	public NavMeshAgent myAgent;

	// Use this for initialization
	void Start () {
	
		animator = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	if (myAgent.velocity.x == 0){
		animator.SetInteger("Direction", 0);
	}
	
	else if (myAgent.velocity.x < -.5f && myAgent.velocity.z > .5f){
		animator.SetInteger("Direction", 7);
	}
	else if (myAgent.velocity.x > .5f && myAgent.velocity.z > .5f){
		animator.SetInteger("Direction", 9);
	}
	else if (myAgent.velocity.x < -.5f && myAgent.velocity.z < -.5f){
		animator.SetInteger("Direction", 1);
	}
	else if (myAgent.velocity.x > .5f && myAgent.velocity.z < -.5f){
		animator.SetInteger("Direction", 3);	
	
	}	
	
	else if (myAgent.velocity.z > .5f){
		animator.SetInteger("Direction", 8);
	}
	else if (myAgent.velocity.z < -.5f){
		animator.SetInteger("Direction", 5);
	}
	else if (myAgent.velocity.x > .5f){
		animator.SetInteger("Direction", 6);
	}
	else if (myAgent.velocity.x < -.5f){
		animator.SetInteger("Direction", 4);
	}

	


}

	void LateUpdate(){
		transform.localPosition = new Vector3 (target.localPosition.x, transform.localPosition.y, target.localPosition.z+ zOffset);
	}
}
