using UnityEngine;
using System.Collections;
using PlayFab;

public class Player : Photon.MonoBehaviour {
	// Public game variables
	public float speed = 10f;
	public int currentHealth = 100;
	public int currentMana = 100;
	public int hp = 100;
	public int mp = 100;
	public int exp = 0;
	public int str = 10;
	public int dex = 10;
	public int intl = 10;
	public int level = 10;
	
	// Private variables
	// Sync variables are used to reduce latency issue betwee instances due to send rate. Thus use interpolation.
	// These variables are also used for prediction to have smoother movement.
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector2 syncStartPosition = Vector2.zero;
	private Vector2 syncEndPosition = Vector2.zero;
	
	// Automatically called every time player receives or sends data.
	// "State synchronization" - constantly updates values over the network. Useful for data that changes often.
	// OnSerializeNetworkView() used to customized synchronization of variables in a script watched by the network view.

	// void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		int health = 0;
		int mana = 0;
		// If sending data
		if (stream.isWriting) {
			// Store current position.
			syncPosition = rigidbody2D.position;
			// stream.Serialize allows for variable to be serialized and sent to other clients.
			// Order of variables streamed should be the same when sending and receiving to avoid value mixups.
			stream.Serialize(ref syncPosition);
			
			syncVelocity = rigidbody2D.velocity;
			stream.Serialize(ref syncVelocity);
			
			// Player value changes.
			health = currentHealth;
			stream.Serialize(ref health);
		}
		// If receiving data
		else {
			// stream.Serialize allows for variables to be serialized and received by clients.
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			// Synchronization adjustment.
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			// Store start position for future calls.
			syncStartPosition = syncPosition + syncVelocity * syncDelay;
			// Store data locally
			syncEndPosition = syncPosition;
			
			// Player value updates.
			stream.Serialize(ref health);
			stream.Serialize(ref mana);
		}
	}
	
	// MonoBehaviour.Awake() called whenever script instance is loaded.
	// MonoBehaviour.Awake() used to initialize variables/game state before game starts. Can only be called once during lifetime of script.
	// Called for any Start() functions.
	void Awake() {
		// Once you log in, set baseline for synchronization.
		lastSynchronizationTime = Time.time;
		// Retrieve previously stored player data from PlayFab.
		PlayFabData.LoadData();
	}
	
	// Update is called once per frame
	void Update() {
		// Updates based on host input
		if (photonView.isMine) {
			InputMovement();
			InputEvents();			
		}
		else {
			SyncMovement();
		}
	}
	
	// WASD movement.
	void InputMovement() {
		if (Input.GetKey(KeyCode.W)) {
			Debug.Log("Up");
			rigidbody2D.MovePosition(rigidbody2D.position + Vector2.up * speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S)) {
			Debug.Log("Down");
			rigidbody2D.MovePosition(rigidbody2D.position - Vector2.up * speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D)) {
			Debug.Log("Right");
			rigidbody2D.MovePosition(rigidbody2D.position + Vector2.right * speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.A)) {
			Debug.Log("Left");
			rigidbody2D.MovePosition(rigidbody2D.position - Vector2.right * speed * Time.deltaTime);
		}
	}
	
	// Sync WASD movement
	private void SyncMovement() {
		syncTime += Time.deltaTime;
		rigidbody2D.position = Vector2.Lerp(syncStartPosition, syncEndPosition, (syncTime+1)/(syncDelay+1));
	}
	
	private void InputEvents() {
		/*
		GAME LOGIC
		*/
	}
	
	// Remote procedure calls (RPC) useful for data that does not constantly change.
	// Adding "[RPC]" allows it to be called over the network.
	// RPC sent by caling networkView.RPC().
	[RPC] void UpdateHP(int hitpoints) {
		hp = hitpoints;
		if (photonView.isMine) {
			photonView.RPC("UpdateHP", PhotonTargets.OthersBuffered, hp);
		}
	}
	
	[RPC] void UpdateMP(int manapoints) {
		mp = manapoints;
		if (photonView.isMine) {
			photonView.RPC("UpdateMP", PhotonTargets.OthersBuffered, mp);
		}
	}
	
	[RPC] void UpdateEXP(int experience) {
		exp = experience;
		if (photonView.isMine) {
			photonView.RPC("UpdateEXP", PhotonTargets.OthersBuffered, exp);
		}
	}
	
	[RPC] void UpdateLVL(int LVL) {
		level = LVL;
		if (photonView.isMine) {
			photonView.RPC("UpdateLVL", PhotonTargets.OthersBuffered, level);
		}
	}
	
	[RPC] void UpdateBaseStats(int STR, int DEX, int INT) {
		str = STR;
		dex = DEX;
		intl = INT;
		if (photonView.isMine) {
//			photonView.RPC("UpdateBaseStats", PhotonTargets.OthersBuffered,)
		}
	}
	
	[RPC] void UpdateCurrentHP(int currentHP) {
		currentHealth = currentHP;
	}
	
	[RPC] void UpdateCurrentMP(int currentMP) {
		currentMana = currentMP;
	}
}