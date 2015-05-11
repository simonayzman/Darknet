using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//Cammie Storey

public class DisplayPlayerStat : MonoBehaviour {

	//[SerializeField] public Actor actor;
	private Player player;
	[SerializeField] public Slider bar=null;
	[SerializeField] public Text label;
	[SerializeField] public string attribute;
	[SerializeField] public string attributeMax = "";
	[SerializeField] public string name = "";
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
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();

		if (player == null || label == null){
			Debug.Log ("No player/label.");
			return;
		}

		if (player != null) {


			if(attribute != "")
				value = player.GetComponent<Actor>().getAttribute(attribute);

			if(attributeMax != "")
				valueMax = player.GetComponent<Actor>().getAttribute(attributeMax);
				


			if (bar != null && value >= 0 && valueMax >= 0)
				bar.value = ((float) value) / ((float) valueMax);

			cache = "";
			if (name != "")
				cache += name + ": ";

			if (label != null)
				cache += value.ToString();
			if (valueMax >= 0)
				cache += "/" + valueMax.ToString();

			label.text = cache;


		}


	
	}
}
