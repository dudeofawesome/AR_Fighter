using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public int ledgeGrabCountdown = 0;

	private GameObject touchingEnemy = null;
	private Player.Attack currentAttack = null;
	private bool movementKeyDown = false;
	private int timeSinceFirstJump = 0;
	private List<Rect> jumpZones = new List<Rect> ();



	// Use this for initialization
	void Start () {
		GameObject[] ledgeGrabs = GameObject.FindGameObjectsWithTag ("LedgeGrab");

		for (int i = 0; i < ledgeGrabs.Length; i++) {
			if (ledgeGrabs[i].name == "LedgeGrabLeft") {
				jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x - 9.0f, ledgeGrabs[i].transform.position.y, 9.0f, 7.5f));
				if (ledgeGrabs[i].transform.position.y >= 1)
					jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x, ledgeGrabs[i].transform.position.y + 7.5f, 9.0f, 7.5f));
			}
			else {
				jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x, ledgeGrabs[i].transform.position.y, 9.0f, 7.5f));
				if (ledgeGrabs[i].transform.position.y >= 1)
					jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x - 9.0f, ledgeGrabs[i].transform.position.y + 7.5f, 9.0f, 7.5f));
			}
		} 
	}

	// Update is called once per frame
	void Update () {
		if (!stunned){
			if (controlMe) {
				if (Input.GetKeyDown (keyUp)) {
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
					if (_touch.phase == TouchPhase.Began && (!doubleJumpUsed || ledgeHanging) && _touch.position.x > Screen.width / 2) {
						jump();
					}
					if (_touch.phase == TouchPhase.Began && currentAttack == null && _touch.position.x < Screen.width / 2) {
						attack();
					}
				}

			}

			if (!touchingGround) {
				GameObject _ledge = ledgeGrabHitbox.GetComponent<CollisionHandeler>().collidingWith;
				if (_ledge != null && rigidbody.velocity.y <= 0 && _ledge.tag == "LedgeGrab" && ledgeGrabCountdown == 0) {
					rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					rigidbody.velocity = new Vector3(0, 0, 0);
					ledgeHanging = true;
					transform.position = new Vector3(_ledge.transform.position.x + (_ledge.transform.rotation.eulerAngles.y == 0 ? 1 : -1), _ledge.transform.position.y - 4.5f, 0);
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

		// Boundaries
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
				// Find a player
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject player in players) {
					if (player.name != gameObject.name) {
						//Track to the player if it's not near the edge of the platform
						if (player.transform.position.x < GameObject.Find ("Ground/LedgeGrabRight").transform.position.x && player.transform.position.x > GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x){
							//prevent AI from getting stuck above player
							if (Mathf.Abs(player.transform.position.y - transform.position.y) < 1) {
								// print("going to the player (on the same lvl)");
								if (!((transform.position.x > GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - 5 && transform.rotation.y == 0) || (transform.position.x < 5 && transform.rotation.y == 0))) {
									move(player.transform.position.x > transform.position.x ? false : true, 1);
								}
								else{
									move(player.transform.position.x > transform.position.x ? false : true, (transform.rotation.y == 0) ? (GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - transform.position.x) / (carefulness * 1) : (transform.position.x - GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x) / (carefulness * 1));
								}
								//}
							}
							//Make AI go to nearest jump zone
							else if (player.transform.position.y > transform.position.y + 5) {
								// print("going towards jump zone");
								float closestJumpZoneX = -1f;
								float closenessOfJumpZoneX = -1f;
								for(int i = 0; i < jumpZones.Count; i++) {
									if (Mathf.Abs((jumpZones[i].y - jumpZones[i].height) - transform.position.y) < 2f && Mathf.Abs(player.transform.position.x - (jumpZones[i].x + jumpZones[i].width)) < closenessOfJumpZoneX || closenessOfJumpZoneX == -1f) {
										closestJumpZoneX = (jumpZones[i].x + jumpZones[i].width);
										closenessOfJumpZoneX = Mathf.Abs(transform.position.x - closestJumpZoneX);
									}
								}
								move ((transform.position.x > closestJumpZoneX) ? true : false, 1);
							}
							else /*if (touchingGround) */{
								// print("jumping to get on my level");
								move (true, 0.7f);
							}
						}
						// Consider attacking
						if (Mathf.Abs(player.transform.position.x - transform.position.x) < Random.Range(1f, 4f) && Mathf.Floor(Random.value * 10) == 0) {
							attack();
						}
						// Decide whether or not to take a chance and jump (this is not pathing jumping)
						//if (Random.Range(0,500) == 0){
						//	jump();
						//}
						// This is pathing jumping
						if (player.transform.position.y >= transform.position.y + 0.5) {
							timeSinceFirstJump++;
							for(int i = 0; i < jumpZones.Count; i++) {
								if (transform.position.x > jumpZones[i].x && transform.position.x < jumpZones[i].x + jumpZones[i].width && transform.position.y > jumpZones[i].y - jumpZones[i].height && transform.position.y < jumpZones[i].y && touchingGround) {
									jump();
									timeSinceFirstJump = 0;
									break;
								}
								else if (transform.position.x > jumpZones[i].x && transform.position.x < jumpZones[i].x + jumpZones[i].width && transform.position.y > jumpZones[i].y - jumpZones[i].height && transform.position.y < jumpZones[i].y && timeSinceFirstJump > 5) {
									jump();
									break;
								}
							}
						}
						
						// Decide whether or not to drop off ledge
						if (ledgeHanging && player.transform.position.y < transform.position.y && Random.Range(0,30) == 0) {
							crouch(true);
						}
						else if (ledgeHanging && player.transform.position.y > transform.position.y && Random.Range(0,30) == 0) {
							jump();
						}
						
						break;
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
		if (ledgeGrabCountdown > 0) {
			ledgeGrabCountdown--;
		}
	}

	bool rectangleCollision(Rect r1, Rect r2, int padding) {
		return !(r1.x > r2.x + r2.width + padding || r1.x + r1.width + padding < r2.x || r1.y > r2.y + r2.height + padding || r1.y + r1.height + padding < r2.y);
		// bool widthOverlap =  (r1.xMin >= r2.xMin) && (r1.xMin <= r2.xMax) || (r2.xMin >= r1.xMin) && (r2.xMin <= r1.xMax);
		// bool heightOverlap = (r1.yMin >= r2.yMin) && (r1.yMin <= r2.yMax) || (r2.yMin >= r1.yMin) && (r2.yMin <= r1.yMax);
		// return (widthOverlap && heightOverlap);
	}

	private void respawn () {
		GameObject.Find("GUI").GetComponent<GameGUI>().onPlayerDeath(gameObject);
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
		if (!ledgeHanging && !doubleJumpUsed) {
			rigidbody.velocity = new Vector3(rigidbody.velocity.x,0,0);
			rigidbody.AddForce(new Vector3(0,jumpForce,0),ForceMode.Force);
			if (!touchingGround)
				doubleJumpUsed = true;
		}
		else if (ledgeHanging) {
			rigidbody.AddForce(0, jumpForce, 0);
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
			transform.position = new Vector3(transform.position.x , transform.position.y + 5, 0);
			ledgeHanging = false;
			ledgeGrabCountdown = 10;
		}
	}

	private void crouch (bool down) {
		if (!ledgeHanging){
			if (down)
				transform.localScale = new Vector3(0.1f, 0.03f, 0.1f);
			else
				transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		}
		else {
			stunned = true;
			stunnedCountDown = 15;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
			transform.position = new Vector3(transform.position.x , transform.position.y - 2, 0);
			ledgeGrabCountdown = 10;
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
