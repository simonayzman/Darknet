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
		//theActor.inventory = new List<GameObject> ();
		//theActor.pickUpItem (Instantiate (my_object, new Vector2 (0, 0), Quaternion.identity) as GameObject);

	}

	// Update is called once per frame
	void Update () {
		if (theActor != null && theActor.isDead ()) {
			GameObject.FindGameObjectWithTag("World").GetComponent<GCtrller>().npcDied(gameObject);
		}
	}
}
