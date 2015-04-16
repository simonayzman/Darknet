using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayFab;
using PlayFab.ClientModels;

//namespace PlayFab.Examples {
	public class PlayFabRegisterDN : MonoBehaviour {
		// Error messages and labels
		public string title = "User Registration";
		public string userNameLabel = "User Name";
		public string emailLabel = "Email";
		public string passwordLabel = "Password";
		public string confirmPasswordLabel = "Confirm Password";
		public string nextScene = "PFDN_UserLoginScene";
		public string confirmScene = "PFDN_PurchaseScene";
		public Texture2D playfabBackground,cursor;
		public string emailNotAvailable = "That email address has already been registered.";
		public string usernameNotAvailable = "That username has already been already taken.";
		public string invalidPassword = "Password is invalid (6-24 characters).";
		public string invalidUsername = "Username is invalid (3-24 characters).";

		private string errorLabel = "";
		private string userNameField = "";
		private string emailField = "";
		private string passwordField = "";
		private string confirmPasswordField = "";
		private GUIStyle errorStyle = new GUIStyle();
		private float yStart;
		private bool isEmail = true;
		private bool isPassword = true;
		private bool returnedError = false;

		private void Start() {
			PlayFabData.LoadData();
			errorStyle.normal.textColor = Color.red;
		}

		void OnGUI() {
			if (PlayFabGameBridge.gameState == 1) {
				// Pause everything while displaing registration UI.
				Time.timeScale = 0.0f;
				// Create window for reigstration form.
				Rect winRect = new Rect(0, 0, playfabBackground.width, playfabBackground.height);
				winRect.x = (int) (Screen.width * 0.5f - winRect.width * 0.5f);
				winRect.y = (int) (Screen.height * 0.5f - winRect.height * 0.5f);
				yStart = winRect.y + 80;
				GUI.DrawTexture(winRect, playfabBackground);

				// Validation of text field.
				if (!isEmail) {
					errorLabel = "Invalid email address.";
				}
				else if (!isPassword) {
					errorLabel = "Passwords do not match.";
				}
				else if (!returnedError) {
					errorLabel = "";
				}

				// Placement of labels and textfield for UI.
				GUI.Label(new Rect(winRect.x + 18, yStart - 16, 120, 30), "<size=18>" + title + "</size>");
				GUI.Label(new Rect(winRect.x + 18, yStart + 25, 120, 20), userNameLabel);
				GUI.Label(new Rect(winRect.x + 18, yStart + 50, 120, 20), emailLabel);
				GUI.Label(new Rect(winRect.x + 18, yStart + 75, 120, 20), passwordLabel);
				GUI.Label(new Rect(winRect.x + 18, yStart + 100, 120, 20), confirmPasswordLabel);
				GUI.Label(new Rect(winRect.x + 18, yStart + 200, 120, 20), "OR");

				userNameField = GUI.TextField(new Rect(winRect.x + 135, yStart + 25, 100, 20), userNameField);
				emailField = GUI.TextField(new Rect(winRect.x + 135, yStart + 50, 100, 20), emailField);
				passwordField = GUI.PasswordField(new Rect(winRect.x + 135, yStart + 75, 100, 20), passwordField,"*"[0], 20);
				confirmPasswordField = GUI.PasswordField(new Rect(winRect.x + 135, yStart + 100, 100, 20), confirmPasswordField,"*"[0], 20);
				
				GUI.Label(new Rect(winRect.x + 18, yStart + 128, 200, 20), errorLabel, errorStyle);

				if (confirmPasswordField.Length > 0) {
					validatePassword ();
				}
				
				// Attempt to register. If all email and password fields check out, registration should occur succesfully.
				if (GUI.Button(new Rect(winRect.x + 18, yStart + 155, 100, 30), "Register") || Event.current.Equals(Event.KeyboardEvent("[enter]"))) {
					validateEmail();
					validatePassword();
					// If the entered email has a valid structure and passwords match, register with the information inputted.
					if (isEmail && isPassword) {
						returnedError = false;
						RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
						request.TitleId = PlayFabData.TitleId;
						request.Username = userNameField;
						request.Email = emailField;
						request.Password = passwordField;
						Debug.Log("TitleId: " + request.TitleId);
						PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterResult, OnPlayFabError);
					}	
				}
				
				// Move to login window.
				if (GUI.Button(new Rect(winRect.x + 18, yStart + 235, 80, 20), "Log In")) {
					PlayFabGameBridge.gameState = 2;
				}

				if (Input.mousePosition.x < winRect.x + winRect.width && Input.mousePosition.x > winRect.x && Screen.height - Input.mousePosition.y < winRect.y + winRect.height && Screen.height - Input.mousePosition.y > winRect.y) {
					Rect cursorRect = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height);
					GUI.DrawTexture(cursorRect, cursor);
				}
			}
		}

		// Meet all requirments of RFCs 5321 & 5322
		const string pattern = @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
		private void validateEmail() {
			isEmail = Regex.IsMatch(emailField, pattern);
		}

		// Confirm password and confirmed password are the same.
		// Check if the email has been entered.
		private void validatePassword() {
			isPassword = ((passwordField == confirmPasswordField) && emailField.Length > 0); 
		}

		// If registered succesfully, update in-game display name (IGN) for the game.
		public void OnRegisterResult(RegisterPlayFabUserResult result) {
			UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest();
			request.DisplayName = result.Username;
			PlayFabClientAPI.UpdateUserTitleDisplayName(request, NameUpdated, OnPlayFabError);
		}

		// Callback function: Unpause and start game.
		public void NameUpdated(UpdateUserTitleDisplayNameResult result) {
			Debug.Log("Name " + result.DisplayName + " updated.");
			// Unpause and start game.
			Time.timeScale = 1.0f;
			PlayFabGameBridge.gameState = 3;

			// if (PlayFabData.AngryBotsModActivated) {
			//	Application.LoadLevel("Default");
			// else {
			//  Application.LoadLevel(confirmScene);
			// }
		}

		// Callback function: Error message display.
		void OnPlayFabError(PlayFabError error) {
			returnedError = true;
			Debug.Log("Error: " + error.Error);
			if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password")) || (error.Error == PlayFabErrorCode.InvalidPassword)) {
				errorLabel = invalidPassword;
			}
			else if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username")) || (error.Error == PlayFabErrorCode.InvalidUsername)) {
				errorLabel = invalidUsername;
			}
			else if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable) {
				errorLabel = emailNotAvailable;
			}
			else if (error.Error == PlayFabErrorCode.UsernameNotAvailable) {
				errorLabel = usernameNotAvailable;
			}
		}
	}
//}