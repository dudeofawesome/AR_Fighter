using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	private List<HUD.GUIelement> GUIelements = new List<HUD.GUIelement>();

	public GUISkin guiSkin;
	public GameObject deathIndicator = null;

	public bool paused = false;


	// Use this for initialization
	void Start () {
		GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.HEALTH, new Rect(Screen.width - 100, 0, 100, 50), GameObject.Find("Player1")));
		GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.HEALTH, new Rect(0, 0, 100, 50), GameObject.Find("Player2")));
	}
	
	// Update is called once per frame
	void Update () {
		foreach (HUD.GUIelement element in GUIelements) {
			switch (element.type) {
				case HUD.GUIelement.ElementType.MESSAGE :

				break;
				case HUD.GUIelement.ElementType.OTHERTEXT :

				break;
				case HUD.GUIelement.ElementType.SCORE :

				break;
				case HUD.GUIelement.ElementType.HEALTH :
					element.text = element.relatesTo.GetComponent<CharacterController>().damage.ToString();
					// TODO the GUI element may cover other players, so we need to check for that
					element.rectangle.x = Camera.main.WorldToScreenPoint(element.relatesTo.transform.position).x - element.rectangle.width / 2;
					element.rectangle.y = Screen.height - Camera.main.WorldToScreenPoint(element.relatesTo.transform.position).y - 100;
				break;
				case HUD.GUIelement.ElementType.LIVES :

				break;
				case HUD.GUIelement.ElementType.POSITIONINDICATOR :

				break;
			}
		}

		// Global keys
		if (Input.GetKeyDown(KeyCode.Escape)) {
			paused = !paused;
			Time.timeScale = !paused ? 1 : 0;
		}
	}

	void FixedUpdate () {
		for (int i = 0; i < GUIelements.Count; i++) {
			if (GUIelements[i].displayTime > 0) {
				GUIelements[i].displayTime--;
			}
			else if (GUIelements[i].displayTime != -1) {
				GUIelements.RemoveAt(i);
			}
		}
	}

	void OnGUI () {
		GUI.skin = guiSkin;
		
		foreach	(HUD.GUIelement element in GUIelements) {
			switch (element.type) {
				case HUD.GUIelement.ElementType.MESSAGE :

				break;
				case HUD.GUIelement.ElementType.OTHERTEXT :

				break;
				case HUD.GUIelement.ElementType.SCORE :

				break;
				case HUD.GUIelement.ElementType.HEALTH :
					GUI.Box(element.rectangle, element.text);
				break;
				case HUD.GUIelement.ElementType.LIVES :

				break;
				case HUD.GUIelement.ElementType.POSITIONINDICATOR :

				break;
			}
		}
	}

	public void onPlayerDeath (GameObject player) {
		Instantiate(deathIndicator, player.transform.position, Quaternion.Euler(-90, Mathf.Atan2(10 - player.transform.position.y, GameObject.Find ("Ground/LedgeGrabRight").transform.position.x / 2 - player.transform.position.x), 0));
		// GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.DEATHINDICATOR, new Rect(Camera.main.WorldToScreenPoint(player.transform.position).x, Camera.main.WorldToScreenPoint(player.transform.position).y, 200, 200), player));
	}
}
