using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public int jumpForce = 500000;
	public int movementForce = 10000;
	public float drag = 0.3f;

	public int health = 100;

	public string keyUp = "w";
	public string keyLeft = "a";
	public string keyDown = "s";
	public string keyRight = "d";
	public string keyAttack = "space";


	private bool touchingGround = false;
	private bool doubleJumpUsed = false;

	private Player.Attack currentAttack = null;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (keyUp) && !doubleJumpUsed) {
			rigidbody.velocity = new Vector3(rigidbody.velocity.x,0,0);
			rigidbody.AddForce(new Vector3(0,jumpForce,0),ForceMode.Force);
			if (!touchingGround)
				doubleJumpUsed = true;
			touchingGround = false;
		}
		if (Input.GetKeyDown (keyDown)) {
			transform.localScale = new Vector3(4,1,4);
		}
		if (Input.GetKeyUp (keyDown)) {
			transform.localScale = new Vector3(4,4,4);
		}
		if (Input.GetKeyDown (keyAttack) && currentAttack == null) {
			// TODO		change "this.gameObject" to the hitbox holder for the attack
			currentAttack = new Player.Attack(this.gameObject);
		}

		//apply max velocity
		if (rigidbody.velocity.x > 20f || rigidbody.velocity.x < -20f) 
			rigidbody.velocity = new Vector3 (Mathf.Clamp (rigidbody.velocity.x, -20f, 20f), rigidbody.velocity.y, 0);
	}

	void FixedUpdate () {
		if (Input.GetKey (keyLeft)) {
//			Player.transform.position = new Vector3(Player1.transform.position.x - 0.5f,Player1.transform.position.y,Player1.transform.position.z);
			rigidbody.AddForce(new Vector3(movementForce * -1,0,0));
		}
		if (Input.GetKey (keyRight)) {
//			Player.transform.position = new Vector3(Player.transform.position.x + 0.5f,Player.transform.position.y,Player.transform.position.z);
			rigidbody.AddForce(new Vector3(movementForce,0,0));
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

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.tag == "Terrain" || collision.gameObject.tag == "Player") {
			touchingGround = true;
			doubleJumpUsed = false;
		}
	}
}
