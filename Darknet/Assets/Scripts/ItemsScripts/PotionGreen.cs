using UnityEngine;
using System.Collections;

public class PotionGreen : MonoBehaviour {

	private Item current;
	// Use this for initialization
	void Start () {
		current = this.GetComponent<Item>();
		if (current != null) {
			current.setConsumable( true );
			current.setDescription( "potion green" );
			current.item_properties = new Hashtable();
			current.item_properties["health"] = 200;
			current.item_sprite = this.GetComponent<SpriteRenderer>().sprite;
		} else {
			Debug.Log ("No such item, Bro.");
		}
	}
}
