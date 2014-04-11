﻿using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public int jumpForce = 500000;
	public int movementForce = 10000;
	public float drag = 0.15f;

	public GameObject feet = null;

	public float damage = 100;
	public bool stunned = false;
	private int stunnedCountDown = 30;

	public string keyUp = "w";
	public string keyLeft = "a";
	public string keyDown = "s";
	public string keyRight = "d";
	public string keyAttack = "space";


	private bool touchingGround = false;
	private bool doubleJumpUsed = false;

	private GameObject touchingEnemy = null;

	private Player.Attack currentAttack = null;

	private bool movementKeyDown = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (!stunned){
			if (Input.GetKeyDown (keyUp) && !doubleJumpUsed) {
				rigidbody.velocity = new Vector3(rigidbody.velocity.x,0,0);
				rigidbody.AddForce(new Vector3(0,jumpForce,0),ForceMode.Force);
				if (!touchingGround)
					doubleJumpUsed = true;
			}
			if (Input.GetKeyDown (keyDown)) {
				transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
			}
			if (Input.GetKeyUp (keyDown)) {
				transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			}
			if (Input.GetKeyDown (keyAttack) && currentAttack == null) {
				currentAttack = new Player.Attack(this.gameObject);
			}

			if (feet.GetComponent<CollisionHandeler>().collidingWith != null && (feet.GetComponent<CollisionHandeler>().collidingWith.layer == 8 || feet.GetComponent<CollisionHandeler>().collidingWith.layer == 9)) {
				touchingGround = true;
				doubleJumpUsed = false;
			}
			else{
				touchingGround = false;
			}

			//apply max velocity
			if (movementKeyDown && (rigidbody.velocity.x > 20f || rigidbody.velocity.x < -20f))
				rigidbody.velocity = new Vector3 (Mathf.Clamp (rigidbody.velocity.x, -20f, 20f), rigidbody.velocity.y, 0);
		}
	}

	void FixedUpdate () {
		if (!stunned) {
			movementKeyDown = false;
			if (Input.GetKey (keyLeft)) {
	//			Player.transform.position = new Vector3(Player1.transform.position.x - 0.5f,Player1.transform.position.y,Player1.transform.position.z);
				rigidbody.AddForce(new Vector3(movementForce * -1,0,0));
				transform.rotation = Quaternion.Euler( 0, 180, 0);
				movementKeyDown = true;
			}
			if (Input.GetKey (keyRight)) {
	//			Player.transform.position = new Vector3(Player.transform.position.x + 0.5f,Player.transform.position.y,Player.transform.position.z);
				rigidbody.AddForce(new Vector3(movementForce,0,0));
				transform.rotation = Quaternion.Euler( 0, 0, 0);
				movementKeyDown = true;
			}

			if (!Input.GetKey (keyLeft) && !Input.GetKey (keyRight) && touchingGround) {
				rigidbody.velocity = new Vector3 (rigidbody.velocity.x * (1 - drag), rigidbody.velocity.y, 0);
			}
			else if (!touchingGround) {
				rigidbody.velocity = new Vector3 (rigidbody.velocity.x * (0.95f), rigidbody.velocity.y, 0);
			}


			if (currentAttack != null) {
				if (currentAttack.countdown()) {
					currentAttack = null;
				}
			}
		}
		else{
			stunnedCountDown--;
			if (stunnedCountDown <= 0)
				stunned = false;
		}
	}

	public void hurt (float damage, Vector3 source) {
		this.damage += damage;
		if (damage > 20) {
			stunned = true;
			stunnedCountDown = damage;
		}
		rigidbody.AddForce (movementForce * this.damage * ((source.x > 0) ? -1 : 1), jumpForce / 40 * this.damage, 0);
	}

//	void OnCollisionEnter (Collision collision) {
//		if (collision.gameObject.tag == "Player") {
//			touchingEnemy = collision.gameObject;
//		}
//	}
//
//	void OnCollisionExit (Collision collision) {
//		if (collision.gameObject.tag == "Player") {
//			touchingEnemy = null;
//		}
//	}

	public GameObject attackLanded () {
		return GameObject.Find (gameObject.name + "/AttackHitbox").GetComponent<CollisionHandeler> ().collidingWith;
	}
}
