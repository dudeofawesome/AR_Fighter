using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class MainMenuGUI : MonoBehaviour
{
	
	public GUISkin guiSkin, verticalSkin, scrollSkin, howtoplaySkin, inGameSkin;
	public string mainLevelDojo;
	public string mainLevelTurret;

	public enum MenuState
	{
		MAIN,
		PLAYERS,
		SERVER,
		LEVELLOADER,
		SETTINGS,
		SETTINGSVI,
		SETTINGSCONTROLS, 
		HOWTOPLAY,
		HOWTODOWNLOAD,
		HOWTOPRINT,
		HOWTOSETUP,
		HOWTOUSE,
		LOADING}
	;
	
	public MenuState menuPosition = MenuState.MAIN;
	private float originalWidth = 800;
	private float originalHeight = 480;
	private Vector3 scale;
	private Vector2 scrollPosition = Vector2.zero;
	private Vector2 scrollPositionServer = Vector2.zero;
	private bool onLoading = false;
	public Texture loadingTexture = null;
	public float angle = 0;
	public Vector2 size = new Vector2 (0, 0);
	Vector2 pos = new Vector2 (0, 0);
	Rect rect;
	Vector2 pivot;
	private int tweenSwitch = 0;
	public AsyncOperation op = null;
	public float alphaFadeValue = 0;
	
	// Use this for initialization
	void Start ()
	{
		
		HOTween.Kill ();
		
		UpdateSettings ();
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		//2 and a half seconds
		HOTween.Init (false, false, false);
		HOTween.DisableOverwriteManager ();
		
		//Application.LoadLevel (mainLevel);
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (Touch touch in Input.touches) {
			bool fInsideList = IsTouchInsideList (touch.position);
			
			if (touch.phase == TouchPhase.Moved) {
				// dragging
				scrollPosition.y += touch.deltaPosition.y;
			}
		}
	}
	
	void OnGUI ()
	{
		GUI.skin = guiSkin;
		
		scale.x = Screen.width / originalWidth; // calculate hor scale
		scale.y = Screen.height / originalHeight; // calculate vert scale
		scale.z = 1;
		var svMat = GUI.matrix; // save current matrix
		// substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);
		// draw your GUI controls here:
		
		switch (menuPosition) {
			
		case MenuState.MAIN:
			
			//GUI.Box (new Rect(0, 0, 800, 480), "");
			if (!Application.genuine) {
				GUI.Box (new Rect (0, 410, 430, 70), "PLEASE BUY");
			}
			
			
			GUI.Label (new Rect (30, 75, 325, 325), "");
			//PlayerPrefs.SetString ("name",GUI.TextField (new Rect(100, 200, 100, 50), PlayerPrefs.GetString("name")));
			
			GUI.Box(new Rect(380,100,400,72), "Sole Fighter");
			
			
			if (GUI.Button (new Rect (380, 165, 400, 55), "Start")) {
				
				menuPosition = MenuState.PLAYERS;
				
				
			} else if (GUI.Button (new Rect (380, 215, 400, 55), "Settings")) {
				menuPosition = MenuState.SETTINGS;
				
				
			} else if (GUI.Button (new Rect (380, 265, 400, 55), "How To Play")) {
				menuPosition = MenuState.HOWTOPLAY;
				
				
			} else if (GUI.Button (new Rect (380, 315, 400, 55), "Exit")) {
				//QUIT GAME
				Application.Quit ();
			}
			
			
			
			break;
			
		case MenuState.PLAYERS:
			GUI.Label (new Rect (30, 75, 325, 325), "");
			GUI.Box(new Rect(380,100,400,72), "Players");
			if (GUI.Button (new Rect (380, 165, 400, 55), "Single Player")) {
				
				menuPosition = MenuState.LEVELLOADER;
				
				
			} else if (GUI.Button (new Rect (380, 215, 400, 55), "Multiplayer")) {
				menuPosition = MenuState.SERVER;
			}
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			}
			break;
			
		case MenuState.SERVER:
			GUI.skin = null;
			
			if (PhotonNetwork.connected && PhotonNetwork.room == null) {
				RoomInfo[] _rooms = PhotonNetwork.GetRoomList ();
				int i = 0;
				for (i = 0; i < _rooms.Length; i++) {
					if (GUI.Button (new Rect (Screen.width / 2 - 100, 10 + i * 30, 200, 25), _rooms [i].name + " " + _rooms [i].playerCount + "/" + _rooms [i].maxPlayers)) {
						GameObject.Find ("SessionStarter").GetComponent<SessionStarter> ().roomHost = false;
						PhotonNetwork.JoinRoom (_rooms [i].name);
					}
				}
				if (GUI.Button (new Rect (Screen.width / 2 - 100, 10 + i * 30, 200, 25), "Create new room")) {
					print ("created room with name " + System.Environment.UserName);
					PhotonNetwork.CreateRoom (System.Environment.UserName + "'s Room", new RoomOptions () { maxPlayers = 2 }, null);
					menuPosition = MenuState.LEVELLOADER;
				}
			}
			GUI.skin = guiSkin;
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			}
			
			GUI.skin = null;
			GUI.Label (new Rect (5, 5, 200, 20), "status: " + PhotonNetwork.connectionStateDetailed.ToString() + ((PhotonNetwork.room != null) ? " " + PhotonNetwork.room.name + " room" : ""));
			GUI.skin = guiSkin;
			
			break;
			
		case MenuState.SETTINGS:
			GUI.Label (new Rect (30, 75, 325, 325), "");

			GUI.Box(new Rect(380,100,400,72), "Settings");
			if (GUI.Button (new Rect (380, 165, 400, 55), "Visuals")) {
				menuPosition = MenuState.SETTINGSVI;
				
				
			} else if (GUI.Button (new Rect (380, 215, 400, 55), "Controls")) {
				menuPosition = MenuState.SETTINGSCONTROLS;
				
				
			}
			
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			}
			
			
			
			break;
			
		case MenuState.SETTINGSVI:
			//GUI.skin = verticalSkin;
			
			GUI.Box (new Rect (100, 10, 600, 72), "Visual Effect Setting");
	
			var names = QualitySettings.names;
			int pausey = 75;
			
			//GUILayout.BeginArea (new Rect (120, 100, 680, 600));
			for (var i = 0; i < names.Length; i++) {
				if (GUI.Button (new Rect(100, pausey, 600, 55),names [i]))
					QualitySettings.SetQualityLevel (i, true);
				pausey += 50;
			}
			//GUILayout.EndArea ();
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.SETTINGS;
			}
			
			
			
			break;
			
		case MenuState.SETTINGSCONTROLS:
			
			//GUILayout.BeginArea (new Rect (120, 100, 680, 600));
			//GUILayout.EndArea ();
			
			GUI.Box (new Rect (100, 100, 600, 72), "Control Setting");
			//GUILayout.BeginArea (new Rect (200, 170, 400, 500));
			if (GUI.Button (new Rect (100, 165, 600, 55), "Full Tilt"))
				PlayerPrefs.SetInt ("controlScheme", 0);
			if (GUI.Button (new Rect (100, 215, 600, 55),"Tilt with buttons"))
				PlayerPrefs.SetInt ("controlScheme", 1);
			if (GUI.Button (new Rect (100, 265, 600, 55),"On screen buttons"))
				PlayerPrefs.SetInt ("controlScheme", 2);
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.SETTINGS;
			}
			
			
			break;
			
		case MenuState.LEVELLOADER:
			GUI.skin = scrollSkin;
			
			GUI.Box (new Rect (50, 20, 500, 70), "Select Visual Style");
			GUI.Box (new Rect (30, 90, 770, 390), "");
			
			
			
			scrollPosition = GUI.BeginScrollView (new Rect (430, 100, 370, 330),
			                                      scrollPosition, new Rect (0, 0, 340, 420));
			
			// Make four buttons - one in each corner. The coordinate system is defined
			// by the last parameter to BeginScrollView.
			if (GUI.Button (new Rect (0, 0, 340, 70), "Dojo in the Trees")) {
				if (PhotonNetwork.room != null) {
					ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable ();
					ht.Add ("gameLevel", "treeTower");
					PhotonNetwork.room.SetCustomProperties (ht);
					GameObject.Find ("SessionStarter").GetComponent<SessionStarter> ().singlePlayer = false;
				}
				else {
					PhotonNetwork.Disconnect();
					PhotonNetwork.offlineMode = true;
				}
				menuPosition = MenuState.LOADING;
				op = null;
				op = Application.LoadLevelAsync (mainLevelDojo);
				op.allowSceneActivation = false;
				GameObject.Find("Map").GetComponent<RotateAroundByAccel>().enabled = false;
				HOTween.Kill();
				HOTween.To(GameObject.Find ("Map").transform, 4, new TweenParms().Prop("rotation", new Vector3(0,360,0), true).UpdateType(UpdateType.TimeScaleIndependentUpdate).OnComplete(actuallyLoadLevel));
				HOTween.To(GameObject.Find ("Main Camera").transform, 4, "position", GameObject.Find ("Map/dojo_in_tree/TweenTo").transform.position);
				HOTween.To(GameObject.Find ("Main Camera").transform, 4, "rotation", Quaternion.Euler(0, -40, 0));
				HOTween.To(this, 0.5f, "alphaFadeValue", 1, false, EaseType.EaseInBack, 3.5f);
			}
			if (GUI.Button (new Rect (0, 70, 340, 70), "Castle Turret")) {
				if (PhotonNetwork.room != null) {
					ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable ();
					ht.Add ("gameLevel", "castleTurret");
					PhotonNetwork.room.SetCustomProperties (ht);
					GameObject.Find ("SessionStarter").GetComponent<SessionStarter> ().singlePlayer = false;
				}
				else {
					PhotonNetwork.Disconnect();
					PhotonNetwork.offlineMode = true;
				}
				menuPosition = MenuState.LOADING;
				op = null;
				op = Application.LoadLevelAsync (mainLevelTurret);
				op.allowSceneActivation = false;
				GameObject.Find("Map").GetComponent<RotateAroundByAccel>().enabled = false;
				HOTween.Kill();
				HOTween.To(GameObject.Find ("Map").transform, 4, new TweenParms().Prop("rotation", new Vector3(0,360,0), true).UpdateType(UpdateType.TimeScaleIndependentUpdate).OnComplete(actuallyLoadLevel));
				HOTween.To(GameObject.Find ("Main Camera").transform, 4, "position", GameObject.Find ("Map/castle_turret/TweenTo").transform.position);
				HOTween.To(GameObject.Find ("Main Camera").transform, 4, "rotation", Quaternion.Euler(0, -40, 0));
				HOTween.To(this, 0.5f, "alphaFadeValue", 1, false, EaseType.EaseInBack, 3.5f);
			}
			GUI.Button (new Rect (0, 140, 340, 70), "Setting3");
			if (GUI.Button (new Rect (0, 210, 340, 70), "Setting4")) {
				menuPosition = MenuState.MAIN;
			}
			GUI.Button (new Rect (0, 280, 340, 70), "Setting5");
			GUI.Button (new Rect (0, 350, 340, 70), "Setting6");
			// End the scroll view that we began above.
			GUI.EndScrollView ();
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.PLAYERS;
			} 
			//else if (GUI.Button (new Rect (200, 400, 150, 70), "Start!")) {
			//	menuPosition = MenuState.LOADING;
				//Application.LoadLevel (mainLevel);
			//}
			
			GUI.skin = null;
			GUI.Label (new Rect (5, 5, 200, 20), "status: " + PhotonNetwork.connectionStateDetailed.ToString() + ((PhotonNetwork.room != null) ? " " + PhotonNetwork.room.name + " room" : ""));
			GUI.skin = guiSkin;
			
			break;
		case MenuState.HOWTOPLAY:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "Introduction");
			GUI.Label (new Rect (0, 50, 800, 450), "");
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (630, 400, 150, 70), "Next")) {
				menuPosition = MenuState.HOWTODOWNLOAD;
			}
			break;
			
			
		case MenuState.HOWTODOWNLOAD:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "How to Download");
			GUI.Label (new Rect (0, 50, 800, 450), "");
			if (GUI.Button (new Rect (315, 400, 150, 70), "Main")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.HOWTOPLAY;
			} else if (GUI.Button (new Rect (630, 400, 150, 70), "Next")) {
				menuPosition = MenuState.HOWTOPRINT;
			}
			
			break;
			
		case MenuState.HOWTOPRINT:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "How to Print");
			GUI.Label (new Rect (0, 50, 800, 450), "");
			GUI.skin = howtoplaySkin;
			if (GUI.Button (new Rect (315, 400, 150, 70), "Main")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.HOWTODOWNLOAD;
			} else if (GUI.Button (new Rect (630, 400, 150, 70), "Next")) {
				menuPosition = MenuState.HOWTOSETUP;
			}
			break;
			
		case MenuState.HOWTOSETUP:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "How to Set Up");
			GUI.Label (new Rect (0, 50, 800, 450), "");
			if (GUI.Button (new Rect (315, 400, 150, 70), "Main")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.HOWTOPRINT;
			} else if (GUI.Button (new Rect (630, 400, 150, 70), "Next")) {
				menuPosition = MenuState.HOWTOUSE;
			}
			break;
			
		case MenuState.HOWTOUSE:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "How to Use");
			GUI.Label (new Rect (0, 50, 800, 450), "");
			if (GUI.Button (new Rect (315, 400, 150, 70), "Main")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.HOWTOSETUP;
			}
			
			
			break;
			
		case MenuState.LOADING:
			GUI.color = new Color(0, 0, 0, alphaFadeValue);
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), loadingTexture );
			
			break;
		}
		
		
		
		//...
		// restore matrix before returning
		GUI.matrix = svMat; // restore matrix
		
		
		
		
		
		
		
		
	}
	
	void UpdateSettings ()
	{
		//pos = new Vector2 (350, 150);
		rect = new Rect (350, 150, 150, 150);
		//pivot = new Vector2 (258, 150);
		pivot = new Vector2 (Screen.width / 2, Screen.height / 2);
		
	}
	
	bool IsTouchInsideList (Vector2 touchPos)
	{
		Vector2 screenPos = new Vector2 (touchPos.x, Screen.height - touchPos.y);  // invert y coordinate
		Rect rAdjustedBounds = new Rect (430, 100, 370, 330);
		
		return rAdjustedBounds.Contains (screenPos);
	}

	void actuallyLoadLevel (TweenEvent data) {
		op.allowSceneActivation = true;
	}
}
