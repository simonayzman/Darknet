using UnityEngine;
using System.Collections;

public class Trans : MonoBehaviour {
	
	public Transform target;  
	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other){
		target.position = new Vector3(0, 0, 0);

	}	

}
