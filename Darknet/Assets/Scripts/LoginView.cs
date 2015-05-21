// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

//Raymond Liang

using UnityEngine;
using System.Collections;

public class LoginView : MonoBehaviour {
	public bool RegistrationGUI = false;
	private string username = "";
	private string password = "";
	private string confirm = "";
	private string status ="";

	// Called once per frame.
	void Update() {
		OnGUI();
	}

	// Login Form
	void LoginAuthentication(string USERNAME, string PASSWORD) {

	}

	// RegisterUser
	void RegisterUser(string USERNAME, string PASSWORD) {

	}

	void OnGUI() {
		GUI.Window(0, new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), LoginWindow, "Log In");
	}

	void LoginWindow(int windowId) {
		// User + Password UI labels.
//		GUI.Label(new Rect());
		GUI.Label(new Rect(10, 50, 100, 30), "Username:");
		GUI.Label(new Rect(10, 90, 100, 30), "Password:");
		if (GUI.Button(new Rect(310, 50, 100, 30), "Log In")) {
			LoginAuthentication(username, password);
		}
		username = GUI.TextField (new Rect(100, 50, 200, 30), username, 25);
		password = GUI.PasswordField(new Rect(100, 90, 200, 30), password, "*"[0], 25);
		if (GUI.Button(new Rect(310, 90, 100, 30), "New Account")) {
			RegistrationGUI = true;
		}
		if (RegistrationGUI) {
			GUI.Label(new Rect(10, 130, 100, 30), "Confirm password:");
			confirm = GUI.PasswordField(new Rect(100, 130, 200, 30), confirm, "*"[0], 25);
			if (confirm == password && GUI.Button (new Rect(310, 130, 100, 30), "Register")) {
				RegisterUser(username, password);
			}
		}
	}

	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log(username + " connected from " + player.ipAddress + ":" + player.port);
	}
}
