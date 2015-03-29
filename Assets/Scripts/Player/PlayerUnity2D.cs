using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

/// <summary>
/// Reimplementation of the Player.cs that uses Unity2D's native tools.
/// </summary>
[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (SpriteRenderer))]
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
	public SpriteRenderer spriteRenderer;

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

	// This player's Special Script
	public GameObject specialScript;

	// Particles
	public ParticleSystem bloodParticles;

	// Physics
	private float forceX;
	private float velocityX;
	private Vector2 velocity;
	private RaycastHit2D hit;
	private Rigidbody2D rigidbod2D;
	private BoxCollider2D boxCollider2D;

	// This is called before Start
	void Awake() 
	{
		this.cam = Camera.main;
		this.boxCollider2D = GetComponent<BoxCollider2D>();
		this.rigidbod2D = GetComponent<Rigidbody2D>();
		this.spriteAnimator = GetComponent<Animator>();
		this.spriteRenderer = GetComponent<SpriteRenderer>();



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
			//GameManager.instance.EndLevel();
			Debug.Log("You lose!");
		} else if(this.transform.position.y > 70f) {
			//GameManager.instance.EndLevel();
			Debug.Log("You lose!");
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
			this.rigidbod2D.AddForce(Vector2.right * this.forceX, ForceMode2D.Force);
		}

		// Check float vs. stand
		this.hit = Physics2D.Raycast(this.transform.position, -Vector2.up, 3f, this.stickMask);
		// Debug.Log(this.hit.collider);
		// Debug.DrawRay(this.transform.position,-Vector3.up);
		
		if(this.hit.collider != null) { // if we hit something, then there is ground underneath
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", true);
		}
		
		else { // otherwise, we're not above anything
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", false);
		}

	}

	/// <summary>
	/// Kill this instance and spawn this player's corpse.
	/// </summary>
	public void Kill() 
	{
		if(this.isDead) {
			return; // do nothing.
		}

		this.isDead = true;

		// Spawn Corpse
		GameObject corpse = ((GameObject) Instantiate(this.corpsePrefab, this.transform.position, this.transform.rotation));
		Sprite corpseSprite = Resources.Load<Sprite>("Sprites/Player/PlayerBlueDead");
		corpse.GetComponent<SpriteRenderer>().sprite = corpseSprite;

		// --- Player modifications ---
		// change the sprite to dead legs
		Sprite deadLegsSprite = Resources.Load<Sprite>("Sprites/Player/PlayerBlueDeadLegs");
		this.spriteRenderer.sprite = deadLegsSprite;
		this.transform.position += new Vector3(0f, -deadLegsSprite.bounds.size.y);

		// adjust this Sprite's collider to dead legs size, since they remain attached to the chain
		this.boxCollider2D.size = deadLegsSprite.bounds.size;
		this.rigidbod2D.fixedAngle = false; // dead legs can be chaotic!

		// destroy the animator, since dead legs don't animate
		Destroy(this.spriteAnimator);

		// blood!
		this.bloodParticles.Play();
	}

	/// <summary>
	/// Auxiliary method for handling input detection.
	/// </summary>
	private void UpdateInput(InputDevice inputDevice)
	{
		if(Time.timeScale == 0f) {
			return; //we've effectively paused, so there's nothing to update
		}

		bool actionOn = inputDevice.Action1;
		if(actionOn) {
			this.Kill();
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
	}
}




