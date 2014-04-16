using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public bool controlMe = false;

	public int jumpForce = 500000;
	public int movementForce = 10000;
	public float drag = 0.15f;

	public GameObject feet = null;
	public GameObject attackHitbox = null;
	public GameObject ledgeGrabHitbox = null;

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
	public bool ledgeHanging = false;

	private GameObject touchingEnemy = null;

	private Player.Attack currentAttack = null;

	private bool movementKeyDown = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (!stunned){
			if (controlMe) {
				if (!ledgeHanging){
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


					//Phone Controls
					foreach (Touch _touch in Input.touches){
						if (_touch.phase == TouchPhase.Began && !doubleJumpUsed && _touch.position.x > Screen.width / 2) {
							rigidbody.velocity = new Vector3(rigidbody.velocity.x,0,0);
							rigidbody.AddForce(new Vector3(0,jumpForce,0),ForceMode.Force);
							if (!touchingGround)
								doubleJumpUsed = true;
						}
						if (_touch.phase == TouchPhase.Began && currentAttack == null && _touch.position.x < Screen.width / 2) {
							currentAttack = new Player.Attack(this.gameObject);
						}
					}
				}
			}

			if (!touchingGround) {
				GameObject _ledge = ledgeGrabHitbox.GetComponent<CollisionHandeler>().collidingWith;
				if (_ledge != null && rigidbody.velocity.y <= 0 && _ledge.tag == "LedgeGrab") {
					rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					rigidbody.velocity = new Vector3(0, 0, 0);
					ledgeHanging = true;
					transform.position = new Vector3(_ledge.transform.position.x + (_ledge.transform.forward.x * 3000000), _ledge.transform.position.y, 0);
					print(_ledge.transform.forward.x);
				}
			}

			if (ledgeHanging) {
				if (Input.GetKeyDown (keyUp)) {
					rigidbody.AddForce(0, jumpForce, 0);
					rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
					ledgeHanging = false;
				}
				if (Input.GetKeyDown (keyDown)) {
					stunned = true;
					stunnedCountDown = 15;
					rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
					ledgeHanging = false;
				}


				//Phone controls
				foreach (Touch _touch in Input.touches){
					if (_touch.phase == TouchPhase.Began && _touch.position.x > Screen.width / 2) {
						rigidbody.AddForce(0, jumpForce, 0);
						rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
						ledgeHanging = false;
					}
//					if (_touch.phase == TouchPhase.Began && currentAttack == null && _touch.position.x < Screen.width / 2) {
//						currentAttack = new Player.Attack(this.gameObject);
//					}
				}
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

			// Right Boundary
		if (transform.position.x > ((GameObject.Find ("Ground").transform.position.x) * 2) + 6 && transform.position.y < - 40){
					transform.position = new Vector3 (0, 5, 0);
			}
		// Left Boundary
		if (transform.position.x < - 6 && transform.position.y < - 40){
			transform.position = new Vector3 (0, 5, 0);
		}

		//If it falls down below anways
		if (transform.position.y < - 40) {
			transform.position = new Vector3 (0, 5, 0);
		}

	}

	void FixedUpdate () {
		if (!stunned) {
			movementKeyDown = false;
			if (controlMe) {
				if (!ledgeHanging){
					if (Input.GetKey (keyLeft)) {
						rigidbody.AddForce(new Vector3(movementForce * -1,0,0));
						transform.rotation = Quaternion.Euler( 0, 180, 0);
						movementKeyDown = true;
					}
					if (Input.GetKey (keyRight)) {
						rigidbody.AddForce(new Vector3(movementForce,0,0));
						transform.rotation = Quaternion.Euler( 0, 0, 0);
						movementKeyDown = true;
					}



					// Phone controls
					float _tilt = Mathf.Clamp (Input.acceleration.x * 4, -1, 1);
					if (_tilt > -0.1 && _tilt < 0.1)
						_tilt = 0;
					rigidbody.AddForce(new Vector3(movementForce * _tilt,0,0));
					if (_tilt > 0) {
						transform.rotation = Quaternion.Euler( 0, 0, 0);
						movementKeyDown = true;
					}
					else if (_tilt < 0) {
						transform.rotation = Quaternion.Euler( 0, 180, 0);
						movementKeyDown = true;
					}
<<<<<<< HEAD
=======
				}
			}
			// AI !!!
			else {
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject player in players) {
					if (player.name != gameObject.name) {
						//Track to this player
						rigidbody.AddForce(new Vector3(movementForce * ((player.transform.position.x > transform.position.x) ? 1 : -1),0,0));
						movementKeyDown = true;
						//Rotate to face
						transform.rotation = Quaternion.Euler( 0, ((player.transform.position.x > transform.position.x) ? 0 : 180), 0);
						//Consider attacking
						if (Mathf.Abs(player.transform.position.x - transform.position.x) < Random.Range(1f, 3f)) {
							if (currentAttack == null) {
								currentAttack = new Player.Attack(this.gameObject);
							}
						}
						//Decide whether or not to drop off ledge
						if (ledgeHanging && Random.Range(0,50) == 0) {
							stunned = true;
							stunnedCountDown = 15;
							rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
							ledgeHanging = false;
						}
						else if (ledgeHanging && Random.Range(0,50) == 0) {
							rigidbody.AddForce(0, jumpForce, 0);
							rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
							ledgeHanging = false;
						}
					}
>>>>>>> Alpha-Louis
				}
			}


			// Drag
			if (!movementKeyDown && touchingGround) {
				rigidbody.velocity = new Vector3 (rigidbody.velocity.x * (1 - drag), rigidbody.velocity.y, 0);
			}
			else if (!touchingGround) {
				rigidbody.velocity = new Vector3 (rigidbody.velocity.x * (0.97f), rigidbody.velocity.y, 0);
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
			stunnedCountDown = (int) damage;
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
		return attackHitbox.GetComponent<CollisionHandeler> ().collidingWith;
	}
}
