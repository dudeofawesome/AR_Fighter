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
		for (int i = 0; i < GUIelements.Count; i++) {
			switch (GUIelements[i].type) {
				case HUD.GUIelement.ElementType.MESSAGE :

				break;
				case HUD.GUIelement.ElementType.OTHERTEXT :

				break;
				case HUD.GUIelement.ElementType.SCORE :

				break;
				case HUD.GUIelement.ElementType.HEALTH :
					GUIelements[i].text = GUIelements[i].relatesTo.GetComponent<CharacterController>().damage.ToString();
					// TODO the GUI element may cover other players, so we need to check for that
					GUIelements[i].rectangle.x = Camera.main.WorldToScreenPoint(GUIelements[i].relatesTo.transform.position).x - GUIelements[i].rectangle.width / 2;
					GUIelements[i].rectangle.y = Screen.height - Camera.main.WorldToScreenPoint(GUIelements[i].relatesTo.transform.position).y - 100;
					for (int j = 0; j < GUIelements.Count; j++) {
						if (GUIelements[i] != GUIelements[j] && rectangleCollision(GUIelements[i].rectangle, GUIelements[j].rectangle, 10)) {
							// decide where to push the score element to
							GUIelements[i].rectangle.x = (GUIelements[i].rectangle.x + GUIelements[i].rectangle.width / 2 > GUIelements[j].rectangle.x + GUIelements[j].rectangle.width / 2) ? GUIelements[j].rectangle.x + GUIelements[j].rectangle.width + 11 : GUIelements[j].rectangle.x - GUIelements[i].rectangle.width - 11;
							// GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.LINE, new Rect(Camera.main.WorldToScreenPoint(GUIelements[i].relatesTo.transform.position).x, Camera.main.WorldToScreenPoint(GUIelements[i].relatesTo.transform.position).y - 75, GUIelements[i].rectangle.x, GUIelements[i].rectangle.y + GUIelements[i].rectangle.height / 2)));
						}
					}
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
				case HUD.GUIelement.ElementType.LINE :
					LineDraw.Drawing.DrawLine (new Vector2 (element.rectangle.x, element.rectangle.y), new Vector2 (element.rectangle.width, element.rectangle.height), Color.blue, 2, false);
				break;
			}
		}
	}

	public void onPlayerDeath (GameObject player) {
		Instantiate(deathIndicator, player.transform.position, Quaternion.Euler(-90, Mathf.Atan2(10 - player.transform.position.y, GameObject.Find ("Ground/LedgeGrabRight").transform.position.x / 2 - player.transform.position.x), 0));
		// GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.DEATHINDICATOR, new Rect(Camera.main.WorldToScreenPoint(player.transform.position).x, Camera.main.WorldToScreenPoint(player.transform.position).y, 200, 200), player));
	}

	bool rectangleCollision(Rect r1, Rect r2, int padding) {
		return !(r1.x > r2.x + r2.width + padding || r1.x + r1.width + padding < r2.x || r1.y > r2.y + r2.height + padding || r1.y + r1.height + padding < r2.y);
		// bool widthOverlap =  (r1.xMin >= r2.xMin) && (r1.xMin <= r2.xMax) || (r2.xMin >= r1.xMin) && (r2.xMin <= r1.xMax);
		// bool heightOverlap = (r1.yMin >= r2.yMin) && (r1.yMin <= r2.yMax) || (r2.yMin >= r1.yMin) && (r2.yMin <= r1.yMax);
		// return (widthOverlap && heightOverlap);
	}
}
