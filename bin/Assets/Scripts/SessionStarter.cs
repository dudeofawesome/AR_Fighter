using UnityEngine;
using System.Collections;

[AddComponentMenu("Multiplayer/Session Starter")]
public class SessionStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings("v4.2");
		//while (!PhotonNetwork.connected) {}
		PhotonNetwork.CreateRoom("MyMatch");
		PhotonNetwork.CreateRoom("MyMatcher");
		PhotonNetwork.CreateRoom("MyMatchest");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		if(PhotonNetwork.connected){
			foreach(RoomInfo game in PhotonNetwork.GetRoomList()){
				if (GUILayout.Button(game.name + " " + game.playerCount + "/" + game.maxPlayers)) {
					PhotonNetwork.JoinRoom(game.name);
				}
			}
		}
	}
}
