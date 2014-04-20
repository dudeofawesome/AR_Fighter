using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	private List<HUD.GUIelement> GUIelements = new List<HUD.GUIelement>();

	public GUISkin guiSkin;


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
				case HUD.GUIelement.ElementType.DEATHINDICATOR :
					
				break;
			}
		}
	}

	void FixedUpdate () {
		foreach (HUD.GUIelement element in GUIelements) {
			if (element.displayTime > 0) {
				element.displayTime--;
			}
			else if (element.displayTime != -1) {
				GUIelements.Remove(element);
			}
		}
	}

	void OnGUI () {
		GUI.skin = guiSkin;
		
		foreach	(HUD.GUIelement element in GUIelements) {
			GUI.Box(element.rectangle, element.text);
		}
	}

	public void onPlayerDeath (GameObject player) {
		GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.DEATHINDICATOR, new Rect(Camera.main.WorldToScreenPoint(player.transform.position).x, Camera.main.WorldToScreenPoint(player.transform.position).y, 200, 200), player));
	}
}
