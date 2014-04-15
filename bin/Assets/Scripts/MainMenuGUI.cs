using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	public GUISkin guiSkin;

	public string mainLevel;

	public enum MenuState {MAIN, LEVELLOADER, SETTINGS, HOWTOPLAY};

	public MenuState menuPosition = MenuState.MAIN;


	private float originalWidth = 800; 
	private float originalHeight = 480; 
	private Vector3 scale; 


	// Use this for initialization
	void Start () {

		Screen.orientation = ScreenOrientation.LandscapeRight;

		//Application.LoadLevel (mainLevel);
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void OnGUI () {

		GUI.skin = guiSkin;

		scale.x = Screen.width/originalWidth; // calculate hor scale
		scale.y = Screen.height/originalHeight; // calculate vert scale
		scale.z = 1;
		var svMat = GUI.matrix; // save current matrix
		// substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
		// draw your GUI controls here:

		switch(menuPosition){
			
		case MenuState.MAIN:
			
			//GUI.Box (new Rect(0, 0, 800, 480), "");
			
			GUI.Label (new Rect(50, 50, 300, 300), "TITLE");
			//PlayerPrefs.SetString ("name",GUI.TextField (new Rect(100, 200, 100, 50), PlayerPrefs.GetString("name")));
			
			if(GUI.Button (new Rect(450, 50, 300, 70), "Start")){
				menuPosition = MenuState.LEVELLOADER;

				
			}
			else if(GUI.Button (new Rect(450, 150, 300, 70), "Settings")){
				menuPosition = MenuState.SETTINGS;

				
			}
			else if(GUI.Button (new Rect(450,250, 300, 70), "How To Play")){
				menuPosition = MenuState.HOWTOPLAY;


			}

			else if(GUI.Button (new Rect(450, 350, 300, 70), "Exit")){
				//QUIT GAME
			}
			
			
			break;
		case MenuState.SETTINGS:
			GUI.Box (new Rect(50,20, 700, 70), "Set Visual Effects Intensity");
			GUI.Box (new Rect(30, 90, 770, 390),"");
			break;
		case MenuState.LEVELLOADER:
			GUI.Box (new Rect(50, 20, 500, 70), "Select Visual Style");
			GUI.Box (new Rect(30, 90, 770, 390),"");
			break;
		case MenuState.HOWTOPLAY:
			GUI.Box(new Rect(50, 20, 500, 70), "How To Play");
			GUI.Box (new Rect(30, 90, 770, 390),"");
			break;
		
		}



		//...
		// restore matrix before returning
		GUI.matrix = svMat; // restore matrix







	
	}

}
