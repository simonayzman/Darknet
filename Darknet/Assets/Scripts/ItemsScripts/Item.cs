using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {


	public Hashtable item_properties;
	private bool consumable;
	private string description;
	private bool is_consumed;
	public Sprite item_sprite;

	public Hashtable Consume(){
		if (consumable) {
			is_consumed = true;
			return item_properties;
		} else
			return new Hashtable ();
	}

	public void setConsumable(bool is_consumable){
		consumable = is_consumable;
		is_consumed = false;
	}

	public void setDescription(string desc){
		description = desc;
	}

	public string getDescription(){
		return description;
	}

	public void setItemSprite( Sprite el_sprite ){
		item_sprite = el_sprite;
	}

	public Sprite getItemSprite(){
		return item_sprite;
	}
}
