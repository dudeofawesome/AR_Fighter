using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class MainMenuGUI : MonoBehaviour
{
	
	public GUISkin guiSkin, verticalSkin, scrollSkin, howtoplaySkin, inGameSkin, audioLevelSkin, serverPickerSkin;
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
		SETTINGSAUDIO,
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
		if (PlayerPrefs.GetInt ("firstLaunch") == 0) {
			menuPosition = MenuState.HOWTOPLAY;

			PlayerPrefs.SetInt ("masterVolume", 100);
			PlayerPrefs.SetInt ("musicVolume", 100);
			PlayerPrefs.SetInt ("soundEffectVolume", 100);

			PlayerPrefs.SetInt ("firstLaunch", 1);
		}

		if (PlayerPrefs.GetInt ("controlScheme") == 0) {
			PlayerPrefs.SetInt ("controlScheme", 1);
		}

		HOTween.Kill ();
		
		UpdateSettings ();
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		//2 and a half seconds
		HOTween.Init (false, false, false);
		HOTween.DisableOverwriteManager ();

		SetAudioLevels ();
		
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
			
			GUI.Box(new Rect(380,100,400,72), "Sole Champion");
			
			
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
			GUI.skin = serverPickerSkin;

			GUI.Box (new Rect (220, 0, 530, 360), "");
			scrollPosition = GUI.BeginScrollView (new Rect (220, 20, 530, 330),
			                                      scrollPosition, new Rect (0, 0, 340, 420));
			if (PhotonNetwork.connected && PhotonNetwork.room == null) {
				RoomInfo[] _rooms = PhotonNetwork.GetRoomList ();
				int i = 0;
				for (i = 0; i < _rooms.Length; i++) {
					if (GUI.Button (new Rect (0, 5 + i * 45, 530, 50), _rooms [i].name + " | " + _rooms [i].playerCount + "/" + _rooms [i].maxPlayers)) {
						GameObject.Find ("SessionStarter").GetComponent<SessionStarter> ().roomHost = false;
						PhotonNetwork.JoinRoom (_rooms [i].name);
					}
				}
				if (GUI.Button (new Rect (0, 5 + i * 45, 530, 50), "Create new room")) {
					print ("created room with name " + System.Environment.UserName);
					PhotonNetwork.CreateRoom (System.Environment.UserName + "'s Room", new RoomOptions () { maxPlayers = 2 }, null);
					menuPosition = MenuState.LEVELLOADER;
				}
			}
			GUI.EndScrollView();

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
				
				
			} else if (GUI.Button (new Rect (380, 215, 400, 55), "Audio")) {
				menuPosition = MenuState.SETTINGSAUDIO;
				
				
			}
			else if (GUI.Button (new Rect (380, 265, 400, 55), "Controls")) {
				menuPosition = MenuState.SETTINGSCONTROLS;
				
				
			}
			
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			}
			
			
			
			break;

		case MenuState.SETTINGSAUDIO:
			GUI.skin = guiSkin;
			GUI.Label (new Rect (30, 75, 325, 325), "");
			GUI.Box(new Rect(380,100,400,72), "Audio");

			int _a = PlayerPrefs.GetInt ("masterVolume");
			PlayerPrefs.SetInt ("masterVolume", (int) GUI.HorizontalSlider(new Rect(435, 173, 288, 30), _a, 0, 100));
			GUI.Button (new Rect (380, 165, 400, 55), "Master");
			GUI.HorizontalSlider(new Rect(435, 173, 288, 30), _a, 0, 100);

			_a = PlayerPrefs.GetInt ("musicVolume");
			PlayerPrefs.SetInt ("musicVolume", (int) GUI.HorizontalSlider(new Rect(435, 223, 288, 30), _a, 0, 100));
			GUI.Button (new Rect (380, 215, 400, 55), "Music");
			GUI.HorizontalSlider(new Rect(435, 223, 288, 30), _a, 0, 100);

			_a = PlayerPrefs.GetInt ("soundEffectVolume");
			PlayerPrefs.SetInt ("soundEffectVolume", (int) GUI.HorizontalSlider(new Rect(435, 273, 289, 30), _a, 0, 100));
			GUI.Button (new Rect (380, 265, 400, 55), "Sound Effects");
			GUI.HorizontalSlider(new Rect(435, 273, 289, 30), _a, 0, 100);

			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.SETTINGS;
			}

			SetAudioLevels();

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
//			if (GUI.Button (new Rect (100, 265, 600, 55), "Full Tilt"))
//				PlayerPrefs.SetInt ("controlScheme", 0);
			if (GUI.Button (new Rect (100, 215, 600, 55),"Tilt with buttons"))
				PlayerPrefs.SetInt ("controlScheme", 1);
			if (GUI.Button (new Rect (100, 165, 600, 55),"On screen buttons"))
				PlayerPrefs.SetInt ("controlScheme", 2);
			
			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.SETTINGS;
			}
			
			
			break;
			
		case MenuState.LEVELLOADER:
			GUI.skin = scrollSkin;
			

			GUI.Box (new Rect (0, 20, 800, 460), "");
			GUI.Label (new Rect (50, 20, 550, 85), "Select Visual Style");
			
			
			scrollPosition = GUI.BeginScrollView (new Rect (430, 120, 360, 330),
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

			GUI.Button (new Rect (0, 140, 340, 70), "Coming Soon");
			GUI.Button (new Rect (0, 210, 340, 70), "Coming Soon");
			GUI.Button (new Rect (0, 280, 340, 70), "Coming Soon");
			GUI.Button (new Rect (0, 350, 340, 70), "Coming Soon");
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
			GUI.Label (new Rect (0, 50, 800, 450), "Sole Champion is an unique mobile device game that utilizes AR cards to create 3D environments. " +
			           "The application is a fighting game played alone against AI or with friends. The players brawl each other in a free-for-all to see who will emerge as the top fighter.");

			if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (630, 400, 150, 70), "Next")) {
				menuPosition = MenuState.HOWTOPRINT;
			}
			break;

			
		case MenuState.HOWTOPRINT:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "How to Print");
			GUI.Label (new Rect (0, 50, 800, 450), "Playing Sole Champion requires AR cards to display the world. " +
				"The platform card is the main card that can snap to other cards and allows the users to create position their world. " +
				"The cards should be downloaded and printed on cardstock or high quality paper to ensure that the camera can see them. " +
			           "The cards can be downloaded at");
			if (GUI.Button(new Rect(0,350,850,45), "http://0rleans.com/images/AR_Targets.jpg")) {
				Application.OpenURL("http://0rleans.com/images/AR_Targets.jpg");
			}
			GUI.skin = howtoplaySkin;
			if (GUI.Button (new Rect (315, 400, 150, 70), "Main")) {
				menuPosition = MenuState.MAIN;
			} else if (GUI.Button (new Rect (0, 400, 150, 70), "Back")) {
				menuPosition = MenuState.HOWTOPLAY;
			} else if (GUI.Button (new Rect (630, 400, 150, 70), "Next")) {
				menuPosition = MenuState.HOWTOSETUP;
			}
			break;
			
		case MenuState.HOWTOSETUP:
			GUI.skin = howtoplaySkin;
			GUI.Box (new Rect (0, 0, 800, 50), "How to Set Up");
			GUI.Label (new Rect (0, 50, 800, 450), "Once the cards have been printed out, lay the card lebeled \"R\" face up on the left edge of the table. " +
			    "This card will act as the left edge of the playing field, and will make the playing field appear on the table. " +
				"The AR cards should be placed on a flat surface and be in view of every player's device.");
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
			GUI.Label (new Rect (0, 50, 800, 450), "You may select different control schemes in the settings menu. " +
			           /*"Full Tilt - actions are all done by tilting. " + */ "\n"+ "Tilt with Buttons - move by tilting the device left and right, jump and attack with buttons. " +"\n"+
				"On Screen Buttons - all actions are done with buttons on the screen.");
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

	void SetAudioLevels () {
		AudioListener.volume = (float) ((double) PlayerPrefs.GetInt ("masterVolume") / 100.0);
		GameObject.Find("Music").GetComponent<AudioSource>().volume = (float) (((double) PlayerPrefs.GetInt ("musicVolume") / 100.0) * 0.3);
	}
}
