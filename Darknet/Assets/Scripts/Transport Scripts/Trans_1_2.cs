using UnityEngine;
using System.Collections;

public class Trans_1_2 : MonoBehaviour {

	public GameObject da_player;

	void OnTriggerEnter(Collider other){
		da_player.transform.position = new Vector3(51.55f, 6.22f, 0f);		
	}	

}
