using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//Cammie Storey

public class DisplayPlayerStat : MonoBehaviour {

	//[SerializeField] public Actor actor;
	private GameObject player;
	[SerializeField] public Slider bar=null;
	[SerializeField] public Text label;
	[SerializeField] public string attribute;
	[SerializeField] public string attributeMax = "";
	[SerializeField] public string statName = "";
	private int value, valueMax;
	private string cache;

	// Use this for initialization
	void Start () {
		value = -1;
		valueMax = -1;

	}
	
	// Update is called once per frame
	void Update () {
		//player = GameObject.FindWithTag ("Player");
		//Actor actor = player.GetComponent<Actor> ();
		//player = GameObject.FindWithTag ("Player");
		GameObject world = GameObject.FindGameObjectWithTag("World");
		if(world){

			GameObject player = world.GetComponent<GCtrller>().da_player;
			if(player){

				if (label == null){
					Debug.Log ("No label.");
					return;
				}
					
				if(attribute != "")
					value = player.GetComponent<Player>().getAttribute(attribute);
				
				if(attributeMax != "")
					valueMax = player.GetComponent<Player>().getAttribute(attributeMax);
				
				
				
				if (bar != null && value >= 0 && valueMax >= 0)
					bar.value = ((float) value) / ((float) valueMax);
				
				cache = "";
				if (statName != "")
					cache += statName + ": ";
				
				if (label != null)
					cache += value.ToString();
				if (valueMax >= 0)
					cache += "/" + valueMax.ToString();
				
				label.text = cache;
				
				
				
			}
		}
		




	
	}
}
