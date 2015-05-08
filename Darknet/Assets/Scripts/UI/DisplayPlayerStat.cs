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
	private int value, valueMax;

	// Use this for initialization
	void Start () {
		value = -1;
		valueMax = -1;

	}
	
	// Update is called once per frame
	void Update () {
		player = GameObject.FindWithTag ("Player");
		Actor actor = player.GetComponent<Actor> ();
		if (actor != null) {


			if(attribute != "")
				value = actor.getAttribute(attribute);

			if(attributeMax != "")
				valueMax = actor.getAttribute(attributeMax);


			if (bar != null && value >= 0 && valueMax >= 0)
				bar.value = ((float) value) / ((float) valueMax);

			label.text = value.ToString();
			if (valueMax >= 0)
				label.text += "/" + valueMax.ToString();


		}


	
	}
}
