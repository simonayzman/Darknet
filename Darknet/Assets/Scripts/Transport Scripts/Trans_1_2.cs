using UnityEngine;
using System.Collections;

public class Trans_1_2 : MonoBehaviour {

	public GameObject da_player;

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Teleport trigger.");
		GameObject world = GameObject.FindGameObjectWithTag("World");
        if(world){
        	Debug.Log("Found world.");
        	GameObject player = world.GetComponent<GCtrller>().da_player;
        	if(player){
        		Debug.Log("Found player.");
        		player.GetComponent<Player>().transform.position = new Vector3(30.55f, 6.22f, 0f);
       		}
       	}
	}	

}
