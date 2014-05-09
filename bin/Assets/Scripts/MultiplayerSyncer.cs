using System;
using UnityEngine;
using System.Collections;

[AddComponentMenu("Multiplayer/Multiplayer Syncer")]
public class MultiplayerSyncer : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		if (PhotonNetwork.room != null) {
			GameObject _myPlayer = PhotonNetwork.Instantiate("Player", new Vector3(10, 30, 0), Quaternion.identity, 0);
			_myPlayer.GetComponent<CharacterController>().controlMe = true;
			_myPlayer.GetComponent<CharacterController>().networkControl = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Awake()
	{
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;
		
		// generate a name for this player, if none is assigned yet
		if (String.IsNullOrEmpty(PhotonNetwork.playerName))
		{
			PhotonNetwork.playerName = "Guest" + UnityEngine.Random.Range(1, 9999);
		}
	}
}