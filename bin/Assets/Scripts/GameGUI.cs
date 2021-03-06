using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class GameGUI : MonoBehaviour {
	public List<HUD.GUIelement> GUIelements = new List<HUD.GUIelement>();
	public GameObject myPlayer = null;
	public GUISkin guiSkin,inGameSkin,pauseSkin;
	public bool paused = false;
	
	private float originalWidth = 800;
	private float originalHeight = 480;
	private Vector3 scale;

	public float alphaFadeValue = 1;
	public Texture loadingTexture = null;

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

		HOTween.Kill();
		HOTween.To(this, 1, "alphaFadeValue", 0f);

		SetAudioLevels ();
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

		if (alphaFadeValue > 0 && Time.frameCount > 120) {
			alphaFadeValue = 0;
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
			GUI.skin = pauseSkin; 
			GUI.Box (new Rect (150, 100, 500, 72), "Paused");
			
			if (GUI.Button (new Rect (150, 165, 500, 55), "Resume")) {
				menuPosition = MenuState.MAIN;
				if (!PhotonNetwork.offlineMode) 
					GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().photonView.RPC("netPause", PhotonTargets.All, !paused);
				else
					GameObject.Find ("Multiplayer").GetComponent<MultiplayerSyncer>().netPause(!paused);
			}
			else if (GUI.Button (new Rect (150, 215, 500, 55), "Control Settings")) {
				menuPosition = MenuState.CONTROLSETTINGS;
				
			} else if (GUI.Button (new Rect (150, 265, 500, 55), "Visual Effects")) {
				menuPosition = MenuState.VISUALEFFECTS;
				
			} else if (GUI.Button (new Rect (150, 315, 500, 55), "Disconnect")) {
				PhotonNetwork.LeaveRoom();
				Application.LoadLevel("MainMenu");
			}
			break;
			
		case MenuState.CONTROLSETTINGS:
			GUI.skin = pauseSkin;
			GUI.Box (new Rect (150, 100, 500, 72), "Control Setting");
			//GUILayout.BeginArea (new Rect (200, 170, 400, 500));
			if (GUI.Button (new Rect (150, 165, 500, 55), "Full Tilt"))
				PlayerPrefs.SetInt ("controlScheme", 0);
			if (GUI.Button (new Rect (150, 215, 500, 55),"Tilt with buttons"))
				PlayerPrefs.SetInt ("controlScheme", 1);
			if (GUI.Button (new Rect (150, 265, 500, 55),"On screen buttons"))
				PlayerPrefs.SetInt ("controlScheme", 2);
			if(GUI.Button (new Rect (150, 315, 500, 55),"Back"))
				menuPosition = MenuState.PAUSE;
			//GUILayout.EndArea ();
			
			
			break;
		case MenuState.VISUALEFFECTS:
			GUI.skin = pauseSkin;
			GUI.Box (new Rect (150, 10, 500, 72), "Visual Effects");;
			
			var names = QualitySettings.names;
			int pausey = 75;
			//GUILayout.BeginArea (new Rect (200, 80, 400, 500));
			for (var i = 0; i < names.Length; i++) {
				if (GUI.Button (new Rect(150, pausey, 500, 55),names [i]))
					QualitySettings.SetQualityLevel (i, true);
				pausey += 50;
			}
			if(GUI.Button (new Rect (150, 375, 500, 55),"Back"))
				menuPosition = MenuState.PAUSE;
			//GUILayout.EndArea ();
			
			
			break;
		}
		
		
		if (alphaFadeValue > 0) {
			GUI.color = new Color(0, 0, 0, alphaFadeValue);
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), loadingTexture );
			GUI.color = new Color(255, 255, 255);
		}

		GUI.matrix = svMat; // restore matrix
	}
	
	bool rectangleCollision(Rect r1, Rect r2, int padding) {
		return !(r1.x > r2.x + r2.width + padding || r1.x + r1.width + padding < r2.x || r1.y > r2.y + r2.height + padding || r1.y + r1.height + padding < r2.y);
		// bool widthOverlap =  (r1.xMin >= r2.xMin) && (r1.xMin <= r2.xMax) || (r2.xMin >= r1.xMin) && (r2.xMin <= r1.xMax);
		// bool heightOverlap = (r1.yMin >= r2.yMin) && (r1.yMin <= r2.yMax) || (r2.yMin >= r1.yMin) && (r2.yMin <= r1.yMax);
		// return (widthOverlap && heightOverlap);
	}

	void OnDestroy () {
		HOTween.Kill ();
		GUIelements.Clear ();
		Time.timeScale = 1;

		// Destroy all players
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players) {
			Destroy(player);
		}

	}

	void SetAudioLevels () {
		AudioListener.volume = (float) ((double) PlayerPrefs.GetInt ("masterVolume") / 100.0);
		GameObject.Find("Music").GetComponent<AudioSource>().volume = (float) (((double) PlayerPrefs.GetInt ("musicVolume") / 100.0) * 0.3);
	}
}