using UnityEngine;
using System.Collections;

public class SwordWood : MonoBehaviour {

	private Item current;

	// Use this for initialization
	void Start () {
		current = this.GetComponent<Item>();
		if (current != null) {
			current.consumable = false;
			current.description = "wooden sword";
			current.item_properties = new Hashtable();
			current.item_properties["attack"] = 5;
		} else {
			Debug.Log ("No such item, Bro.");
		}
	}
	

}
