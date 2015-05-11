using UnityEngine;
using System.Collections;
using PlayFab;
using System.Collections.Generic;

public class Player : Photon.MonoBehaviour {
    // Public game variables
	public string username;
	public List<GameObject> inventory;	

	// Player profile stats loaded from PlayFab
	public int currentEXP;
	public int EXPToLevel;
	public int currentLevel;
	public int baseHP;
	public int baseMP;
	public int baseSTR;
	public int baseDEX;
	public int baseINT;
	public int baseVIT;
	public int basePhysAtk;
	public int baseMagAtk;
	public string Race;
	public string Class;
	
	// Equipments
	public string Weapon;
	public string Offhand;
	public string Armor;

	// Calculated profile stats with addition of in-game items (and logic).
	public int currentHealth;
	public int currentMana;
	public int HP;
	public int MP;
	public int STR;
	public int DEX;
	public int INT;
	public int VIT;
	public int physAtk;
	public int magAtk;
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

	public int inventory_capacity;

	public int getAttribute (string name){
		if (name == "hp")
			return currentHealth;
		else if (name == "hpmax")
			return HP;
		else if (name == "mp")
			return currentMana;
		else if (name == "mpmax")
			return MP;
		else if (name == "exp")
			return currentEXP;
		else if (name == "expmax")
			return EXPToLevel;
		else if (name == "inv")
			return inventory.Count;
		else if (name == "invmax")
			return inventory_capacity;
		else if (name == "lv")
			return currentLevel;
		else 
			return -1;
	}

    void Start(){
    	
    }
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
	}
	
	// Update is called once per frame
	void Update() {
		// Updates based on host input
		if (photonView.isMine) {
			gameObject.GetComponent<Movement>().InputMovement();
			if(this.tag == "Player")
				InputEvents();			
		}
		else {
			SyncMovement();
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
	[RPC] public void UpdateHP(int hitpoints) {
		HP = hitpoints;
		if (!photonView.isMine) {
			photonView.RPC("UpdateHP", PhotonTargets.OthersBuffered, hitpoints);
		}
	}
	
	
	[RPC] void UpdateCurrentHP(int thisHP, string who) {
		Debug.Log("NPC Attacked.");
		currentHealth = thisHP;
		
	}

	[RPC] void PickUpItem(int x_pos, int y_pos, string who){
       if(PhotonNetwork.isMasterClient){
       	    Debug.Log("Got request to pick up item at " + x_pos + ", " + y_pos + " from " + who);
       	    GameObject world = GameObject.FindGameObjectWithTag ("World");
       	    if(world){
       	        Debug.Log("There are " + world.GetComponent<GCtrller>().items_in_game.Count + " items in da game.");
       	    }
       }
       if(who == PhotonNetwork.playerName){
       	    Debug.Log("It's my bro player who should get da item");
       }
	}
	
    public void dealDamage(GameObject other, int damage){
    	other.GetComponent<Player>().acceptDamage(damage);
    }

    public void acceptDamage(int damage){
    	photonView.RPC("UpdateCurrentHP", PhotonTargets.All, currentHealth - damage, PhotonNetwork.playerName);
    }

	public void hp_status(){
        int current_hp = gameObject.GetComponent<Player>().currentHealth;
    	Debug.Log("I am " + gameObject.name + " and have " + current_hp + "hp");
    }

    public bool isDead(){
    	return currentHealth <= 0;
    }

    public string printProfile() {
		//{"STR": "10", "DEX": "20", "INT": "10", "VIT": "10", "BaseHP": "100", "BaseMP": "100", "BasePhysAtk": "10", "BaseMagAtk": "5"}
		return "{\"STR\": \""+baseSTR+"\", \"DEX\": \""+baseDEX+"\", \"INT\": \""+baseINT+"\", \"VIT\": \""+baseVIT+"\", \"BaseHP\": \""+baseHP+"\", \"BaseMP\": \""+baseMP+"\", \"BasePhysAtk\": \""+basePhysAtk+"\", \"BaseMagAtk\": \""+baseMagAtk+"\"}";
	}
}