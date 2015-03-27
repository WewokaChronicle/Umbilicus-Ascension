using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

/// <summary>
/// Reimplementation of the Player.cs that uses Unity2D's native tools.
/// </summary>
[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (Rigidbody2D))]
public class PlayerUnity2D : MonoBehaviour
{
	public const float FORCE = 50.0f;
	public const float MAX_SPEED = 50.0f;


	// Camera
	private Camera cam;


	// Layer mask
	public LayerMask stickMask;


	// Sprites
	// public tk2dSpriteAnimation[] playerSprites;
	// private tk2dSprite sp;
	// [HideInInspector]
	// public tk2dSpriteAnimator spAnim;

	public Sprite sprite;
	public Animator spriteAnimator;


	// Audio
	public AudioClip[] specialSounds;
	public AudioClip[] goreSounds;
	public AudioClip[] deathSounds;


	// Input
	public int playerNum;
	public Slider cooldownSlider;

	[HideInInspector]
	public bool inGame = false;

	[HideInInspector]
	public bool playerOnTheEnd = false;

	[HideInInspector]
	public InputDevice inputDevice;

	[HideInInspector]
	public bool isDead = false;

	// Corpse 
	public GameObject corpsePrefab;

	// Particles
	public ParticleSystem bloodParticles;

	// Physics
	private float forceX;
	private float velocityX;
	private Vector2 velocity;
	private RaycastHit2D hit;
	private Rigidbody2D rigidbod2D;

	// This is called before Start
	void Awake() 
	{
		this.cam = Camera.main;
		this.spriteAnimator = GetComponent<Animator>();
		this.rigidbod2D = GetComponent<Rigidbody2D>();

		// Get the input device corresponding to the player number
		this.inputDevice = (InputManager.Devices.Count > playerNum && PlayerControl.NumberOfPlayers > playerNum) ? InputManager.Devices[playerNum] : null;
		if(this.inputDevice == null) {
			//this.cooldownSlider.gameObject.SetActive(false);
			// If no controller exists for this player, destroy it
			Destroy(gameObject);
		} 

		else {
			inGame = true;
		}

		// Actions (To be continued)

	}
	
	// Update is called once per frame
	void Update()
	{
		// End Game
		if(this.transform.position.y < -5f) {
			GameManager.instance.EndLevel();
		} else if(this.transform.position.y > 70f) {
			GameManager.instance.EndLevel();
		}
		
		// Don't do anything if we're dead
		if(isDead) {
			return;
		}

		// Input
		UpdateInput(this.inputDevice);
	}

	void FixedUpdate()
	{
		if(this.isDead) {
			return; //do nothing!
		}

		// Limit speed
		this.velocityX = this.rigidbod2D.velocity.x;

		if(this.velocityX >= MAX_SPEED && this.forceX > 0) {
			// Too fast, no more force will be applied!
		}

		else if(this.velocityX <= -MAX_SPEED && this.forceX < 0) {
			// Too fast in negative, no more force
		}

		else {
			// Move target left and right with input stick
			this.rigidbod2D.AddForce(Vector2.right * this.forceX, ForceMode2D.Impulse);
		}

	}

	/// <summary>
	/// Auxiliary method for handling input detection.
	/// </summary>
	private void UpdateInput(InputDevice inputDevice)
	{
		if(Time.timeScale == 0f) {
			return; //we've effectively paused, so there's nothing to update
		}

		// Direction
		this.forceX = FORCE * inputDevice.Direction.X;
		if(this.playerOnTheEnd) {
			this.forceX *= 0.75f;
		}

		// Facing direction
		// if we're moving right and we are facing left,
		if(this.forceX > 0.0f && this.transform.localScale.x < 0) {
			// invert!
			Vector3 oldLocalScale = this.transform.localScale;
			Vector3 newLocalScale = new Vector3(-oldLocalScale.x, oldLocalScale.y, oldLocalScale.z);
			this.transform.localScale = newLocalScale;
		}

		// if we're moving left and we are facing right,
		else if(this.forceX < 0.0f && this.transform.localScale.x > 0) {
			Vector3 oldLocalScale = this.transform.localScale;
			Vector3 newLocalScale = new Vector3(-oldLocalScale.x, oldLocalScale.y, oldLocalScale.z);
			this.transform.localScale = newLocalScale;
		}

		// Check float vs. stand
		this.hit = Physics2D.Raycast(this.transform.position, -Vector2.up, 3f, this.stickMask);
		Debug.Log(this.hit.collider);
		Debug.DrawRay(this.transform.position,-Vector3.up);
		
		if(this.hit.collider != null) { // if we hit something, then there is ground underneath
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", true);
		}
		
		else { // otherwise, we're not above anything
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", false);
		}
	}
}




