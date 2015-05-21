// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

//Christian Diaz
//This script is used for the panels that allow players to move between areas.

using UnityEngine;
using System.Collections;

public class Trans_11_12 : MonoBehaviour {

  //if the player touches the collider of the component that this script is attached to, this code will run.
  //First the World will be checked, and the the player will be found. After the player is located and 
  //correctly identified, the player will be transformed from their current location to a different location,
  //one that corresponds to a different area.
	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Teleport trigger.");
		GameObject world = GameObject.FindGameObjectWithTag("World");
        if(world){
        	//Debug.Log("Found world.");
        	GameObject player = world.GetComponent<GCtrller>().da_player;
        	if(player){
        		//Debug.Log("Found player.");
        		player.GetComponent<Player>().transform.position = new Vector3(-58.19828f, 11.09542f, -2.54f);
       		}
       	}
	}
}
