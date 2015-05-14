using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GCtrller : MonoBehaviour {
	public int MAX_NO_ENEMIES;
	public GameObject player;
	public GameObject[] all_npcs; 	
	public GameObject[] all_game_items;
	private List<GameObject> npcs_in_game;
	public List<GameObject> items_in_game;
    public GameObject player_camera;
    public GameObject da_player;
    private GameObject[] arena_panels;
    public GameObject death_animation;
    public string da_name;
    
	// Use this for initialization
	void Start () {

		//PhotonNetwork.playerName = da_name;
		bool am_i = PhotonNetwork.isMasterClient;
		Debug.Log("Am I master: " + am_i);
       

		SpawnPlayer ();
        npcs_in_game = countNPC();
        
		if(PhotonNetwork.isMasterClient){
			addNPC();
		}
		
		arena_panels = GameObject.FindGameObjectsWithTag ("Panel");

        // Create camera
        Instantiate(player_camera, Vector3.zero, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		shouldTeleport(da_player);
	}


    private List<GameObject> countNPC(){
    	List<GameObject> result = new List<GameObject>();
    	GameObject[] living_npc = GameObject.FindGameObjectsWithTag ("NPC");
		for(int i = 0; i < living_npc.Length; ++i){
			result.Add(living_npc[i]);
		}
		return result;
    }

    private void addNPC(){
    	Debug.Log("Adding NPCs");
        while(npcs_in_game.Count < MAX_NO_ENEMIES){
          spawnNPC(Random.Range(-61,-54), Random.Range(9, 15));
		}
    }

	private void SpawnPlayer() {
		Debug.Log("Spawning da playa.");
		Vector3 spawnlocation = new Vector3(-58.2f, -6.4f, 0.0f);
		da_player = PhotonNetwork.Instantiate(player.name, spawnlocation, Quaternion.identity, 0);
	}


    private void spawnNPC(int x_pos, int y_pos ) {
    	int which = Random.Range(0, all_npcs.Length);
    	GameObject npc = all_npcs[which];
    	Debug.Log("Spawning " + npc.name);
		Vector3 spawnlocation = new Vector3( x_pos, y_pos, 0.0f);
		GameObject spawned = PhotonNetwork.Instantiate(npc.name, spawnlocation, Quaternion.identity, 0);
		npcs_in_game.Add(spawned);
	}

    private void spawnItem(GameObject item, int x_pos, int y_pos){
    	string item_title = item.GetComponent<Item>().item_name;
    	Debug.Log("Spawning " + item_title);
    	Vector3 spawnlocation = new Vector3( x_pos, y_pos, 0.0f );
    	GameObject spawned = PhotonNetwork.Instantiate(item_title, spawnlocation, Quaternion.identity, 0);
    	items_in_game.Add(spawned);
    }
  
	private void shouldTeleport(GameObject obj){
		Vector3 result = obj.transform.position;
		for(int i = 0; i < arena_panels.Length; ++i){
		    if(Vector2.Distance(obj.transform.position, arena_panels[i].transform.position) <= 0.5f){
                result = arena_panels[(i+1)%2].transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		    }
		}
		if(result != obj.transform.position){
			da_player.GetComponent<PlayerController>().teleportPlayer(result);
		}
	}

	public void npcDied(GameObject which){
	    
	    npcs_in_game = countNPC();
	    Transform location = which.transform;

	    int index = -1;
	    for(int i = 0; i < npcs_in_game.Count; ++i){ 
	    	if(npcs_in_game[i].GetComponent<Player>().isDead()){
	    		index = i;
	    	}
	    }
		if(index != -1){
			npcs_in_game.RemoveAt(index);
			Destroy(which);
		}
		
		if(PhotonNetwork.isMasterClient){
			addNPC();
			int item_no = Random.Range(0, all_game_items.Length - 1);
			Debug.Log("I need item: " + item_no);
			spawnItem(all_game_items[item_no], (int) location.position.x, (int) location.position.y);
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
