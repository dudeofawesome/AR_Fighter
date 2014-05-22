using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class GameGUI : MonoBehaviour {
	public List<HUD.GUIelement> GUIelements = new List<HUD.GUIelement>();
	public GameObject myPlayer = null;
	public GUISkin guiSkin;
	public bool paused = false;
	
	private float originalWidth = 800;
	private float originalHeight = 480;
	private Vector3 scale;
	
	public enum MenuState {
		MAIN,
		PAUSE,
		CONTROLSETTINGS,
		VISUALEFFECTS
	}
	
	public MenuState menuPosition = MenuState.MAIN;
	
	// Use this for initialization
	void Start () {
		HOTween.Init(false, false, true);
		HOTween.EnableOverwriteManager();

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players) {
			if (player.GetComponent<CharacterController>().controlMe && !player.GetComponent<CharacterController>().networkControl)
				myPlayer = player;
			else if (PhotonNetwork.room != null)
				GameObject.Find("GUI").GetComponent<GameGUI>().GUIelements.Add(new HUD.GUIelement(HUD.GUIelement.ElementType.HEALTH, new Rect(Screen.width - 100, 0, 100, 50), player.gameObject));
		}
		if (myPlayer == null)
			myPlayer = players[players.Length - 1];
			
		if (Application.isEditor) {
			if (!PhotonNetwork.offlineMode) 
				GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().photonView.RPC("netPause", PhotonTargets.All, !paused);
			else
				GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().netPause(!paused);
		}
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
			if (!PhotonNetwork.offlineMode) 
				GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().photonView.RPC("netPause", PhotonTargets.All, !paused);
			else
				GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().netPause(!paused);
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
			if (GUI.Button (new Rect(0, Screen.height - (_btnSize) * 2, _btnSize, _btnSize), "*") && myPlayer.GetComponent<CharacterController>().currentAttack == null) {
				if(!PhotonNetwork.offlineMode)
					myPlayer.GetComponent<CharacterController>().photonView.RPC ("attack", PhotonTargets.All);
				else
					myPlayer.GetComponent<CharacterController>().attack();
			}
			if (GUI.Button (new Rect(Screen.width - _btnSize, Screen.height - (_btnSize) * 2, _btnSize, _btnSize), "^")) {
				myPlayer.GetComponent<CharacterController>().jump();
			}
		}
		
		scale.x = Screen.width / originalWidth; // calculate hor scale
		scale.y = Screen.height / originalHeight; // calculate vert scale
		scale.z = 1;
		var svMat = GUI.matrix; // save current matrix
		//substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);
		
		
		switch (menuPosition) {
			
		case MenuState.MAIN:
			
			if (GUI.Button (new Rect (720, 5, 50, 50), "||")) {
				menuPosition = MenuState.PAUSE;
				if (!PhotonNetwork.offlineMode) 
					GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().photonView.RPC("netPause", PhotonTargets.All, !paused);
				else
					GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().netPause(!paused);
			}
			break;
		case MenuState.PAUSE:
			GUI.Box (new Rect (200, 100, 400, 70), "Paused");
			
			if (GUI.Button (new Rect (200, 170, 400, 50), "Resume")) {
				menuPosition = MenuState.MAIN;
				if (!PhotonNetwork.offlineMode) 
					GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().photonView.RPC("netPause", PhotonTargets.All, !paused);
				else
					GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().netPause(!paused);
			}
			else if (GUI.Button (new Rect (200, 224, 400, 50), "Control Settings")) {
				menuPosition = MenuState.CONTROLSETTINGS;
				
			} else if (GUI.Button (new Rect (200, 278, 400, 50), "Visual Effects")) {
				menuPosition = MenuState.VISUALEFFECTS;
				
			} else if (GUI.Button (new Rect (200, 332, 400, 50), "Disconnect")) {
				PhotonNetwork.LeaveRoom();
				Application.LoadLevel("MainMenu");
			}
			break;
			
		case MenuState.CONTROLSETTINGS:
			GUI.Box (new Rect (200, 100, 400, 70), "Control Setting");
			GUILayout.BeginArea (new Rect (200, 170, 400, 500));
			if (GUILayout.Button ("Full Tilt"))
				PlayerPrefs.SetInt ("controlScheme", 0);
			if (GUILayout.Button ("Tilt with buttons"))
				PlayerPrefs.SetInt ("controlScheme", 1);
			if (GUILayout.Button ("On screen buttons"))
				PlayerPrefs.SetInt ("controlScheme", 2);
			if(GUILayout.Button ("Back"))
				menuPosition = MenuState.PAUSE;
			GUILayout.EndArea ();
			
			
			break;
		case MenuState.VISUALEFFECTS:
			GUI.Box (new Rect (200, 10, 400, 70), "Visual Effects");
			
			var names = QualitySettings.names;
			
			GUILayout.BeginArea (new Rect (200, 80, 400, 500));
			for (var i = 0; i < names.Length; i++) {
				if (GUILayout.Button (names [i]))
					QualitySettings.SetQualityLevel (i, true);
			}
			if(GUILayout.Button ("Back"))
				menuPosition = MenuState.PAUSE;
			GUILayout.EndArea ();
			
			
			break;
		}
		
		GUI.matrix = svMat; // restore matrix
	}
	
	bool rectangleCollision(Rect r1, Rect r2, int padding) {
		return !(r1.x > r2.x + r2.width + padding || r1.x + r1.width + padding < r2.x || r1.y > r2.y + r2.height + padding || r1.y + r1.height + padding < r2.y);
		// bool widthOverlap =  (r1.xMin >= r2.xMin) && (r1.xMin <= r2.xMax) || (r2.xMin >= r1.xMin) && (r2.xMin <= r1.xMax);
		// bool heightOverlap = (r1.yMin >= r2.yMin) && (r1.yMin <= r2.yMax) || (r2.yMin >= r1.yMin) && (r2.yMin <= r1.yMax);
		// return (widthOverlap && heightOverlap);
	}
}