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
		if (collision.gameObject.name == "AttackHitbox") {
			collidingWith = collision.gameObject.transform.parent.gameObject;
		}
		else {
			collidingWith = collision.gameObject;
		}
	}
	
	void OnTriggerExit (Collider collision) {
		if (collision.gameObject == collidingWith)
			collidingWith = null;
	}

	void OnCollisionEnter (Collision collision) {
		collidingWith = collision.gameObject;
	}
	
	void OnCollisionExit (Collision collision) {
		if (collision.gameObject == collidingWith)
			collidingWith = null;
	}
}
