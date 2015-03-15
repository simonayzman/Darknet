using UnityEngine;
using System.Collections;

public class Lobby : MonoBehaviour {
	private const string typeName = "DarkNet";
	private const string gameName = "RoomName";
	
	private bool isRefreshingHostList = false;
	private HostData[] hostList;
	
	public GameObject playerPrefab;
	
	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			// Start server button.
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server")) {
				StartServer();
			}
			// Refresh host list button.
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts")) {
				RefreshHostList();
			}
			// Display rooms on server if any.
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					// Room button to join.
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName)) {
						JoinServer(hostList[i]);
					}
				}
			}
		}
	}
	
	private void StartServer() {
		//Network.InitializeServer(int MaxNumIncomingConnections, int ListenPort, bool useNAT)
		Network.InitializeServer(20, 25000, !Network.HavePublicAddress());
		//MasterServer.RegisterHost(string gameTypeName, string gameName, string comment)
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	// Called whenever Network.InitializeServer() is invoked and completed.
	void OnServerInitialized() {
		SpawnPlayer();
	}
	
	// Called upon every frame if MonoBehaviour is enabled.
	void Update() {
		if (isRefreshingHostList && MasterServer.PollHostList().Length > 0) {
			// Set false so that it does not continuous fetch hostList.
			isRefreshingHostList = false;
			// Checked for latest host list rceived by RequestHostList() and store locally.
			hostList = MasterServer.PollHostList();
		}
	}
	
	// Refresh host list.
	private void RefreshHostList() {
		if (!isRefreshingHostList) {
			// Set true for update frame.
			isRefreshingHostList = true;
			// Obtain host list from master server.
			MasterServer.RequestHostList(typeName);
		}
	}
	
	// Join server
	private void JoinServer(HostData hostData) {
		// Network.Connect(string IP, int remotePort, string password)
		// Note HostData class can hold this information.
		Network.Connect(hostData);
	}
	
	// Executes when connected to server. Called on client.
	void OnConnectedToServer() {
		SpawnPlayer();
	}
	
	// Script to spawn player.
	private void SpawnPlayer() {
		// Network.Instantiate(Object prefab, Vector3 position, Quaternion rotation, int group)
		// Instantiate handles synchronization between clients over the network.
		Network.Instantiate(playerPrefab, Vector3.up * 5, Quaternion.identity, 0);
	}
}
