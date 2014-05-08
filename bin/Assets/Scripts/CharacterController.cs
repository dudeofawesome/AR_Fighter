using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Character/Generic Character Controller")]
public class CharacterController : MonoBehaviour {

	public bool controlMe = false;

	public int jumpForce = 500000;
	public int movementForce = 10000;
	public float drag = 0.15f;

	public GameObject feet = null;
	public GameObject attackHitbox = null;
	public GameObject ledgeGrabHitbox = null;
	public GameObject sceneCamera = null;

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
	private MyPlayer.Attack currentAttack = null;
	private bool movementKeyDown = false;
	private float heightOfFirstJump = 0;
	private List<custTypes.JumpZone> jumpZones = new List<custTypes.JumpZone> ();
	private float jumpZoneWidth = 9.0f;
	private float jumpZoneHeight = 7.5f;
	public float maxJumpHeight;



	// Use this for initialization
	void Start () {
		GameObject[] ledgeGrabs = GameObject.FindGameObjectsWithTag ("LedgeGrab");

		for (int i = 0; i < ledgeGrabs.Length; i++) {
			if (ledgeGrabs[i].name == "LedgeGrabLeft") {
				jumpZones.Add (new custTypes.JumpZone(ledgeGrabs[i].transform.position.x - jumpZoneWidth, ledgeGrabs[i].transform.position.y, jumpZoneWidth, jumpZoneHeight, true));
				if (ledgeGrabs[i].transform.position.y >= 1)
					jumpZones.Add (new custTypes.JumpZone(ledgeGrabs[i].transform.position.x, ledgeGrabs[i].transform.position.y + jumpZoneHeight, jumpZoneWidth, jumpZoneHeight, false));
			}
			else {
				jumpZones.Add (new custTypes.JumpZone(ledgeGrabs[i].transform.position.x, ledgeGrabs[i].transform.position.y, jumpZoneWidth, jumpZoneHeight, false));
				if (ledgeGrabs[i].transform.position.y >= 1)
					jumpZones.Add (new custTypes.JumpZone(ledgeGrabs[i].transform.position.x - jumpZoneWidth, ledgeGrabs[i].transform.position.y + jumpZoneHeight, jumpZoneWidth, jumpZoneHeight, true));
			}
		} 

		maxJumpHeight = calculateJumpHeight ();
	}

	// Update is called once per frame
	void Update () {
		if (!stunned){
			if (currentAttack == null) {
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
			if (currentAttack == null) {
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
//						float _tilt = Mathf.Clamp (Input.acceleration.x * 4, -1, 1);
						float _tilt = (sceneCamera.transform.position.z > 1 ? -1 : 1) * Mathf.Clamp (Input.acceleration.x * 4, -1, 1);
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
							// AI isn't ledge grabbing
							if (!ledgeHanging) {
								// AI isn't off the edge
								if (transform.position.x > GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x && transform.position.x < GameObject.Find ("Ground/LedgeGrabRight").transform.position.x) {
									// AI is above player
									if (transform.position.y > player.transform.position.y + 5) {
										// print("going off of platform edge");
										move(Random.Range(0, 1) == 0 ? false : true, 0.7f);
									}

									// AI is below player
									if (transform.position.y + 5 < player.transform.position.y) {
										// print("going towards jump zone");
										float closestJumpZoneX = -1f;
										float closenessOfJumpZoneX = -1f;
										for(int i = 0; i < jumpZones.Count; i++) {
											if (Mathf.Abs((jumpZones[i].y - jumpZones[i].height) - transform.position.y) < 2f && Mathf.Abs(player.transform.position.x - (jumpZones[i].x + jumpZones[i].width)) < closenessOfJumpZoneX || closenessOfJumpZoneX == -1f) {
												closestJumpZoneX = (jumpZones[i].x + jumpZones[i].width);
												closenessOfJumpZoneX = Mathf.Abs(transform.position.x - closestJumpZoneX);
											}
										}
										move((transform.position.x > closestJumpZoneX) ? true : false, 1);
										if (transform.position.x > closestJumpZoneX && transform.position.x < closestJumpZoneX + jumpZoneWidth) {
											// print("jumping in jump zone");
											jump();
											heightOfFirstJump = transform.position.y;
										}
									}

									// AI on same level
									if (Mathf.Abs(transform.position.y - player.transform.position.y) < 5) {
										// print("running to player");
										if (transform.position.x > GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x + 2 && transform.position.x < GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - 2) {
											move(player.transform.position.x > transform.position.x ? false : true, 1);
										}
										else {
											move(player.transform.position.x > transform.position.x ? false : true, (transform.rotation.y == 0) ? (GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - transform.position.x) / (carefulness * 1) : (transform.position.x - GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x) / (carefulness * 1));
										}
									}

									// Double jump if needed
									if (transform.position.y < heightOfFirstJump + maxJumpHeight - Random.value && transform.position.y < player.transform.position.y && !touchingGround) {
										// print("trying to double jump");
										jump();
									}
								}
								// Run back to ledge
								else {
									jump();
									move(transform.position.x < GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x + 2 ? false : true, 1);
								}
								// Consider attacking
								if (Mathf.Abs(player.transform.position.x - transform.position.x) < Random.Range(1f, 4f) && Mathf.Floor(Random.value * 10) == 0) {
									attack();
								}
							}
							else {
								// Decide whether or not to drop off ledge
								if (player.transform.position.y < transform.position.y && Random.Range(0,30) == 0) {
									crouch(true);
								}
								else if (player.transform.position.y > transform.position.y && !touchingGround && Random.Range(0,30) == 0) {
									jump();
								}
							}
						}
					}
				}
			}

			// Drag
			if (currentAttack != null) {
				rigidbody.velocity = new Vector3 (rigidbody.velocity.x * (0.5f), rigidbody.velocity.y, 0);
			}
			else if (!movementKeyDown && touchingGround) {
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

	float calculateJumpHeight(){
		//Using derivitives to calculate Jump Height of the Artifical Intelligence
		//Here we are given the Force of the Object that the RigidBody jumps with
		//We can use this and the formula F=MA to calculate accelration 
		//dy/dt((g*t^2)/2)+((f*t)/m) = (F/M)*(g*t)
		
		//Calculate when slope = 0
		float t = -(jumpForce / rigidbody.mass) / Physics.gravity.y;
		//Find the y value at t
		return (Physics.gravity.y * Mathf.Pow(t / 6000, 2) / 2) + ((jumpForce * t / 6000)/ rigidbody.mass);
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
			transform.position = new Vector3(transform.position.x , transform.position.y + 1, 0);
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
			currentAttack = new MyPlayer.Attack(this.gameObject);
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
