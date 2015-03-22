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
			Debug.Log (" You died bro ");
			// get some dying scene
		}
	}

	public bool isDead() {
		return hp <= 0;
	}
}
