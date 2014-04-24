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

	private GameObject touchingEnemy = null;

	private Player.Attack currentAttack = null;

	private bool movementKeyDown = false;

	private List<Rect> jumpZones = new List<Rect> ();



	// Use this for initialization
	void Start () {
		GameObject[] ledgeGrabs = GameObject.FindGameObjectsWithTag ("LedgeGrab");

		for (int i = 0; i < ledgeGrabs.Length; i++) {
			if (ledgeGrabs[i].name == "LedgeGrabLeft") {
				jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x - 7.5f, ledgeGrabs[i].transform.position.y, 7.5f, 7.5f));
				jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x, ledgeGrabs[i].transform.position.y + 7.5f, 7.5f, 7.5f));
			}
			else {
				jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x, ledgeGrabs[i].transform.position.y, 7.5f, 7.5f));
				jumpZones.Add (new Rect(ledgeGrabs[i].transform.position.x - 7.5f, ledgeGrabs[i].transform.position.y + 7.5f, 7.5f, 7.5f));
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
				if (_ledge != null && rigidbody.velocity.y <= 0 && _ledge.tag == "LedgeGrab") {
					rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					rigidbody.velocity = new Vector3(0, 0, 0);
					ledgeHanging = true;
					transform.position = new Vector3(_ledge.transform.position.x + (_ledge.transform.forward.x * 3000000), _ledge.transform.position.y, 0);
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
				// Find all players
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject player in players) {
					if (player.name != gameObject.name) {
						//Track to this player if it's not near the edge of the platform
						if (player.transform.position.x < GameObject.Find ("Ground/LedgeGrabRight").transform.position.x && player.transform.position.x > GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x){
							//prevent AI from getting stuck above player
							if (Mathf.Abs(player.transform.position.y - transform.position.y) < 0.5 || player.transform.position.y >= transform.position.y) {
								//prevent AI from getting stuck below player
								//if (transform.position.y - player.transform.position.y < 5) {
								//	move (false, 0.5f);
								//}
								//else {
									if (!((transform.position.x > GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - 5 && transform.rotation.y == 0) || (transform.position.x < 5 && transform.rotation.y == 0))) {
										move(player.transform.position.x > transform.position.x ? false : true, 1);
									}
									else{
										move(player.transform.position.x > transform.position.x ? false : true, (transform.rotation.y == 0) ? (GameObject.Find ("Ground/LedgeGrabRight").transform.position.x - transform.position.x) / (carefulness * 1) : (transform.position.x - GameObject.Find ("Ground/LedgeGrabLeft").transform.position.x) / (carefulness * 1));
									}
								//}
							}
							else if (touchingGround) {
								move (true, 0.5f);
							}
						}
						//Consider attacking
						if (Mathf.Abs(player.transform.position.x - transform.position.x) < Random.Range(1f, 4f)) {
							attack();
						}
						//Decide whether or not to take a chance and jump (this is not pathing jumping)
<<<<<<< HEAD
						if (Random.Range(0,2) == 0){
							jump();
=======
						//if (Random.Range(0,500) == 0){
						//	jump();
						//}
						//print (player.transform.position.x);// PLAYER

					//	if(transform.position.x
						if (Mathf.Abs(player.transform.position.y - transform.position.y) > 0.5) {
							for(int i = 0; i < jumpZones.Count; i++) {
								//if (rectangleCollision(new Rect (transform.position.x - 0.5f, transform.position.y - 0.5f, 1, 1),jumpZones[i],1)) {
								if (transform.position.x > jumpZones[i].x && transform.position.x < jumpZones[i].x + jumpZones[i].width && transform.position.y > jumpZones[i].y - jumpZones[i].height && transform.position.y < jumpZones[i].y) {
									jump();
									break;
								}
								if (!doubleJumpUsed && !touchingGround) {
									jump();
									break;
								}
							}
>>>>>>> origin/Alpha-Vik
						}

					//	print (GameObject.Find ("Step").transform.position.y + " " + GameObject.Find ("Step2").transform.position.y + " " + transform.position.y + " " + player.transform.position.y);


						//Decide whether or not to drop off ledge
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
