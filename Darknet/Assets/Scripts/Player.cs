using UnityEngine;
using System.Collections;
using PlayFab;
using System.Collections.Generic;

public class Player : Photon.MonoBehaviour {
    // Public game variables
	public string username;
	public List<GameObject> inventory;	
	public int xpValue; // when this player gets killed how much xp will killer get
	public bool is_alive;    // used for the purposes of detecting when this player dies by the masterClient
	public int player_id;   // every NPC will have unique id

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
	public int inventory_capacity;

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
	
	// Automatically called every time player receives or sends data.
	// "State synchronization" - constantly updates values over the network. Useful for data that changes often.
	// OnSerializeNetworkView() used to customized synchronization of variables in a script watched by the network view.

    void Start(){
    	inventory_capacity = 4;
    	is_alive = true;
    }

	// void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		int health = 0;
		int mana = 0;
		int xp = 0;
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
            xp = currentEXP;
            stream.Serialize(ref xp);
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
			stream.Serialize(ref xp);
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
				
			if(currentHealth <= 0){
				GameObject world = GameObject.FindGameObjectWithTag("World");
		        if(world){
				    world.GetComponent<GCtrller>().playerDied(gameObject);
				}
			}		
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
	

	// Remote procedure calls (RPC) useful for data that does not constantly change.
	// Adding "[RPC]" allows it to be called over the network.
	// RPC sent by caling networkView.RPC().
	[RPC] public void UpdateHP(int hitpoints) {
		HP = hitpoints;
		if (!photonView.isMine) {
			photonView.RPC("UpdateHP", PhotonTargets.OthersBuffered, hitpoints);
		}
	}
	
	
	[RPC] void UpdateCurrentHP(int damage) {	
		if(currentHealth > 0 && damage >= currentHealth){
			Debug.Log(PhotonNetwork.playerName + " claims the kill.");
			photonView.RPC("ClaimKill", PhotonTargets.AllBuffered, this.player_id, PhotonNetwork.playerName);
		}

		currentHealth -= damage;
	}

    [RPC] void ClaimKill(int playa_id, string client){
    	GameObject world = GameObject.FindGameObjectWithTag("World");
    	if(world){
    		GameObject playa = world.GetComponent<GCtrller>().npcs_in_game[playa_id] as GameObject;
	        if(playa && PhotonNetwork.isMasterClient && playa.GetComponent<Player>().player_id == playa_id){
	        	Debug.Log("Got Kill Claim from " + client);
	        	playa.GetComponent<Player>().is_alive = false;
	        	photonView.RPC("UpdatePlayerEXP", PhotonTargets.AllBuffered, playa.GetComponent<Player>().xpValue, client);
	        }
	    }
    }

    [RPC] void UpdatePlayerEXP(int deltaXP, string client) {
        if(PhotonNetwork.playerName == client){
        	Debug.Log("Kill granted to: " + client + " worth " + deltaXP + " xp points.");
        	GameObject world = GameObject.FindGameObjectWithTag("World");
        	if(world){
        		GameObject player = world.GetComponent<GCtrller>().da_player;
        		if(player){
        			player.GetComponent<Player>().currentEXP += deltaXP;
        		}
        	}
        }
    }

    public void dealDamage(GameObject other, int damage){
    	other.GetComponent<Player>().acceptDamage(damage);
    }

    public void acceptDamage(int damage){
    	photonView.RPC("UpdateCurrentHP", PhotonTargets.All, damage);
    	if(gameObject.tag == "NPC"){
    		gameObject.GetComponent<NPCController>().fightBack();
    	}
    }

	public void hp_status(){
    	Debug.Log("I am " + PhotonNetwork.playerName + " and have " + currentEXP + " xp");
    }

    public bool isDead(){
    	return currentHealth <= 0;
    }

    public string printProfile() {
		//{"STR": "10", "DEX": "20", "INT": "10", "VIT": "10", "BaseHP": "100", "BaseMP": "100", "BasePhysAtk": "10", "BaseMagAtk": "5"}
		return "{\"STR\": \""+baseSTR+"\", \"DEX\": \""+baseDEX+"\", \"INT\": \""+baseINT+"\", \"VIT\": \""+baseVIT+"\", \"BaseHP\": \""+baseHP+"\", \"BaseMP\": \""+baseMP+"\", \"BasePhysAtk\": \""+basePhysAtk+"\", \"BaseMagAtk\": \""+baseMagAtk+"\"}";
	}

    public void setID(int id){
    	player_id = id;
    }

	[RPC] void PickUpItem(int x_pos, int y_pos, string who){
        if(PhotonNetwork.isMasterClient){
        	int item_index = -1;
       	    Debug.Log("Got request to pick up item at " + x_pos + ", " + y_pos + " from " + who);
       	    GCtrller world = GameObject.FindGameObjectWithTag("World").GetComponent<GCtrller>();
       	    if( world ){
	       	    for(int i = 0; i < world. items_in_game.Count; ++i){
	       	    	Debug.Log(world.items_in_game[i].name + " at: " + world.items_in_game[i].transform.position.x +
	       	    		", " + world.items_in_game[i].transform.position.y);
	                if(world.items_in_game[i].transform.position.x == x_pos && 
	                   world.items_in_game[i].transform.position.y == y_pos &&
	                   !world.items_in_game[i].GetComponent<Item>().is_picked){
	                   	string item_name = world.items_in_game[i].name;
	                    world.items_in_game[i].GetComponent<Item>().is_picked = true;
	                	Debug.Log("Item available to be picked up");
                        item_index = i;
	                    photonView.RPC("AssignItem", PhotonTargets.AllBuffered, item_name, who); 
	                }
	       	    }
	       	    PhotonNetwork.Destroy (world.items_in_game[item_index]);
	       	    world.items_in_game.Remove(world.items_in_game[item_index]);
	       	    Debug.Log("There are " + world.items_in_game.Count + " items in the game");
	       	}

        }
       
	}

    [RPC] void AssignItem(string item, string who) {
    	if(who == PhotonNetwork.playerName){
    		Debug.Log("I: " + PhotonNetwork.playerName + " got permission to pick up item: " + item );
    	}
    }
    
}