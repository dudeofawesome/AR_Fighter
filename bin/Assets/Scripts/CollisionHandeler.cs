using UnityEngine;
using System.Collections;

public class CollisionHandeler : MonoBehaviour {

	public GameObject collidingWith = null;
	public Collision collisionObj = null;

	// Use this for initialization
	void Start () {

	}

	void Update () {

	}

	void OnTriggerEnter (Collider collision) {
		if (gameObject.name == "LedgeGrabHitbox") {
			if (collision.gameObject.tag == "LedgeGrab")
				collidingWith = collision.gameObject;
		}
		else if (collision.gameObject.name != "AttackHitbox") {
			collidingWith = collision.gameObject;
		}
	}
	
	void OnTriggerExit (Collider collision) {
		if (collision.gameObject == collidingWith)
			collidingWith = null;
	}

	void OnCollisionEnter (Collision collision) {
		if (gameObject.name == "LedgeGrabHitbox") {
			if (collision.gameObject.tag == "LedgeGrab")
				collidingWith = collision.gameObject;
		}
		else {
			collidingWith = collision.gameObject;
			collisionObj = collision;
		}
	}
	
	void OnCollisionExit (Collision collision) {
		if (collision.gameObject == collidingWith) {
			collidingWith = null;
			collisionObj = null;
		}
	}
}
