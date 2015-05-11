using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;                        // need it for access to String

public class GameController : MonoBehaviour {

	private GameObject player;
	public GameObject teacher;
	public Text Avatar;
	public Text Experience;
	public Text Health;
	private int player_level;
	UnityEngine.UI.Image[] inventory_images;
	public GameObject[] game_items;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		if (player == null) {
			Debug.Log ("no player");
			return;
		}
		if (Avatar == null) {
			Debug.Log ("no avatar");
			return;
		}
		teacher = GameObject.FindWithTag ("Teacher");
		player_level = player.GetComponent<Actor> ().level;
		Avatar.text = "Name: " + player.GetComponent<Actor>().actor_name;
		Experience.text = "XP: ";
		Health.text = "HP: ";


		/*
		// gather all objects and make them invisible
		foreach ( GameObject item in items ) {
			item.SetActive(false);
			gameItems.Add( item );
		}
        */
		//get all image elements
		inventory_images = FindObjectsOfType(typeof(Image)) as UnityEngine.UI.Image[];

		int item_index;
		float x_pos;
		for (int i = 0; i < 4; i++) {
			item_index = UnityEngine.Random.Range (0, 12);
			x_pos = -1.0f + 0.5f * i;
			Instantiate (game_items [item_index], new Vector2 (x_pos, 1), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null || Experience == null)
			return;
		player_level = player.GetComponent<Actor> ().level;
		Experience.text = "XP: " + player.GetComponent<Actor> ().exp.ToString () + "/" + 
			player.GetComponent<Actor> ().xp_level [player_level].ToString ();
		Health.text = "HP: " + player.GetComponent<Actor> ().hp.ToString ();

	}

	public Vector2 playerPosition(){
		return player.transform.position;
	}

	public void setInventoryImage( Sprite item_sprite, int inventory_position ){
		// right now the way Unity searches for Images on screen they are returned in the
		// reversed order !
		if (inventory_position >= 0 && inventory_position < 12) {
			inventory_images [inventory_position].sprite = item_sprite;
			// inventory_images [inventory_position].color = Color.red; this works, changes color, but I need something better
		} else {
			Debug.Log ("Array index out of bounds.");
		}
	}

}
