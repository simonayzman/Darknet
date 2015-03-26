using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour {

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
	public int level = 1;
	public string actor_name = "Pedro";
	public int[] xp_level = new int[4];
    public List<GameObject> inventory;
	private bool is_alive;
	private int inventory_capacity;

	
	public void updateXP( int delta_value ){
		exp += delta_value;
		adjustXPLevel();
		return;
	}

	private void adjustXPLevel(){
		for(int i = 0; i < xp_level.Length; ++i){
		    if( exp < xp_level[i] ){
				level = i;	
				return;
			}
		}
	}

	public void updateHealth( int delta_value ){
		hp += delta_value;
		if( isDead() ){
			// get some dying scene
			die ();
		}
	}

	public bool isDead() {
		return hp <= 0;
	}

	public void die() {
		Debug.Log (" I died bro ");
		is_alive = false;
	}

	public void talkTo(GameObject other){
		other.GetComponent<Actor>().Speak("Yo!");
	}

	public void Speak(string line){
		Debug.Log (line);
	}

	public void Fight(GameObject other){
		other.GetComponent<Actor>().updateHealth (-250);
	}

	public GameObject getTopItem(){
		if(inventory.Count > 0 ){
		    return inventory[0];
		}
		return new GameObject();
	}

	public void displayInventory(GameController g_ctrl = null){

		for (int i = 0; i < inventory_capacity; ++i) {
			if (g_ctrl != null) {
				if (i > inventory.Count - 1) {
					g_ctrl.setInventoryImage (null, i);
				} else {
					g_ctrl.setInventoryImage (inventory [i].GetComponent<Item> ().getItemSprite (), i);
				}
			}
		}

	}

	public void pickUpItem(GameObject item, GameController g_ctrl = null ){
		inventory.Add( item );
		item.SetActive( false );
		displayInventory ( g_ctrl );
	}
	
	public void DropTopItem(Vector2 position, GameController g_ctrl = null){
		if ( inventory.Count > 0 ) {
			GameObject gameItem = inventory [0];
			inventory.RemoveAt (0);
			gameItem.transform.position = position;
			gameItem.SetActive (true);
			displayInventory( g_ctrl );
		} else {
			Debug.Log ("You don't have nothin' in hands Bro.");
		}
	}

    public void setCapacity( int c ){
		inventory_capacity = c;
	}

}
