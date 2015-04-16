using UnityEngine;
using System.Collections;

public class ArmorUpgraded : MonoBehaviour {
	private Item current;
	
	// Use this for initialization
	void Start () {
		current = this.GetComponent<Item>();
		if (current != null) {
			current.setConsumable( false );
			current.setDescription( "armor upgraded" );
			current.item_properties = new Hashtable();
			current.item_properties["defense"] = 40;
			current.item_sprite = this.GetComponent<SpriteRenderer>().sprite;
		} else {
			Debug.Log ("No such item, Bro.");
		}
	}

}
