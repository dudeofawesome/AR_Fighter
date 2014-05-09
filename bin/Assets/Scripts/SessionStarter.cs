using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Multiplayer/Session Starter")]
public class SessionStarter : MonoBehaviour {
	public bool roomHost = true;
	public bool singlePlayer = true;

	public virtual void Start() {
		DontDestroyOnLoad(transform.gameObject);
//		PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
		PhotonNetwork.ConnectUsingSettings("Alpha 0.1");
	}
	
	public virtual void Update() {
//		if (ConnectInUpdate && AutoConnect)
//		{
//			print("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
//			
//			ConnectInUpdate = false;
//			PhotonNetwork.ConnectUsingSettings("1");
//		}
	}

	// to react to events "connected" and (expected) error "failed to join random room", we implement some methods. PhotonNetworkingMessage lists all available methods!
	public virtual void OnConnectedToMaster() {
//		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
//		PhotonNetwork.JoinRandomRoom();
	}
	
	public virtual void OnPhotonRandomJoinFailed() {
//		Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
//		PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 4 }, null);
	}
	
	// the following methods are implemented to give you some context. re-implement them as needed.
	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause) {
		Debug.LogError("Cause: " + cause);
	}
	
	public void OnJoinedRoom() {
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		print ("room name" + PhotonNetwork.room.name);
		if (!roomHost) {
			if (PhotonNetwork.room.customProperties.ContainsKey("gameLevel")) {
				switch (PhotonNetwork.room.customProperties["gameLevel"].ToString()) {
					case "treeTower" :
						Application.LoadLevel("Main");
					break;
				}
			}
		}
	}
	
	public virtual void OnJoinedLobby() {
		Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
//		if (PhotonNetwork.GetRoomList().Length == 0) {
//			print ("created room with name " + System.Environment.UserName);
//			PhotonNetwork.CreateRoom(System.Environment.UserName + "'s Room", new RoomOptions() { maxPlayers = 2 }, null);
//		}
//		else {
//			print (PhotonNetwork.GetRoomList().Length);
//		}
	}
}