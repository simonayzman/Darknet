using UnityEngine;
using System.Collections;

public class BowRegular : MonoBehaviour {
	private Item current;
	
	// Use this for initialization
	void Start () {
		current = this.GetComponent<Item>();
		if (current != null) {
			current.setConsumable( false );
			current.setDescription( "bow regular" );
			current.item_properties = new Hashtable();
			current.item_properties["attack"] = 15;
			current.item_properties["range"] = 4;
			current.item_sprite = this.GetComponent<SpriteRenderer>().sprite;
		} else {
			Debug.Log ("No such item, Bro.");
		}
	}

}
