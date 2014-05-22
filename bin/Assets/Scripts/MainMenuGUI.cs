using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class MainMenuGUI : MonoBehaviour
{
	
	public GUISkin guiSkin, verticalSkin, scrollSkin, howtoplaySkin;
	public string mainLevel;
	
	
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
	AsyncOperation op = null;
	
	// Use this for initialization
	void Start ()
	{
		
		
		
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
			
			
			GUI.Label (new Rect (45, 75, 325, 325), "");
			//PlayerPrefs.SetString ("name",GUI.TextField (new Rect(100, 200, 100, 50), PlayerPrefs.GetString("name")));
			
			
			
			
			if (GUI.Button (new Rect (490, 50, 340, 70), "Start")) {
				
				menuPosition = MenuState.PLAYERS;
				
				
			} else if (GUI.Button (new Rect (490, 150, 340, 70), "Settings")) {
				menuPosition = MenuState.SETTINGS;
				
				
			} else if (GUI.Button (new Rect (490, 250, 340, 70), "How To Play")) {
				menuPosition = MenuState.HOWTOPLAY;
				
				
			} else if (GUI.Button (new Rect (490, 350, 340, 70), "Exit")) {
				//QUIT GAME
				Application.Quit ();
			}
			
			
			
			break;
			
		case MenuState.PLAYERS:
			GUI.Label (new Rect (45, 75, 325, 325), "");
			if (GUI.Button (new Rect (340, 150, 460, 70), "Single Player")) {
				
				menuPosition = MenuState.LEVELLOADER;
				
				
			} else if (GUI.Button (new Rect (340, 250, 460, 70), "Multiplayer")) {
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
			GUI.Label (new Rect (45, 75, 325, 325), "");
			
			if (GUI.Button (new Rect (340, 150, 460, 70), "Visual Settings")) {
				menuPosition = MenuState.SETTINGSVI;
				
				
			} else if (GUI.Button (new Rect (340, 250, 460, 70), "Control Settings")) {
				menuPosition = MenuState.SETTINGSCONTROLS;
				
				
			}
			
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			}
			
			
			
			break;
			
		case MenuState.SETTINGSVI:
			GUI.skin = verticalSkin;
			
			GUI.Box (new Rect (50, 20, 700, 70), "Set Visual Effects Intensity");
			GUI.Box (new Rect (30, 90, 770, 390), "");
			var names = QualitySettings.names;
			
			
			GUILayout.BeginArea (new Rect (120, 100, 680, 600));
			for (var i = 0; i < names.Length; i++) {
				if (GUILayout.Button (names [i]))
					QualitySettings.SetQualityLevel (i, true);
			}
			GUILayout.EndArea ();
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.SETTINGS;
			}
			
			
			
			break;
			
		case MenuState.SETTINGSCONTROLS:
			
			GUILayout.BeginArea (new Rect (120, 100, 680, 600));
			if (GUILayout.Button ("Full Tilt")) {
				PlayerPrefs.SetInt ("controlScheme", 0);
			}
			if (GUILayout.Button ("Tilt with buttons")) {
				PlayerPrefs.SetInt ("controlScheme", 1);
			}
			if (GUILayout.Button ("On screen buttons")) {
				PlayerPrefs.SetInt ("controlScheme", 2);
			}
			GUILayout.EndArea ();
			
			
			
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
				op = Application.LoadLevelAsync (mainLevel);
				op.allowSceneActivation = false;
				HOTween.To(GameObject.Find ("Map").transform, 4, new TweenParms().Prop("rotation", new Vector3(0,360,0), true).UpdateType(UpdateType.TimeScaleIndependentUpdate).OnComplete(actuallyLoadLevel));
				HOTween.To(GameObject.Find ("Main Camera").transform, 4, "position", GameObject.Find ("Map/dojo_in_tree/TweenTo").transform.position);
				HOTween.To(GameObject.Find ("Main Camera").transform, 4, "rotation", Quaternion.Euler(0, -40, 0));
			}
			GUI.Button (new Rect (0, 70, 340, 70), "Setting2");
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
			} else if (GUI.Button (new Rect (200, 400, 150, 70), "Start!")) {
				menuPosition = MenuState.LOADING;
				//Application.LoadLevel (mainLevel);
			}
			
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
			UpdateSettings ();
			
			
			Matrix4x4 matrixBackup = GUI.matrix;
			GUIUtility.RotateAroundPivot (angle, pivot);
			GUI.DrawTexture (rect, loadingTexture);
			GUI.matrix = matrixBackup;
			angle += 5;
			
			
			
			
			
			break;
		}
		
		
		
		//...
		// restore matrix before returning
		GUI.matrix = svMat; // restore matrix
		
		
		
		
		
		
		
		
	}
	
	void UpdateSettings ()
	{
		//pos = new Vector2 (transform.localPosition.x + 200, transform.localPosition.y + 100);
		//pos = new Vector2 (size.x,size.y);
		//rect = new Rect (pos.x - size.x * .01f, pos.y - size.y * .01f, size.x, size.y);
		rect = new Rect (Screen.width / 2 + 100, Screen.height / 2 + 10, size.x, size.y);
		//pivot = new Vector2 (rect.width/2, rect.height/2);
		pivot = new Vector2 (Screen.width / 2, Screen.height / 2);
		
	}
	
	bool IsTouchInsideList (Vector2 touchPos)
	{
		Vector2 screenPos = new Vector2 (touchPos.x, Screen.height - touchPos.y);  // invert y coordinate
		Rect rAdjustedBounds = new Rect (430, 100, 370, 330);
		
		return rAdjustedBounds.Contains (screenPos);
	}

	void actuallyLoadLevel (TweenEvent data) {
		print ("switch scene");
		op.allowSceneActivation = true;
	}
}
