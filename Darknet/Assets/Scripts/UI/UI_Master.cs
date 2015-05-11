using UnityEngine;
using System.Collections;

public class UI_Master : MonoBehaviour {

	[SerializeField] public Canvas gameplay;
	[SerializeField] public Canvas login;
	[SerializeField] public Canvas character;
		
	public int state;


	// Use this for initialization
	void Start () {
		state = 0; //login
		login.enabled = true;
		character.enabled = false;
		gameplay.enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0)
			toLogin();
		else if (state == 1)
			toCharacter();
		else if (state == 2)
			toGameplay();
	
	}
	

	void  toCharacter() {
		state = 1; //character select
		login.enabled = false;
		character.enabled = true;
		gameplay.enabled = false;
	}

	void  toGameplay() {
		state = 2; //gameplay
		login.enabled = false;
		character.enabled = false;
		gameplay.enabled = true;
	}


	void  toLogin() {
		state = 0; //login
		login.enabled = true;
		character.enabled = false;
		gameplay.enabled = false;
	}


}
