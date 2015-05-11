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
		GameObject player = GameObject.FindWithTag ("Player");

		if (label != null)
			label.text = "Level " + player.GetComponent<Player>().getAttribute ("lv").ToString();

	
	}
}
