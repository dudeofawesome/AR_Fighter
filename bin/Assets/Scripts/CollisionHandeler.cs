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
		if (collision.gameObject.name == "AttackHitbox") {
			collidingWith = collision.gameObject.transform.parent.gameObject;
		}
		else {
			collidingWith = collision.gameObject;
		}
	}
	
	void OnTriggerExit (Collider collision) {
		if (collision.gameObject == collidingWith || (collision.gameObject.name == "AttackHitbox" && collision.gameObject.transform.parent.gameObject == collidingWith))
			collidingWith = null;
	}

	void OnCollisionEnter (Collision collision) {
		collidingWith = collision.gameObject;
		collisionObj = collision;
	}
	
	void OnCollisionExit (Collision collision) {
		if (collision.gameObject == collidingWith || (collision.gameObject.name == "AttackHitbox" && collision.gameObject.transform.parent.gameObject == collidingWith)) {
			collidingWith = null;
			collisionObj = null;
		}
	}
}
