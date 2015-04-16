using UnityEngine;
using PlayFab;

namespace PlayFab.Examples {
	public class PlayFabLoadData : MonoBehaviour {
		// Simple script to load the PlayFabDataFile. 
		// ie : Usually put on the first scene to have everything ready upon initiation.
		void Awake() {
			Debug.Log("Loading player data...");
			PlayFabData.LoadData();
		}
	}
}
