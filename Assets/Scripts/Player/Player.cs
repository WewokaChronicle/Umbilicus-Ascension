using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

/// <summary>
/// The base class for all Players.
/// </summary>
[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class Player : MonoBehaviour
{
	public const float MOVEMENT_FORCE = 50.0f;
	public const float MAX_SPEED = 50.0f;
	public float jumpingForce = 10.0f;

	// This is this player's unique character ID.
	// Milkyway Mike = 0
	// Quasar Quade = 1
	// Stardust Stan = 2
	// Cosmonaut Carla = 3
	public int characterID;

	// Layer mask
	public LayerMask stickMask;

	// Sprites
	public Sprite deadTorsoSprite;
	public Sprite deadLegsSprite;

	[HideInInspector]
	public Sprite sprite;

	[HideInInspector]
	public Animator spriteAnimator;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	// Audio
	public AudioClip jumpSound;
	public AudioClip[] specialSounds;
	public AudioClip[] goreSounds;
	public AudioClip[] deathSounds;
	
	// Player properties
	public int playerNumber;
	public Slider cooldownSlider;

	[HideInInspector]
	public InputDevice inputDevice;

	[HideInInspector]
	public bool inGame = false;

	[HideInInspector]
	public bool isDead = false;

	[HideInInspector]
	public bool canJump = false;

	[HideInInspector]
	public bool playerOnTheEnd = false;
	
	// Corpse 
	public GameObject corpsePrefab;
	
	// Particles
	public ParticleSystem bloodParticles;

	// Physics
	private float forceX;
	private float velocityX;
	private Vector2 velocity;
	private Rigidbody2D rigidbod2D;
	private BoxCollider2D boxCollider2D;

	// This is called before Start
	void Awake() 
	{
		// start as out of the game
		this.inGame = false;

		// Get the player number controlling this character
		// if characters have been selected in the Menu Scene, overwrite the player number
		if(CharacterManager.selectedCharacters != null) {
			// this is inside an "if" guard so that I can test Scenes individually without
			// having to necessarily load the Menu Scene prior to testing this class
			this.playerNumber = CharacterManager.selectedCharacters[this.characterID];
		}

		// If this character hasn't been assigned a player number, destroy it
		if(this.playerNumber == CharacterManager.UNASSIGNED) {
			this.cooldownSlider.gameObject.SetActive(false);
			Destroy(this.gameObject);
		}

		else {
			this.inGame = true;
			this.isDead = false;
			this.canJump = true;

			// Attach a player special, depending on which space person it is
			if(this.characterID == CharacterManager.MILKYWAY_MIKE_INDEX) {
				this.gameObject.AddComponent<JetSpecial>().specialSound = this.specialSounds[this.characterID];
			}

			else if(this.characterID == CharacterManager.QUASAR_QUADE_INDEX) {
				this.gameObject.AddComponent<FloatSpecial>().specialSound = this.specialSounds[this.characterID];;
			}

			else if(this.characterID == CharacterManager.STARDUST_STAN_INDEX) {
				this.gameObject.AddComponent<StickSpecial>().specialSound = this.specialSounds[this.characterID];;
			} 

			else if(this.characterID == CharacterManager.COSMONAUT_CARLA_INDEX) {
				this.gameObject.AddComponent<RockSpecial>().specialSound = this.specialSounds[this.characterID];;
			}

			this._RepositionSlider(this.cooldownSlider, Vector3.up);
		}
	}

	// Init
	public void Start()
	{
		// Get handles on all necessary components
		this.boxCollider2D = GetComponent<BoxCollider2D>();
		this.rigidbod2D = GetComponent<Rigidbody2D>();
		this.spriteAnimator = GetComponent<Animator>();
		this.spriteRenderer = GetComponent<SpriteRenderer>();

		// Get the input device corresponding to the player number
		this.inputDevice = (InputManager.Devices.Count > playerNumber && 
		                    PlayerControl.NumberOfPlayers > playerNumber) ? 
			InputManager.Devices[playerNumber] : null;
	}
	
	// Update is called once per frame
	public void Update()
	{
		// We've jumped outside the screen.
		if(this._IsOutsideScreen()) {
			this.Kill();
		} 

		// We've ran out of oxygen
		if(OxygenTank.instance.isEmpty()) {
			this.Kill();
		}

		// Don't do anything if we're dead
		if(this.isDead) {
			return;
		}

		// Input
		this._UpdateInput(this.inputDevice);

		// Reposition the cooldown slider over us
		this._RepositionSlider(this.cooldownSlider, Vector3.up);
	}

	// FixedUpdate is called at a fixed framerate frame
	public void FixedUpdate()
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
		if(this.IsOnTheGround()) { 
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", true);
			this.canJump = true;
		}
		
		else { 
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", false);
			this.canJump = false;
		}

		// Jumping
		if(this.inputDevice.Direction.Up.WasPressed && this.canJump) {
			this.rigidbod2D.AddForce(Vector2.up * jumpingForce, ForceMode2D.Impulse);
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", false); // there won't be ground when we jump!
			this.canJump = false;
			Sound_Manager.Instance.PlayEffectOnce(this.jumpSound);
		}
	}
	
	/// <returns><c>true</c> if this instance is on the ground; otherwise, <c>false</c>.</returns>
	public bool IsOnTheGround() 
	{
		// if we hit something, then there is ground underneath
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -Vector2.up, 0.65f, this.stickMask);
		return (hit.collider != null);
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
		this.inGame = false;

		// Spawn Corpse
		GameObject corpse = ((GameObject) Instantiate(this.corpsePrefab, this.transform.position, this.transform.rotation));
		corpse.GetComponent<SpriteRenderer>().sprite = this.deadTorsoSprite;
		corpse.GetComponent<BoxCollider2D>().size = this.deadTorsoSprite.bounds.size;

		// --- Player modifications ---
		// change the sprite to dead legs
		this.spriteRenderer.sprite = this.deadLegsSprite;
		this.transform.position += new Vector3(0f, -deadLegsSprite.bounds.size.y);
		this.cooldownSlider.gameObject.SetActive(false);

		// Play a death grunt
		Sound_Manager.Instance.PlayEffectOnce(this.deathSounds[UnityEngine.Random.Range(0, 3)]);

		// Play a gore sound
		Sound_Manager.Instance.PlayEffectOnce(this.goreSounds[UnityEngine.Random.Range(0, 3)]);

		// Disable this player's Special (this method should exist in the Special script attached to this game object)
		SendMessage("DisableSpecial");

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
	private void _UpdateInput(InputDevice inputDevice)
	{
		if(Time.timeScale == 0f) {
			return; //we've effectively paused, so there's nothing to update
		}

		// Direction
		this.forceX = MOVEMENT_FORCE * inputDevice.Direction.X;
		if(this.playerOnTheEnd) {
			// this causes players at the end of the chain to have less 
			// ability to pull the entire group sideways
			this.forceX *= 0.75f; 
		}

		// Facing direction
		// if we're moving right and we are facing left,
		if(this.forceX > 0.0f && this.transform.localScale.x < 0) {
			// invert!
			Vector3 oldLocalScale = this.transform.localScale;
			Vector3 newLocalScale = new Vector3(-oldLocalScale.x, oldLocalScale.y, oldLocalScale.z);
			this.transform.localScale = newLocalScale;
			this.cooldownSlider.direction = Slider.Direction.LeftToRight;
		}

		// if we're moving left and we are facing right,
		else if(this.forceX < 0.0f && this.transform.localScale.x > 0) {
			Vector3 oldLocalScale = this.transform.localScale;
			Vector3 newLocalScale = new Vector3(-oldLocalScale.x, oldLocalScale.y, oldLocalScale.z);
			this.transform.localScale = newLocalScale;
			this.cooldownSlider.direction = Slider.Direction.RightToLeft;
		}
	}

	/// <summary>
	/// Auxiliary method for respositioning the slider bar relative to the player.
	/// </summary>
	private void _RepositionSlider(Slider slider, Vector3 direction) 
	{
		Debug.Log("RespositionSlider");
		slider.transform.position = this.transform.position + direction * 1f;
		if(this.isDead || Time.timeScale == 0f) {
			slider.gameObject.SetActive(false);
		}

		else {
			slider.gameObject.SetActive(true);
		}
	}

	/// <returns><c>true</c>, if this player is outside the screen, <c>false</c> otherwise.</returns>
	private bool _IsOutsideScreen()
	{
		return (this.transform.position.y < -6f || this.transform.position.y > 70f);
	}
}




