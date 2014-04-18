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

	public float damage = 0;
	public bool stunned = false;
	private int stunnedCountDown = 30;
	public int carefulness = 10;

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
				if (Input.GetKeyDown (keyUp) && !doubleJumpUsed) {
					jump();
				}
				if (Input.GetKeyDown (keyDown)) {
					crouch(true);
				}
				if (Input.GetKeyUp (keyDown)) {
					crouch(false);
				}
				if (Input.GetKeyDown (keyAttack) && currentAttack == null) {
					attack();
				}


				//Phone Controls
				foreach (Touch _touch in Input.touches){
					if (_touch.phase == TouchPhase.Began && !doubleJumpUsed && _touch.position.x > Screen.width / 2) {
						jump();
					}
					if (_touch.phase == TouchPhase.Began && currentAttack == null && _touch.position.x < Screen.width / 2) {
						attack();
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
					jump();
				}
				if (Input.GetKeyDown (keyDown)) {
					crouch(true);
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
		if (transform.position.x < -40 || transform.position.x > ((GameObject.Find ("Ground/LedgeGrabRight").transform.position.x)) + 40 || transform.position.y < - 40 || transform.position.y > 80){
			respawn();
		}
	}

	void FixedUpdate () {
		if (!stunned) {
			movementKeyDown = false;
			if (controlMe) {
				if (!ledgeHanging){
					if (Input.GetKey (keyLeft)) {
						move(true, 1);
					}
					if (Input.GetKey (keyRight)) {
						move(false, 1);
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
				}
			}
			// AI !!!
			else {
				// Find all players
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject player in players) {
					if (player.name != gameObject.name) {
						//Track to this player if it's not near the edge of the platform
						if (player.transform.position.x < GameObject.Find ("Ground/LedgeGrabRight").transform.position.x && player.transform.position.x > GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x){
							if (!((transform.position.x > GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - 5 && transform.rotation.y == 0) || (transform.position.x < 5 && transform.rotation.y == 0))) {
								move(player.transform.position.x > transform.position.x ? false : true, 1);
							}
							else{
								move(player.transform.position.x > transform.position.x ? false : true, (transform.rotation.y == 0) ? (GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - transform.position.x) / (carefulness * 1) : (transform.position.x - GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x) / (carefulness * 1));
							}
						}
						//Consider attacking
						if (Mathf.Abs(player.transform.position.x - transform.position.x) < Random.Range(1f, 4f)) {
							attack();
						}
						//Decide whether or not to take a chance and jump (this is not pathing jumping)
						if (Random.Range(0,500) == 0){
							jump();
						}
						//Decide whether or not to drop off ledge
						if (ledgeHanging && Random.Range(0,50) == 0) {
							crouch(true);
						}
						else if (ledgeHanging && Random.Range(0,50) == 0) {
							jump();
						}
					}
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

	private void respawn () {
		transform.position = new Vector3 (0, 5, 0);
		rigidbody.velocity = new Vector3(0, 0, 0);
		damage = 0;
		stunned = false;
		touchingGround = false;
		doubleJumpUsed = false;
		ledgeHanging = false;
		touchingEnemy = null;
		currentAttack = null;
	}

	private void move (bool left, float speed) {
		rigidbody.AddForce(new Vector3(movementForce * (left ? -1 : 1) * speed,0,0));
		transform.rotation = Quaternion.Euler( 0, (left ? 180 : 0), 0);
		movementKeyDown = true;
	}

	private void jump () {
		if (!ledgeHanging) {
			rigidbody.velocity = new Vector3(rigidbody.velocity.x,0,0);
			rigidbody.AddForce(new Vector3(0,jumpForce,0),ForceMode.Force);
			if (!touchingGround)
				doubleJumpUsed = true;
		}
		else{
			rigidbody.AddForce(0, jumpForce, 0);
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
			ledgeHanging = false;
		}
	}

	private void crouch (bool down) {
		if (!ledgeHanging){
			if (down)
				transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
			else
				transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		}
		else {
			stunned = true;
			stunnedCountDown = 15;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
			ledgeHanging = false;
		}
	}

	private void attack () {
		if (currentAttack == null)
			currentAttack = new Player.Attack(this.gameObject);
	}

	public void hurt (float damage, Vector3 source) {
		this.damage += damage;
		if (damage > 20) {
			stunned = true;
			stunnedCountDown = (int) damage;
		}
		rigidbody.AddForce (movementForce * this.damage * (damage / 10) * ((source.x > 0) ? -1 : 1), jumpForce / 40 * this.damage, 0);
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
