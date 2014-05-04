﻿using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class MainMenuGUI : MonoBehaviour
{

		public GUISkin guiSkin, verticalSkin, scrollSkin, howtoplaySkin;
		public string mainLevel;


		public enum MenuState
		{
				MAIN,
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
		private bool onLoading = false;
		public Texture loadingTexture = null;
		public float angle = 0;
		public Vector2 size = new Vector2 (50, 58);
		Vector2 pos = new Vector2 (0, 0);
		Rect rect;
		Vector2 pivot;
		
	private int tweenSwitch = 0;

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


				
		if (tweenSwitch == 0) {
			//HOTween.To (GameObject.Find ("Map").transform, 10, "rotation", Quaternion.Euler (new Vector3 (0, 0, 0)));
			HOTween.To (GameObject.Find ("Map").transform, 10, new TweenParms ().Prop ("rotation", Quaternion.Euler (new Vector3 (0, 0, 0))).Ease(EaseType.Linear).OnComplete (tweenFunction));
		}
		
		//HOTween.To (GameObject.Find ("Map").transform, 5, "rotation", Quaternion.Euler (new Vector3 (0, 180, 0)));
				
		//HOTween.To (GameObject.Find ("Main Camera").transform, 15, "position", new Vector3 (GameObject.Find ("dojo_in_tree").transform.position.x, GameObject.Find ("dojo_in_tree").transform.position.y + 3f, GameObject.Find ("dojo_in_tree").transform.position.z));




	
		}

	void tweenFunction(){
		Debug.Log ("Work");
		tweenSwitch = 1;
	//	HOTween.To (GameObject.Find ("Map").transform, 1, "rotation", Quaternion.Euler (new Vector3 (0, 360, 0)));
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


						GUI.Label (new Rect (20, 50, 350, 350), "");
			//PlayerPrefs.SetString ("name",GUI.TextField (new Rect(100, 200, 100, 50), PlayerPrefs.GetString("name")));


			

						if (GUI.Button (new Rect (490, 50, 340, 70), "Start")) {

								menuPosition = MenuState.LEVELLOADER;

				
						} else if (GUI.Button (new Rect (490, 150, 340, 70), "Settings")) {
								menuPosition = MenuState.SETTINGS;

				
						} else if (GUI.Button (new Rect (490, 250, 340, 70), "How To Play")) {
								menuPosition = MenuState.HOWTOPLAY;


						} else if (GUI.Button (new Rect (490, 350, 340, 70), "Exit")) {
								//QUIT GAME
								Application.Quit ();
						}
			

			
						break;
				case MenuState.SETTINGS:
						GUI.Label (new Rect (20, 50, 350, 350), "");

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
								Application.LoadLevel (mainLevel);
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
								menuPosition = MenuState.MAIN;
						} else if (GUI.Button (new Rect (200, 400, 150, 70), "Start!")) {
								menuPosition = MenuState.LOADING;
								//Application.LoadLevel (mainLevel);
						}


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
						angle += 1;





						break;
				}



				//...
				// restore matrix before returning
				GUI.matrix = svMat; // restore matrix







	
		}

		void UpdateSettings ()
		{
				pos = new Vector2 (transform.localPosition.x, transform.localPosition.y);
				rect = new Rect (pos.x - size.x * .01f, pos.y - size.y * .01f, size.x, size.y);
				pivot = new Vector2 (rect.width / 2 - 20, rect.height / 2 - 20);
		}

		bool IsTouchInsideList (Vector2 touchPos)
		{
				Vector2 screenPos = new Vector2 (touchPos.x, Screen.height - touchPos.y);  // invert y coordinate
				Rect rAdjustedBounds = new Rect (430, 100, 370, 330);
		
				return rAdjustedBounds.Contains (screenPos);
		}

}


