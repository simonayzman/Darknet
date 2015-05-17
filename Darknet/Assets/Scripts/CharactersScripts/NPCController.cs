using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NPCController : MonoBehaviour {
	
	private GameController gController;
	Player theActor;
	public GameObject my_object;
	

	// Use this for initialization
	void Start () {
		theActor = gameObject.GetComponent<Player> ();
	}

	// Update is called once per frame
	void Update () {
		/*
		if (theActor.isDead ()) {
			GameObject.FindGameObjectWithTag("World").GetComponent<GCtrller>().npcDied(gameObject);
		}
		*/
	}


	public void fightBack(){
		Debug.Log("NPC fight back.");
        GameObject world = GameObject.FindGameObjectWithTag("World");
    	if(world){
    		world.GetComponent<GCtrller>().da_player.GetComponent<Player>().dealDamage(world.GetComponent<GCtrller>().da_player, 10);
    	}
	}
}
