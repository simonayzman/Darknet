using UnityEngine;
using System.Collections;

public class Trans_9_10 : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Teleport trigger.");
		GameObject world = GameObject.FindGameObjectWithTag("World");
        if(world){
        	//Debug.Log("Found world.");
        	GameObject player = world.GetComponent<GCtrller>().da_player;
        	if(player){
        		//Debug.Log("Found player.");
        		player.GetComponent<Player>().transform.position = new Vector3(-77.19734f, -7.131994f, -2.54f);
       		}
       	}
	}	
}
