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
		collidingWith = null;
	}

	void OnCollisionEnter (Collision collision) {
		collidingWith = collision.gameObject;
	}
	
	void OnCollisionExit (Collision collision) {
		collidingWith = null;
	}
}
