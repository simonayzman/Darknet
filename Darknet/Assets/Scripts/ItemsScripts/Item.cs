using UnityEngine;
using System.Collections;
using PlayFab;

public class Item : Photon.MonoBehaviour {

    public string item_name;
	public Hashtable item_properties;
	private bool consumable;
	private string description;
	private bool is_consumed;
	public Sprite item_sprite;
	public bool is_picked;

	void Start () {
        is_picked = false;
	}
    
	public Hashtable Consume(){
		if (consumable) {
			is_consumed = true;
			return item_properties;
		} else
			return new Hashtable ();
	}

    // Private variables
	// Sync variables are used to reduce latency issue betwee instances due to send rate. Thus use interpolation.
	// These variables are also used for prediction to have smoother movement.
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector2 syncStartPosition = Vector2.zero;
	private Vector2 syncEndPosition = Vector2.zero;
	// void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		// Store current position.
		syncPosition = gameObject.transform.position;
		
		// If sending data
		if (stream.isWriting) {
			// stream.Serialize allows for variable to be serialized and sent to other clients.
			// Order of variables streamed should be the same when sending and receiving to avoid value mixups.
			stream.Serialize(ref syncPosition);
			
		}
		// If receiving data
		else {
			// stream.Serialize allows for variables to be serialized and received by clients.
			stream.Serialize(ref syncPosition);
			
			// Synchronization adjustment.
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			// Store start position for future calls.
			syncStartPosition = syncPosition + syncVelocity * syncDelay;
			// Store data locally
			syncEndPosition = syncPosition;
		}
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
