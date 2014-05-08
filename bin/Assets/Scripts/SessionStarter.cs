using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Multiplayer/Session Starter")]
public class SessionStarter : MonoBehaviour {
	/// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
	public bool AutoConnect = false;
	
	/// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
	private bool ConnectInUpdate = true;
	
	public virtual void Start() {
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

	void OnGUI () {
//		GUI.Label (new Rect (5, Screen.height - 18, 200, 20), "status: " + PhotonNetwork.connectionStateDetailed.ToString() + ((PhotonNetwork.room != null) ? " " + PhotonNetwork.room.name : ""));
//
//		print (PhotonNetwork.GetRoomList().Length);
//		if(PhotonNetwork.connected && PhotonNetwork.room == null){
//			RoomInfo[] _rooms = PhotonNetwork.GetRoomList();
//			int i = 0;
//			for (i = 0; i < _rooms.Length; i++) {
//				print (_rooms[i].name);
//				if (GUI.Button(new Rect(Screen.width / 2 - 100, 10 + i * 30, 200, 25), _rooms[i].name + " " + _rooms[i].playerCount + "/" + _rooms[i].maxPlayers)) {
//					PhotonNetwork.JoinRoom(_rooms[i].name);
//				}
//			}
//			if (GUI.Button(new Rect(Screen.width / 2 - 100, 10 + i * 30, 200, 25), "Create new room")) {
//				print ("created room with name " + System.Environment.UserName);
//				PhotonNetwork.CreateRoom(System.Environment.UserName + "'s Room", new RoomOptions() { maxPlayers = 2 }, null);
//			}
//		}
	}
}
