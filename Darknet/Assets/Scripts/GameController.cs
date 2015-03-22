using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private GameObject player;
	public GameObject teacher;
	private List<GameObject> gameItems;
	public Text Avatar;
	public Text Experience;
	public Text Health;
	private int player_level;
	private int teacher1_state;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		teacher = GameObject.FindWithTag ("Teacher");
		player_level = player.GetComponent<Actor> ().level;
		Avatar.text = "Name: " + player.GetComponent<Actor>().actor_name;
		Experience.text = "XP: ";
		Health.text = "HP: ";
		gameItems = new List<GameObject> ();
		GameObject[] items = GameObject.FindGameObjectsWithTag ("Item");

		teacher1_state = 0;
		// gather all objects and make them invisible
		foreach ( GameObject item in items ) {
			item.SetActive(false);
			gameItems.Add( item );
		}
	}
	
	// Update is called once per frame
	void Update () {
		player_level = player.GetComponent<Actor> ().level;
		Experience.text = "XP: " + player.GetComponent<Actor> ().exp.ToString () + "/" + 
			player.GetComponent<Actor> ().xp_level [player_level].ToString ();
		Health.text = "HP: " + player.GetComponent<Actor> ().hp.ToString ();

	}

	public Vector2 playerPosition(){
		return player.transform.position;
	}

	public void nextTeacherState(){
		teacher1_state += 1;
		foreach ( GameObject item in gameItems ) {
			if( item.GetComponent<Item>().description == "wooden sword" ){
				item.SetActive(true);
			}
		}
	}

	public int getTeacherState() {
		return teacher1_state;
	}
}
