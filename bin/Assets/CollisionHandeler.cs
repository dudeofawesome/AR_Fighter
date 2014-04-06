using UnityEngine;
using System.Collections;

public class CollisionHandeler : MonoBehaviour {

	public GameObject collidingWith = null;

	// Use this for initialization
	void Start () {

	}

	void Update () {

	}

	void OnTriggerEnter (Collider collision) {
		collidingWith = collision.gameObject;
	}
	
	void OnTriggerExit (Collider collision) {
		print ("exit");
		collidingWith = null;
	}
}
