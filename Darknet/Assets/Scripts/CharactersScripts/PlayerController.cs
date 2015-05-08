using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Photon.MonoBehaviour {

	private Vector2 playerPosition;
	private Vector2 mousePosition;
	RaycastHit2D hit;
	float distance;
   
	// Use this for initialization
	void Start () {
	    
	    GameObject world = GameObject.FindWithTag ("World");
		if (world == null)
			Debug.Log ("No GameController");
		else {
			Debug.Log("We have world");
		}
        
	}
	
	// Update is called once per frame
	void Update () {
		handleInput();
	}
    
    private void handleInput(){
        playerPosition = this.transform.position;
        // seems like I have to do left click before right click WHY?
		if (Input.GetMouseButtonDown(1)){  // left click
			playerPosition = this.transform.position;
			// this was killing me I was using Input.mousePoistion as position to cast ray from
			// this version is correct
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			hit = Physics2D.Raycast(mousePosition, -Vector2.up);

			if(hit.collider != null){

				if(hit.collider.tag == "Teacher"){ 
					distance = Vector2.Distance(playerPosition, hit.collider.gameObject.transform.position);
					if( distance < .5 ){
						Debug.Log("EASY young Bro. I am your teacher.");
					}
				}
				else if(hit.collider.tag == "NPC"){
					GameObject who = hit.collider.gameObject;
					distance = Vector2.Distance(playerPosition, who.transform.position);
					if( distance < .5 ){
					   Debug.Log("Ready to start the fight with: " + who.name);
					   who.GetComponent<Player>().dealDamage(who, 20);
					}

			    }
				else if(hit.collider.tag == "Item"){
					GameObject gameItem = hit.collider.gameObject;
					distance = Vector2.Distance(playerPosition, mousePosition);
					if( distance < 0.45){
						Debug.Log("Ready to pick up da " + gameItem.GetComponent<Item>().getDescription() + ", Bro.");
						 photonView.RPC("PickUpItem", PhotonTargets.All, (int) gameItem.transform.position.x, 
						 	(int) gameItem.transform.position.y, PhotonNetwork.playerName);
					}
					else{
						Debug.Log("Too far, Bro.");
						//Debug.Log( "Das item est " + gameItem.GetComponent<Item>().description );
				       
					}
				}
			}
				
		}

		if (Input.GetMouseButtonDown(0)) {  // right click
			hit = Physics2D.Raycast(mousePosition, -Vector2.up);
			
			if(hit.collider != null){
				if(hit.collider.tag == "Teacher"){ 
					distance = Vector2.Distance(playerPosition, hit.collider.gameObject.transform.position);
					if( distance < 1.0 ){
						hit.collider.GetComponent<StateMachineTeacher>().StateChanged();
						hit.collider.GetComponent<StateMachineTeacher>().message();
					}
				}
				else if(hit.collider.tag == "NPC"){
					GameObject who = hit.collider.gameObject;
					Debug.Log("Let's talk: " + who.name);
                    who.GetComponent<Player>().hp_status();
				}
			}
		}

        if( Input.GetKeyDown( "h") ) {
        	gameObject.GetComponent<Player>().hp_status();
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


    public void teleportPlayer(Vector3 new_position){
    	gameObject.transform.position = new_position;
    }

}
