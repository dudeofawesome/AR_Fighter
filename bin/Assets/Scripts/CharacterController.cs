using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public int jumpForce = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		GameObject _Player1 = GameObject.Find ("Player1");
		if (Input.GetKey ("a")) {
			print("bang");
			_Player1.transform.position = new Vector3(_Player1.transform.position.x - 0.1f,_Player1.transform.position.y,_Player1.transform.position.z);
		}
		if (Input.GetKey ("d")) {
			print ("boom");
			_Player1.transform.position = new Vector3(_Player1.transform.position.x + 0.1f,_Player1.transform.position.y,_Player1.transform.position.z);
		}


	}
}
