using UnityEngine;
using System.Collections;

public class Trans_9_10 : MonoBehaviour {

	public Transform target;  
	
	void OnTriggerEnter(Collider other){
		target.position = new Vector3(-15, 1, -2);		
	}	
}
