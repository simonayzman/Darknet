using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//Cammie Storey

public class ExpBarLabelLevel : MonoBehaviour {

	[SerializeField] public Text label;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject world = GameObject.FindGameObjectWithTag("World");
        if(world){
        	//Debug.Log("Found world.");
        	GameObject player = world.GetComponent<GCtrller>().da_player;
        	if(player && label){
        		label.text = "Level " + player.GetComponent<Player>().getAttribute ("lv").ToString();
       		}
       	}

	}
}
