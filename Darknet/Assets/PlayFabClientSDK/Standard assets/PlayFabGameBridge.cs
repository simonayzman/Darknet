using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon {
	
}

public class Offhand {
	
}

public class Armor {
	
}

public class Potion {
	
}

public class PlayFabGameBridge : MonoBehaviour {

	// Can be used for game progress
	public static uint gameState = 2;
	
	public static Dictionary<string, Weapon> mainhandType;
	public static List<string> mainhandName;
	
	public static Dictionary<string, Offhand> offhandType;
	public static List<string> offhandName;
	
	public static Dictionary<string, Armor> armorType;
	public static List<string> armorName;
	
	public static Dictionary<string, Potion> potionType;
	public static List<string> potionName;
	
	// Hold the past and present of the item regarding the number of items
	public static Dictionary<string, int?> consumableItems = new Dictionary<string, int?>();
	public static Dictionary<string, int?> consumableItemsConsumed = new Dictionary<string, int?>();
	
	public static void consumeItem(string str) {
		consumableItems[str] -= 1;
		consumableItemsConsumed[str] += 1;
	}
	
	public static void consumeItem(string str, int? count) {
		consumableItems[str] -= count;
		consumableItemsConsumed[str] += count;
	}
	
	public static void recordConsumed(string str) {
		consumableItemsConsumed[str] = 0;
	}
}