using UnityEngine;
using System.Collections;

public class Trans_11_12 : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Teleport trigger.");
		GameObject world = GameObject.FindGameObjectWithTag("World");
        if(world){
        	Debug.Log("Found world.");
        	GameObject player = world.GetComponent<GCtrller>().da_player;
        	if(player){
        		Debug.Log("Found player.");
        		player.GetComponent<Player>().transform.position = new Vector3(-58.19828f, 11.09542f, -2.54f);
       		}
       	}
	}
}
