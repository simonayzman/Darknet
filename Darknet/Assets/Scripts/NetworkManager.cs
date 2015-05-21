// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

//Raymond Liang

using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    public GameObject worldPrefab;
	public GameObject inventory;
	public GameObject playerData;
	public GameObject UIWindow;

	void Start() {
		PhotonNetwork.ConnectUsingSettings("0.1");
		// PhotonNetwork.logLevel = PhotonLogLevel.Full
	}
	

	void OnGUI() {
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}

	void OnJoinedLobby() {
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log("Failed to join random room!");
		PhotonNetwork.CreateRoom(null);
		Debug.Log("New room created!");
	}

	void OnJoinedRoom() {
		spawnWorld();
	}


    void spawnWorld(){
    	Debug.Log("Spawning da world!");
        Instantiate(worldPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(inventory, Vector3.zero, Quaternion.identity);
        Instantiate(UIWindow, Vector3.zero, Quaternion.identity);
		Instantiate(playerData, Vector3.zero, Quaternion.identity);
    }

}
