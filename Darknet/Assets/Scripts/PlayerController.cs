using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private GameObject player;
	private GameObject someObject;
	private Vector2 teacherPosition;
	private Vector2 playerPosition;
	private Vector2 mousePosition;
	private GameController gController;
	RaycastHit2D hit;
	float distance;
	Actor theActor;

	// Use this for initialization
	void Start () {
	    GameObject gControllerObject = GameObject.FindWithTag ("GameController");
		if (gControllerObject == null)
			Debug.Log ("No GameController");
		else {
			gController = gControllerObject.GetComponent <GameController> ();
		}

		theActor = GetComponent<Actor> ();
		theActor.inventory = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
        // seems like I have to do left click before right click WHY?
		if (Input.GetMouseButtonDown(0)){  // left click


			playerPosition = this.transform.position;
			teacherPosition = gController.teacher.transform.position;
			// this was killing me I was using Input.mousePoistion as position to cast ray from
			// this version is correct
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			hit = Physics2D.Raycast(mousePosition, -Vector2.up);

			if(hit.collider != null){

				if(hit.collider.tag == "Teacher"){ 
					distance = Vector2.Distance(playerPosition, teacherPosition);
					if( distance < .3 ){
						Debug.Log("EASY young Bro. I am your teacher.");
					}
				}
				else if(hit.collider.tag == "NPC"){
					GameObject who = hit.collider.gameObject;
					Debug.Log (who.GetComponent<Actor>().actor_name + ": You will pay SUCKA!");

			    }
				else if(hit.collider.tag == "Item"){
					GameObject gameItem = hit.collider.gameObject;
					distance = Vector2.Distance(playerPosition, mousePosition);
					if( distance < 0.35){
						Debug.Log("Ready to pick up da " + gameItem.GetComponent<Item>().description + ", Bro.");
						theActor.inventory.Add(gameItem);
						gameItem.SetActive(false);
					}
					else{
						Debug.Log("Too far, Bro.");
						//Debug.Log( "Das item est " + gameItem.GetComponent<Item>().description );
				       
					}
				}
			}
				
		}

		if (Input.GetMouseButtonDown(1)) {  // right click
			hit = Physics2D.Raycast(mousePosition, -Vector2.up);
			
			if(hit.collider != null){
				if(hit.collider.tag == "Teacher"){ 
					distance = Vector2.Distance(playerPosition, teacherPosition);
					if( distance < 0.5 ){
						Debug.Log ("Take this wooden sword young Bro and defeat your enemy.");
						gController.nextTeacherState();
					}
				}
				else if(hit.collider.tag == "NPC"){
					GameObject who = hit.collider.gameObject;
					Debug.Log (who.GetComponent<Actor>().actor_name + ": What do you have to say before you DIE?");
				}
			}
			distance = Vector2.Distance (playerPosition, teacherPosition);
			/*
			if (distance > 0.5)
				Debug.Log ("Too far for communication");
			else
				Debug.Log ("We can talk");
		}
		*/
		}

		if (Input.GetKeyDown ("f")) {
			theActor.updateXP(50);
		}

		if (Input.GetKeyDown ("h")) {
			theActor.updateHealth (-25);
		}

		if (Input.GetKeyDown ("i")) {
			Debug.Log ("There are " + theActor.inventory.Count.ToString ());
			if( theActor.inventory.Count > 0 ){
				Item props = theActor.inventory[0].GetComponent<Item>();
				if( props != null){
				//foreach( string item in props ){
					Debug.Log("Property : " + props.description + " has value: " + props.item_properties["attack"].ToString() );
				//}
				}
			}
		}

		if (Input.GetKeyDown ("v")) {
			if( theActor.inventory.Count > 0 ){
				GameObject gameItem = theActor.inventory[0];
				theActor.inventory.RemoveAt(0);
				Vector2 position = gController.playerPosition();
				gameItem.transform.position = position;
				gameItem.SetActive(true);
			} else{
				Debug.Log("You don't have nothin' in hands Bro.");
			}
		}
	}
}
