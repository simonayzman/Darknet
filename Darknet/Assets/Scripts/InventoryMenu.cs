// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

//Raymond Liang

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMenu : ItemsController {
	// public Texture2D item1, item2, item3, item4;
	
	public bool autoUpdateConsumeItems = true;
	public int UpdateEverySeconds;
	
	private List<Texture2D> itemTextures;
	private List<Texture2D> itemSelectedTextures;
	public static Dictionary<string, GameObject> stringToImage;
	private Rect[] itemsRect;
	private int currentItemSelected = 0;
	
	public void Start() {
		// itemTextures = new List<Texture2D>(new Texture2D[] {item1, item2, item3, item4});
		// ItemsController function.
		UpdateInventory();
	}
	
	void OnGUI() {
		if (InventoryLoaded && PlayFabGameBridge.gameState == 3) {
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.I)  {
				
				// Show Inventory window
				// inventoryDisplay = GUI.Window(0, inventoryDisplay, showInventory, "My Inventory");
			}
		}
	}
	
	private void ConsumeNow() {
		ConsumeItems();
	}
	
	void Update() {
	}
}