// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

// Cammie Storey

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class DetectKeys : MonoBehaviour {
	[SerializeField] public string slotName;
	[SerializeField] public UISpellSlot parentSlot;

	// Use this for initialization
	void Start (){
	
	}
	
	// Update is called once per frame
	void Update () {
		if (parentSlot != null && Input.GetAxisRaw (slotName) > 0.0) {
			Debug.Log ("Casting " + slotName);
			parentSlot.SpellCast ();
		}

	
	}
}
