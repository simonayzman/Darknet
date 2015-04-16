using UnityEngine;
using System.Collections;
using System;

public abstract class CBaseState {
	// Pure virtual function
	public abstract CBaseState GetNextState();
	// print the string
	public abstract string ToString();
	public abstract string message();	
}
	

public class CExpecting : CBaseState {

	public override CBaseState GetNextState(){
		return new CMeeting ();	
	}

	public override string message(){
		return "Haven't seen my bro around.";
	}
	public override string ToString(){
		return "Expecting";
	}
}


public class CMeeting : CBaseState {

	public override CBaseState GetNextState(){
		return new CAfterMeeting ();
	}
	public override string message(){
			return "Take this wooden sword and defeat your enemy, young Bro.";
	}
	public override string ToString(){
		return "Meeting";
	}
}


public class CAfterMeeting : CBaseState {
    public override CBaseState GetNextState(){
		return new CAfterMeeting ();
	}
	public override string message(){
		return "I already told you what to do, bro.\nNow I will split. Bye. ";
	}
	public override string ToString ()
	{
		return "AfterMeeting";
	}
}

public class StateMachineTeacher : MonoBehaviour {

	CBaseState current_state; 
	bool met_bro = false;    // have teacher meet bro
	public GameObject teacher_item;
	public GameObject death_animation;
	private GameObject d_a;
	private bool animation_started = false;

	// Handles the next state
	public void StateChanged(){
		current_state = current_state.GetNextState ();
	}
	public void GetStateName(){
		Debug.Log ( current_state.ToString() );
	}
	public void message(){
		Debug.Log ( current_state.message() );
	}

	// Use this for initialization
	void Start () {
		current_state = new CExpecting();
	}

	IEnumerator deathScene(bool initial) {

		if ( initial ) {
			d_a = Instantiate ( death_animation, gameObject.transform.position, Quaternion.identity ) as GameObject;
			yield return null;
		}

		yield return new WaitForSeconds( 6f );
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
		if (!met_bro && String.Equals (current_state.ToString (), "Meeting")) {
			Instantiate (teacher_item, new Vector2 (2, 1), Quaternion.identity);
			met_bro = true;
		} else if (String.Equals (current_state.ToString (), "AfterMeeting")) {
			animateDeath();
		}
	}
}

