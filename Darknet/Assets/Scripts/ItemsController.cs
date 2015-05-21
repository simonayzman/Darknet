// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

// Raymond Liang

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab;

public class ItemsController : SingletonMonoBehaviour<ItemsController> {
	// Inventory Management
	private static List<ItemInstance> Inventory;// = new List<ItemInstance>();
	public string[] InventoryItems;
	public static List<ItemInstance> InventoryConsumed;
	public static Dictionary<string, int> VirtualCurrency;
	
	public static bool InventoryLoaded = false;

	public void UpdateInventoryMenu() {
		Debug.Log("I have "+Inventory.Count+" items.");
		string[] temp = new string[Inventory.Count];
		for (int x = 0; x < Inventory.Count; x++) {
			temp[x] = Inventory[x].ItemId;
		}
		Debug.Log("temp has "+temp.Length+" items stored.");
		InventoryItems = temp;
		Debug.Log("InventoryItems has "+InventoryItems.Length+" items stored.");
	}

	// Request for update inventory information from PlayFab.
	// Assumes that there is a valid login session.
	public void UpdateInventory() {
		if (PlayFabData.AuthKey != null) {
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventory, OnPlayFabError);
		}
	}
	
	// If a valid response is returned, parse the inventory information.
	private void OnGetUserInventory(GetUserInventoryResult result) {
		Debug.Log("Getting user inventory...");
		// Store the list of ItemInstances. In other words, your inventory.
		Debug.Log("Retrieved "+result.Inventory.Count+" items from PlayFab.");
		InventoryConsumed = Inventory = result.Inventory;
		Debug.Log("Stored "+Inventory.Count+" items from PlayFab.");
		// Store the virutal currency.
		VirtualCurrency = result.VirtualCurrency;
		
		// For storage of consumable items information in PlayFabGameBridge class.
		PlayFabGameBridge.consumableItems = new Dictionary<string, int?>();
		// For storage of consumption of items in PlayFabGameBridge class.
		PlayFabGameBridge.consumableItemsConsumed = new Dictionary<string, int?>();
		// Go through inventory and populate.
		for (int i = 0; i < Inventory.Count; i++) {
			// Consumable items population and storage in PlayFabGameBridge container
			if (Inventory[i].RemainingUses != null) {
				Debug.Log("Adding " + Inventory[i].RemainingUses + " of class " + Inventory[i].ItemClass);
				if (PlayFabGameBridge.consumableItems.ContainsKey(Inventory[i].ItemClass)) {
					PlayFabGameBridge.consumableItems[Inventory[i].ItemClass] += Inventory[i].RemainingUses;
				}
				else {
					PlayFabGameBridge.consumableItems.Add(Inventory[i].ItemClass, Inventory[i].RemainingUses);
					PlayFabGameBridge.consumableItemsConsumed.Add(Inventory[i].ItemClass, 0);
				}
			}
		}
		UpdateInventoryMenu();
		InventoryLoaded = true;
	}
	
	// If something is purchased, update inventory to reflect changes.
	public static void OnPurchase(PurchaseItemResult result) {
		ItemsController.instance.UpdateInventory();
	}
	
	// If a trade is successful, update inventory to reflect changes.
	public static void OnTradeCompleted(bool tradeComplete) {
		ItemsController.instance.UpdateInventory();
	}
	
	public static void ConsumeItems() {
		var buffer = new List<string>(PlayFabGameBridge.consumableItemsConsumed.Keys);
		
		foreach (string item in buffer) {
			if (PlayFabGameBridge.consumableItemsConsumed[item] != 0) {
				ItemsController.instance.ConsumeCalculator(item, PlayFabGameBridge.consumableItemsConsumed[item]);
				PlayFabGameBridge.recordConsumed(item);
			}
		}
	}
	
	private void ConsumeCalculator(string ItemClass, int? toConsume) {
		ConsumeItemRequest request = new ConsumeItemRequest();
		for (int i = 0; i < Inventory.Count; i++) {
			if (Inventory[i].RemainingUses != null && Inventory[i].ItemClass == ItemClass && Inventory[i].RemainingUses != 0) {
				request.ItemInstanceId = Inventory[i].ItemInstanceId;
				if (toConsume >= Inventory[i].RemainingUses) {
					toConsume -= Inventory[i].RemainingUses;
					request.ConsumeCount = Convert.ToInt32(Inventory[i].RemainingUses);
					Inventory[i].RemainingUses = 0;
				}
				else {
					Inventory[i].RemainingUses -= toConsume;
					request.ConsumeCount = Convert.ToInt32(toConsume);
					toConsume = 0;
				}
				Debug.Log("Consuming " + toConsume + " of " + request.ItemInstanceId);
				PlayFabClientAPI.ConsumeItem(request, onConsumeCompleted, OnPlayFabError);
				if (toConsume == 0) {
					break;
				}
			}
		}
	}
	
	private void onConsumeCompleted(ConsumeItemResult result){
		Debug.Log("Consumed.");
				
	}
	
	void OnPlayFabError(PlayFabError error) {
		Debug.Log("Got an error: " + error.ErrorMessage);
	}
	
}