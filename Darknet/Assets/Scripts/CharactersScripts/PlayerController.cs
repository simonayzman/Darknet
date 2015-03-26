using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private const int INVENTORY_CAPACITY = 6;
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
		theActor.setCapacity (INVENTORY_CAPACITY);
	}
	
	// Update is called once per frame
	void Update () {
        // seems like I have to do left click before right click WHY?
		if (Input.GetMouseButtonDown(0)){  // left click


			playerPosition = this.transform.position;
			teacherPosition = GameObject.FindWithTag("Teacher").transform.position;
			// this was killing me I was using Input.mousePoistion as position to cast ray from
			// this version is correct
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			hit = Physics2D.Raycast(mousePosition, -Vector2.up);

			if(hit.collider != null){

				if(hit.collider.tag == "Teacher"){ 
					distance = Vector2.Distance(playerPosition, teacherPosition);
					if( distance < .5 ){
						Debug.Log("EASY young Bro. I am your teacher.");
					}
				}
				else if(hit.collider.tag == "NPC"){
					GameObject who = hit.collider.gameObject;
					distance = Vector2.Distance(playerPosition, who.transform.position);
					if( distance < .5 ){
					    //Debug.Log (who.GetComponent<Actor>().actor_name + ": You will pay SUCKA!");
						// this will finish the battle
						theActor.Fight(who);
					}

			    }
				else if(hit.collider.tag == "Item"){
					GameObject gameItem = hit.collider.gameObject;
					distance = Vector2.Distance(playerPosition, mousePosition);
					if( distance < 0.45){
						Debug.Log("Ready to pick up da " + gameItem.GetComponent<Item>().getDescription() + ", Bro.");
						theActor.pickUpItem( gameItem, gController );
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
					if( distance < 1.0 ){
						hit.collider.GetComponent<StateMachineTeacher>().StateChanged();
						hit.collider.GetComponent<StateMachineTeacher>().message();
					}
				}
				else if(hit.collider.tag == "NPC"){
					GameObject who = hit.collider.gameObject;
					Debug.Log (who.GetComponent<Actor>().actor_name + ": What do you have to say before you DIE?");
					who.GetComponent<Actor>().getTopItem().GetComponent<Item>().getDescription();
				}
			}
		}

		if (Input.GetKeyDown ("f")) {
			theActor.updateXP(50);
		}

		if (Input.GetKeyDown ("h")) {
			theActor.updateHealth (-25);
		}

		if (Input.GetKeyDown ("i")) {
			Debug.Log ("You have " + theActor.inventory.Count.ToString () + " items in your inventory ");
			if( theActor.inventory.Count > 0 ){
				Item props = theActor.inventory[0].GetComponent<Item>();
				if( props != null){
				//foreach( string item in props ){
					Debug.Log("Property : " + props.getDescription() + " has value: " + props.item_properties["attack"].ToString() );
				//}
				}
			}
		}

		if (Input.GetKeyDown ("v")) {
			theActor.DropTopItem( this.transform.position, gController );
		}

		if( Input.GetKeyDown( "t" ) ) {
			hit = Physics2D.Raycast(mousePosition, -Vector2.up);
			if(hit.collider != null){
			    if(hit.collider.tag == "NPC") {
			        Debug.Log ( "Let's start trade " );
				}
			}
		}
	}
}
