using UnityEngine;
using System.Collections;

public class PotionRed : MonoBehaviour {
	private Item current;

	// Use this for initialization
	void Start () {
		current = this.GetComponent<Item>();
		if (current != null) {
			current.setConsumable( true );
			current.setDescription( "potion red" );
			current.item_properties = new Hashtable();
			current.item_properties["health"] = 50;
			current.item_sprite = this.GetComponent<SpriteRenderer>().sprite;
		} else {
			Debug.Log ("No such item, Bro.");
		}
	}

}
