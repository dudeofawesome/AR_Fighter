using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class GameGUI : MonoBehaviour {
	public List<HUD.GUIelement> GUIelements = new List<HUD.GUIelement>();

	public GameObject myPlayer = null;

	public GUISkin guiSkin;

	public bool paused = false;


	// Use this for initialization
	void Start () {
		HOTween.Init(false, false, true);
		HOTween.EnableOverwriteManager();

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players) {
			if (player.GetComponent<CharacterController>().controlMe && !player.GetComponent<CharacterController>().networkControl)
				myPlayer = player;
		}
		if (myPlayer == null)
			myPlayer = players[players.Length - 1];
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
					HOTween.To (GUIelements[i], 0.1f, "rectangle", new Rect(Camera.main.WorldToScreenPoint(GUIelements[i].relatesTo.transform.position).x - GUIelements[i].rectangle.width / 2, Screen.height - Camera.main.WorldToScreenPoint(GUIelements[i].relatesTo.transform.position).y - 100, GUIelements[i].rectangle.width, GUIelements[i].rectangle.height));
					for (int j = 0; j < GUIelements.Count; j++) {
						if (GUIelements[i] != GUIelements[j] && rectangleCollision(GUIelements[i].rectangle, GUIelements[j].rectangle, 10)) {
							// decide where to push the score element to
							HOTween.To (GUIelements[i], 0.5f, "rectangle", new Rect((GUIelements[i].rectangle.x + GUIelements[i].rectangle.width / 2 > GUIelements[j].rectangle.x + GUIelements[j].rectangle.width / 2) ? GUIelements[j].rectangle.x + GUIelements[j].rectangle.width + 11 : GUIelements[j].rectangle.x - GUIelements[i].rectangle.width - 11, GUIelements[i].rectangle.y, GUIelements[i].rectangle.width, GUIelements[i].rectangle.height));
							// GUIelements[i].rectangle.x = (GUIelements[i].rectangle.x + GUIelements[i].rectangle.width / 2 > GUIelements[j].rectangle.x + GUIelements[j].rectangle.width / 2) ? GUIelements[j].rectangle.x + GUIelements[j].rectangle.width + 11 : GUIelements[j].rectangle.x - GUIelements[i].rectangle.width - 11;
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

		GUI.skin = null;
		GUI.Label (new Rect (5, 5, 200, 20), "status: " + PhotonNetwork.connectionStateDetailed.ToString() + ((PhotonNetwork.room != null) ? " " + PhotonNetwork.room.name + " room" : ""));
		GUI.skin = guiSkin;

		int _btnSize = (int) (Screen.dpi != 0 ? Screen.dpi / 2.5 : 100);
		if (PlayerPrefs.GetInt("controlScheme") == 2) {
			if (GUI.RepeatButton (new Rect(0, Screen.height - _btnSize, _btnSize, _btnSize), "<")) {
				myPlayer.GetComponent<CharacterController>().onscreenKeyLeftDown = true;
			}
			else {
				myPlayer.GetComponent<CharacterController>().onscreenKeyLeftDown = false;
			}
			if (GUI.RepeatButton (new Rect(Screen.width - _btnSize, Screen.height - _btnSize, _btnSize, _btnSize), ">")) {
				myPlayer.GetComponent<CharacterController>().onscreenKeyRightDown = true;
			}
			else {
				myPlayer.GetComponent<CharacterController>().onscreenKeyRightDown = false;
			}
			if (GUI.Button (new Rect(0, Screen.height - (_btnSize) * 2, _btnSize, _btnSize), "*")) {
				myPlayer.GetComponent<CharacterController>().attack();
			}
			if (GUI.Button (new Rect(Screen.width - _btnSize, Screen.height - (_btnSize) * 2, _btnSize, _btnSize), "^")) {
				myPlayer.GetComponent<CharacterController>().jump();
			}
		}
	}

	bool rectangleCollision(Rect r1, Rect r2, int padding) {
		return !(r1.x > r2.x + r2.width + padding || r1.x + r1.width + padding < r2.x || r1.y > r2.y + r2.height + padding || r1.y + r1.height + padding < r2.y);
		// bool widthOverlap =  (r1.xMin >= r2.xMin) && (r1.xMin <= r2.xMax) || (r2.xMin >= r1.xMin) && (r2.xMin <= r1.xMax);
		// bool heightOverlap = (r1.yMin >= r2.yMin) && (r1.yMin <= r2.yMax) || (r2.yMin >= r1.yMin) && (r2.yMin <= r1.yMax);
		// return (widthOverlap && heightOverlap);
	}
}
