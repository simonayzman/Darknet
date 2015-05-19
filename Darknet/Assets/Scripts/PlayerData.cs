using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;

public class PlayerData : MonoBehaviour {
	public Player myPlayer;
	public GameObject chatGUI;

	void Start() {
		myPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
		PlayerConfiguration();
	}
	
	private void PlayerConfiguration() {
		// Account information to obtain in-game name from PlayFab.
		GetAccountInfoRequest AccountInfoRequest = new GetAccountInfoRequest();
		if (PlayFabData.AuthKey != null) {
			PlayFabClientAPI.GetAccountInfo(AccountInfoRequest, LoadAccountInfo, OnPlayFabError);
		}
		// Obtain title-specific data from PlayFab.
		GetUserDataRequest UserDataRequest = new GetUserDataRequest();
		if (PlayFabData.AuthKey != null) {
			PlayFabClientAPI.GetUserData(UserDataRequest, LoadPlayerData, OnPlayFabError);
		}
	}

	private void LoadAccountInfo(GetAccountInfoResult result) {
		Debug.Log("Fetching player in-game name...");
		// Load player data based on key-value pairs from PlayFab
		myPlayer.username = result.AccountInfo.TitleInfo.DisplayName;
		PhotonNetwork.playerName = myPlayer.username;
		Instantiate(chatGUI, Vector3.zero, Quaternion.identity);
	}

	private void LoadPlayerData(GetUserDataResult result) {
		Debug.Log("Loading player data...");
		// Load player data based on key-value pairs from PlayFab.
		if (result.Data.ContainsKey("Level")) {
			myPlayer.currentEXP = int.Parse(result.Data["CurrentExperience"].Value);
			myPlayer.currentLevel = int.Parse(result.Data["Level"].Value);
			myPlayer.baseHP = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["BaseHP"].Value);
			myPlayer.baseMP = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["BaseMP"].Value);
			myPlayer.baseSTR = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["STR"].Value);
			myPlayer.baseDEX = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["DEX"].Value);
			myPlayer.baseINT = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["INT"].Value);
			myPlayer.baseVIT = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["VIT"].Value);
			myPlayer.basePhysAtk = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["BasePhysAtk"].Value);
			myPlayer.baseMagAtk = int.Parse(JSON.Parse(result.Data["Attributes"].Value)["BaseMagAtk"].Value);
			
			myPlayer.Race = JSON.Parse(result.Data["Race"].Value);
			myPlayer.Class = JSON.Parse(result.Data["Class"].Value);
			
			myPlayer.Weapon = JSON.Parse(result.Data["Equipped"].Value)["Weapon"].Value;
			myPlayer.Offhand = JSON.Parse(result.Data["Equipped"].Value)["Offhand"].Value;
			myPlayer.Armor = JSON.Parse(result.Data["Equipped"].Value)["Armor"].Value;
		}
		else {
			// New character! Create key-value pairs.
			CreateNewPlayer();
		}
	}
	
	private void CreateNewPlayer() {
		if (PlayFabData.AuthKey != null) {
			Debug.Log("Initializing player data...");
			UpdateUserDataRequest request = new UpdateUserDataRequest();
			request.Data = new Dictionary<string, string>();
			request.Data.Add("Race","N/A");
			request.Data.Add("Class", "N/A");
			request.Data.Add("Level", "1");
			request.Data.Add("CurrentExperience", "0");
			request.Data.Add("Equipped", "{\"Weapon\": \"none\", \"Offhand\": \"none\", \"Armor\": \"none\"}");
			request.Data.Add("QuickSlots", "{\"Slot1\": \"null\", \"Slot2\": \"null\", \"Slot3\": \"null\", \"Slot4\": \"null\", \"Slot5\": \"null\"}");
			request.Data.Add("SkillsLevel", "");
			request.Data.Add("Attributes", myPlayer.printProfile());
			PlayFabClientAPI.UpdateUserData(request, PlayerDataSaved, OnPlayFabError);
		}
	}
	
	private void SavePlayerState() {
		if (PlayFabData.AuthKey != null) {
			Debug.Log("Saving player data...");
			UpdateUserDataRequest request = new UpdateUserDataRequest();
			request.Data = new Dictionary<string, string>();
			// Obtain values from player script.
			request.Data.Add("Race", myPlayer.Race);
			request.Data.Add("Class", myPlayer.Class);
			request.Data.Add("Level", myPlayer.currentLevel.ToString());
			request.Data.Add("CurrentExperience", myPlayer.currentEXP.ToString());
			request.Data.Add("Attributes", "");
			// Obtain values from 
			request.Data.Add("Equipped", "{\"Weapon\": \"none\", \"Offhand\": \"none\", \"Armor\": \"none\"}");
			request.Data.Add("QuickSlots", "{\"Slot1\": \"null\", \"Slot2\": \"null\", \"Slot3\": \"null\", \"Slot4\": \"null\", \"Slot5\": \"null\"}");
			request.Data.Add("SkillsLevel", ""/*getSkillsProfile()*/);
			PlayFabClientAPI.UpdateUserData(request, PlayerDataSaved, OnPlayFabError);
		}
	}
	
	private void PlayerDataSaved(UpdateUserDataResult result) {
		Debug.Log("Player data saved.");
	}
	
	public void onDestroy() {
		SavePlayerState();
	}
	
	void OnPlayFabError(PlayFabError error) {
		Debug.Log("Got an error: " + error.ErrorMessage);
	}
	
}