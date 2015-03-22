using UnityEngine;
using System.Collections;

public class SwordRegular : MonoBehaviour {

	private Item current;

	// Use this for initialization
	void Start () {
		current = this.GetComponent<Item>();
		if (current != null) {
			current.consumable = false;
			current.description = "regular sword";
			current.item_properties = new Hashtable();
			current.item_properties["attack"] = 25;
		} else {
			Debug.Log ("No such item, Bro.");
		}
	}
	

}
