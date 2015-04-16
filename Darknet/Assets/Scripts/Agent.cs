using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

private NavMeshAgent agent;

// Use this for initialization
void Start () {
  agent = GetComponent<NavMeshAgent>();
 }
	
// Update is called once per frame
void Update(){
  if (Input.GetMouseButtonDown (0)){
	  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	  RaycastHit hit;

   if(Physics.Raycast(ray,out hit)){
	  if(hit.collider.tag == "Ground"){
		 agent.SetDestination(hit.point);
				
	}
   }
  }
 }
}
