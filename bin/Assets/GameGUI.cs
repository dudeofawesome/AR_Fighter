using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	private List<HUD.GUIelement> GUIelements = new List<HUD.GUIelement>();

	public GUISkin guiSkin;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.skin = guiSkin;
		
	}

	public void onPlayerDeath (Vector3 playerPos, Vector3 playerVector) {
		GUIelements.Add(new HUD.GUIelement(Camera.main.WorldToScreenPoint(playerPos)));
	}
}
