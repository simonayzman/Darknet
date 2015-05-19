using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GCtrller :  Photon.MonoBehaviour {
	public int MAX_NO_ENEMIES;
	public GameObject player;
	public GameObject[] all_npcs; 	
	public GameObject[] all_game_items;
	public Hashtable npcs_in_game;
	public List<GameObject> items_in_game;
    public GameObject player_camera;
    public GameObject da_player;
    private GameObject[] arena_panels;
    public GameObject death_animation;
    public string da_name;
    private int npc_id;                       // counter that will provide unique id for each NPC
    
	// Use this for initialization
	void Start () {

		//PhotonNetwork.playerName = da_name;
		bool am_i = PhotonNetwork.isMasterClient;
		Debug.Log("Am I master: " + am_i);

        npc_id = 0;
        npcs_in_game = new Hashtable();

		SpawnPlayer ();
        
        
		if(PhotonNetwork.isMasterClient){
			addNPC();
		}
		
		arena_panels = GameObject.FindGameObjectsWithTag ("Panel");

        // Create camera
        Instantiate(player_camera, Vector3.zero, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
//		shouldTeleport(da_player);
	}


    private void addNPC(){
        while(npcs_in_game.Count < MAX_NO_ENEMIES){
          spawnNPC(Random.Range(-61,-54), Random.Range(9, 15));
		}
    }

	private void SpawnPlayer() {
		Vector3 spawnlocation = new Vector3(-58.2f, -6.4f, 0.0f);
		da_player = PhotonNetwork.Instantiate(player.name, spawnlocation, Quaternion.identity, 0);
	}


    private void spawnNPC(int x_pos, int y_pos ) {
    	int which = Random.Range(0, all_npcs.Length);
    	GameObject npc = all_npcs[which];
		Vector3 spawnlocation = new Vector3( x_pos, y_pos, 0.0f);
		GameObject spawned = PhotonNetwork.Instantiate(npc.name, spawnlocation, Quaternion.identity, 0);
		npcs_in_game.Add(++npc_id, spawned);
		spawned.GetComponent<Player>().setID(npc_id);
	}

    private void spawnItem(GameObject item, int x_pos, int y_pos){
    	string item_title = item.GetComponent<Item>().item_name;
    	Vector3 spawnlocation = new Vector3( x_pos, y_pos, 0.0f );
    	GameObject spawned = PhotonNetwork.Instantiate(item_title, spawnlocation, Quaternion.identity, 0);
    	items_in_game.Add(spawned);
    }
  
//	private void shouldTeleport(GameObject obj){
//		Vector3 result = obj.transform.position;
//		for(int i = 0; i < arena_panels.Length; ++i){
//		    if(Vector2.Distance(obj.transform.position, arena_panels[i].transform.position) <= 0.5f){
  //              result = arena_panels[(i+1)%2].transform.position + new Vector3(0.0f, 1.0f, 0.0f);
//		    }
//		}
//		if(result != obj.transform.position){
//			da_player.GetComponent<PlayerController>().teleportPlayer(result);
//		}
//	}

	public void playerDied(GameObject which){
		Transform location = which.transform;

		if(PhotonNetwork.isMasterClient){
			int player_id = which.GetComponent<Player>().player_id;
			if(which.tag == "NPC"){
				Debug.Log("NPC died.");
		        if(npcs_in_game.ContainsKey(player_id)){
				    PhotonNetwork.Destroy (which);
				    npcs_in_game.Remove(player_id);
			    }

			    addNPC();
			    int item_no = Random.Range(0, all_game_items.Length - 1);
			    spawnItem(all_game_items[item_no], (int) location.position.x, (int) location.position.y);
			} else {
                Debug.Log("Player died.");
                which.GetComponent<Player>().currentHealth = 100;   // reset health
                which.GetComponent<Player>().currentEXP -= (int) (which.GetComponent<Player>().currentEXP * 0.2f);
                Vector3 result = new Vector3(Random.Range(-61,-51), Random.Range(0, -14), 0.0f);
                which.GetComponent<PlayerController>().teleportPlayer(result);
			}

			
		}
	}

   
/*
	IEnumerator deathScene(bool initial, GameObject o) {

		if ( initial ) {
			d_a = Instantiate ( death_animation, o.transform.position, Quaternion.identity ) as GameObject;
			yield return null;
		}
	
		yield return new WaitForSeconds( 6f );

		Destroy ( d_a );
	}

	private void animateDeath(){
		if( !animation_started ){
			StartCoroutine( deathScene( true ) );
			animation_started = true;
		} else {
			StartCoroutine( deathScene( false ) );
		}
	}
	*/
}
