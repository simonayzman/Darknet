using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Serialization.JsonFx;

public class IconClass {
	public string iconName;
}

public class StoreMenu : MonoBehaviour {
	public Texture2D marketIcon, marketMenu, closeIcon;
	
	public int titleSize;
	public int textSize;
	public int priceTextSize;
	public string[] ItemClasses;
	
	public bool showMenu;
	
	public bool listEquipments = false;
	public bool listPotions = false;
	
	private List<CatalogItem> items;
	private Dictionary<string, Texture2D> icons;
	private Dictionary<string, Texture2D> itemIcons;
	
	private bool renderCatalog = false;
	
	private void Start() {
		// Get specified version of title's catalog of virtual goods.
		icons = new Dictionary<string, Texture2D>();
		// Add icons here.
		// icons.Add("", "");
		GetCatalogItemsRequest request = new GetCatalogItemsRequest();
		request.CatalogVersion = PlayFabData.CatalogVersion;
		if (PlayFabData.AuthKey != null) {
			PlayFabClientAPI.GetCatalogItems(request, ConstructCatalog, OnPlayFabError);
		}
	}
	
	void OnGUI() {
		// If the catalog has already been constructed, check if menu should be displayed.
		if (renderCatalog) {
			Rect marketIconRect = new Rect(1,1,1,1);
			// Pause the game and show menu.
			if (GUI.Button(marketIconRect, marketIcon, GUIStyle.none)) {
				showMenu = !showMenu;
				Time.timeScale = !showMenu ? 1.0f : 0.0f;
			}
			if (showMenu) {
				// Area for market window.
				Rect winRect = new Rect((Screen.width - marketMenu.width) * 0.5f, (Screen.height - marketMenu.height) * 0.5f, marketMenu.width, marketMenu.height);
				GUI.DrawTexture(winRect, marketMenu);
				
				Rect closeRect = new Rect(winRect.x+winRect.width-10, winRect.y, 10, 10);
				if (GUI.Button(closeRect, closeIcon, GUIStyle.none)) {
					showMenu = false;
					Time.timeScale = !showMenu ? 1.0f : 0.0f;
				}
				
				int btnWidth = 50;
				int btnHeight = 50;
				
				// Create buttons for each item in the list.
				for (int x = 0; x < items.Count; x++) {
					Texture2D texture = itemIcons[items[x].ItemId];
					Rect btnIconRect = new Rect(winRect.x+50, winRect.y+50, 50, 50);
					foreach (KeyValuePair<string, uint> price in items[x].VirtualCurrencyPrices) {
						// Create a button that creates a purchase request if pressed.
						if (GUI.Button(btnIconRect, "" )) {
							PurchaseItemRequest request = new PurchaseItemRequest();
							request.CatalogVersion = items[x].CatalogVersion;
							request.VirtualCurrency = price.Key;
							request.Price = Convert.ToInt32(price.Value);
							request.ItemId = items[x].ItemId;
							PlayFabClientAPI.PurchaseItem(request, ItemsController.OnPurchase, OnPlayFabError);
						}
					}
				}
			}
		}
	}
	
	private void ConstructCatalog(GetCatalogItemsResult result) {
		items = result.Catalog;
		renderCatalog = true;
		itemIcons = new Dictionary<string, Texture2D>();
		for (int x = 0; x < items.Count; x++) {
			Dictionary<string, string> customData = JsonReader.Deserialize<Dictionary<string, string>>(items[x].CustomData);
			itemIcons.Add(items[x].ItemId, icons[customData["Icon"]]);
			
			if (listEquipments && items[x].ItemClass.Equals("Mainhand") && !PlayFabGameBridge.mainhandType.ContainsKey(items[x].ItemId)) {
				// Use unique id of weapon to add new item.
				// float.Parse(customData["Frequency"])
				string newMainhandName = items[x].ItemId;
				Weapon newMainhand = new Weapon{/*Parse custom data here*/};
				PlayFabGameBridge.mainhandName.Add(newMainhandName);
				PlayFabGameBridge.mainhandType.Add(newMainhandName, newMainhand);
			}
			if (listEquipments && items[x].ItemClass.Equals("Offhand") && !PlayFabGameBridge.offhandType.ContainsKey(items[x].ItemId)) {
				// Use unique id of offhand to add new item.
				string newOffhandName = items[x].ItemId;
				Offhand newOffhand = new Offhand{/*Parse custom data here*/};
				PlayFabGameBridge.offhandName.Add(newOffhandName);
				PlayFabGameBridge.offhandType.Add(newOffhandName, newOffhand);
			}
			
			if (listEquipments && items[x].ItemClass.Equals("Armor") && !PlayFabGameBridge.armorType.ContainsKey(items[x].ItemId)) {
				// Use unique id of armor to add new item.
				string newArmorName = items[x].ItemId;
				Armor newArmor = new Armor{/*Parse custom data here*/};
				PlayFabGameBridge.armorName.Add(newArmorName);
				PlayFabGameBridge.armorType.Add(newArmorName, newArmor);
			}
			
			if (listPotions && items[x].ItemClass.Equals("Potions") && !PlayFabGameBridge.potionType.ContainsKey(items[x].ItemId)) {
				// Use unique id of potion to add new item.
				string newPotionName = items[x].ItemId;
				Potion newPotion = new Potion{/*Parse custom data here*/};
				PlayFabGameBridge.potionName.Add(newPotionName);
				PlayFabGameBridge.potionType.Add(newPotionName, newPotion);
			}
		}
		Time.timeScale = 1;
	}
	
	void OnPlayFabError(PlayFabError error) {
		Debug.Log("Got an error: " + error.ErrorMessage);
	}
	
}