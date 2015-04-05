using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NPCController : MonoBehaviour {
	
	private GameController gController;
	Actor theActor;
	public GameObject my_object;
	public GameObject death_animation;
	private bool animation_started = false;
	GameObject d_a;

	// Use this for initialization
	void Start () {

		theActor = gameObject.GetComponent<Actor> ();
		theActor.inventory = new List<GameObject> ();
		theActor.pickUpItem (Instantiate (my_object, new Vector2 (0, 0), Quaternion.identity) as GameObject);

	}

	IEnumerator deathScene(bool initial) {

		if ( initial ) {
			d_a = Instantiate ( death_animation, gameObject.transform.position, Quaternion.identity ) as GameObject;
			yield return null;
		}
	
		yield return new WaitForSeconds( 6f );
		theActor.DropTopItem(gameObject.transform.position);  
		Destroy( gameObject );

		Destroy ( d_a );
	}

	private void animateDeath(){
		if( !animation_started ){
			StartCoroutine( deathScene( true ) );
			animation_started = true;
		} else {
			StartCoroutine( deathScene( false ) );
		}
	}

	// Update is called once per frame
	void Update () {

		if (theActor.isDead ()) {
			animateDeath ();
		}
	}
}
