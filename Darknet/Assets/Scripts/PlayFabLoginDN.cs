using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

//namespace PlayFab.Examples {
	public class PlayFabLoginDN : MonoBehaviour {
		// Error messages and labels.
		public string title = "User Login";
		public string userNameLabel = "User Name";
		public string passwordLabel = "Password";
		public string nextScene = "PFDN_PurchaseScene";
		public string previousScene = "PFDN_UserRegisterScene";
		public Texture2D playfabBackground,cursor;
		public string accountNotFound = "That account could not be found.";
		public string accountBanned = "That account has been banned.";
		public string invalidPassword = "Password is invalid (6-24 characters).";
		public string invalidUsername = "Username is invalid (3-24 characters).";
		public string wrongPassword = "Wrong password for that user.";

		private string errorLabel = "";
		private GUIStyle errorLabelStyle = new GUIStyle();

		private string userNameField = "";
		private string passwordField = "";
		private float yStart;
		private bool isPassword = true;
		private bool returnedError = false;
		public bool LoggedIn = false;
		public GameObject NetworkManagerPrefab;

		private void Start() {
			errorLabelStyle.normal.textColor = Color.red;
		}

		// Login Form
		void OnGUI() {
			// Game state 2 -> Login
			if (PlayFabGameBridge.gameState == 2) {
				// If have credentials already, move to game.
				if(PlayFabData.SkipLogin && PlayFabData.AuthKey != null){
					PlayFabGameBridge.gameState = 3;
					// Unpause the game.
					Time.timeScale = 1.0f;
				}
				else {
					// Pause the game while showing UI.
					Time.timeScale = 0.0f;

					Rect winRect = new Rect(0,0,playfabBackground.width, playfabBackground.height);
					winRect.x = (int) (Screen.width * 0.5f - winRect.width * 0.5f);
					winRect.y = (int) (Screen.height * 0.5f - winRect.height * 0.5f);
					yStart = winRect.y + 80;
					GUI.DrawTexture(winRect, playfabBackground);

					if (!isPassword) {
						errorLabel = invalidPassword;
					}
					else if (!returnedError) {
						errorLabel = "";
					}

					GUI.Label (new Rect (winRect.x + 18, yStart - 16, 120, 30), "<size=18>"+title+"</size>");
					GUI.Label (new Rect (winRect.x + 18, yStart + 25, 120, 20), userNameLabel);
					GUI.Label (new Rect (winRect.x + 18, yStart + 50, 120, 20), passwordLabel);
					GUI.Label (new Rect (winRect.x + 18, yStart + 73, 120, 20), errorLabel, errorLabelStyle);
					GUI.Label (new Rect (winRect.x + 18, yStart + 145, 120, 20), "OR");
							
					userNameField = GUI.TextField (new Rect (winRect.x+130, yStart+25, 100, 20), userNameField);
					passwordField = GUI.PasswordField  (new Rect (winRect.x+130, yStart+50, 100, 20), passwordField,"*"[0], 20);

					// Initiate login request via PlayFab.
					if (GUI.Button (new Rect (winRect.x+18, yStart+100, 100, 30), "Login")||Event.current.Equals(Event.KeyboardEvent("[enter]"))) {
						if(userNameField.Length > 0 && passwordField.Length > 0) {
							returnedError = false;
							LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
							request.Username = userNameField;
							request.Password = passwordField;
							request.TitleId = PlayFabData.TitleId;
							PlayFabClientAPI.LoginWithPlayFab(request, OnLoginResult, OnPlayFabError);
						}
						else {
							isPassword = false;
						}
					}

					// Go to registration form if button pressed.
					if (GUI.Button(new Rect(winRect.x + 18, yStart + 175, 120, 20), "Register")) {
						PlayFabGameBridge.gameState = 1;
					}


					if (Input.mousePosition.x < winRect.x + winRect.width && Input.mousePosition.x > winRect.x && Screen.height - Input.mousePosition.y < winRect.y + winRect.height && Screen.height - Input.mousePosition.y > winRect.y) {
						Rect cursorRect = new Rect(Input.mousePosition.x,Screen.height-Input.mousePosition.y,cursor.width,cursor.height );
						GUI.DrawTexture(cursorRect, cursor);
					}
				}
			}
		}

		// Callback function: Play game if player login is successful
		public void OnLoginResult(LoginResult result) {
			// Switch to game. Hide the dialog.
			PlayFabGameBridge.gameState = 3;
			// Unpause the game.
			Time.timeScale = 1.0f;
			PlayFabData.AuthKey = result.SessionTicket;
			Instantiate(NetworkManagerPrefab, Vector3.zero, Quaternion.identity);
			Debug.Log("Login successful!");
		}

		// Callback function: Error message display.
		void OnPlayFabError(PlayFabError error)
		{
			returnedError = true;
			Debug.Log ("Got an error: " + error.Error);
			if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password")) {
				errorLabel = invalidPassword;
			}
			else if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username")) {
				errorLabel = invalidUsername;
			}
			else if (error.Error == PlayFabErrorCode.AccountNotFound) {
				errorLabel = accountNotFound;
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned) {
				errorLabel = accountBanned;
			}
			else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword) {
				errorLabel = wrongPassword;
			}
			else {
				errorLabel = "Unknown Error.";
			}
		}
	}
//}