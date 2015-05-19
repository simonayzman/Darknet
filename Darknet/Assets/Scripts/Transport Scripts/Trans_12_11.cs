using UnityEngine;
using System.Collections;

public class Trans_12_11 : MonoBehaviour {


	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Teleport trigger.");
		GameObject world = GameObject.FindGameObjectWithTag("World");
        if(world){
        	Debug.Log("Found world.");
        	GameObject player = world.GetComponent<GCtrller>().da_player;
        	if(player){
        		Debug.Log("Found player.");
        		player.GetComponent<Player>().transform.position = new Vector3(-54.87984f, -4.1862f, -2.54f);
       		}
       	}
	}
	}

