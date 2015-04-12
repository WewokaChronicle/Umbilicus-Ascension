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
	public const float FORCE = 50.0f;
	public const float MAX_SPEED = 50.0f;

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
	public AudioClip[] specialSounds;
	public AudioClip[] goreSounds;
	public AudioClip[] deathSounds;
	
	// Input
	public int playerNumber;
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
	private BoxCollider2D boxCollider2D;

	// This is called before Start
	void Awake() 
	{
		// Get the player number controlling this character
		this.playerNumber = CharacterManager.selectedCharacters[this.characterID];

		// If this character hasn't been assigned a player number, destroy it
		if(this.playerNumber == CharacterManager.UNASSIGNED) {
			this.cooldownSlider.gameObject.SetActive(false);
			this.inGame = false;
			Destroy(this.gameObject);

		}

		else {
			this.inGame = true;

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
		this.inputDevice = (InputManager.Devices.Count > playerNumber && PlayerControl.NumberOfPlayers > playerNumber) ? InputManager.Devices[playerNumber] : null;
	}
	
	// Update is called once per frame
	void Update()
	{
		// End Game
		if(this.transform.position.y < -5f) {
//			GameManager.instance.EndLevel();
			Debug.Log("You lose!");
		} else if(this.transform.position.y > 70f) {
//			GameManager.instance.EndLevel();
			Debug.Log("You lose!");
		}
		
		// Don't do anything if we're dead
		if(this.isDead) {
			return;
		}

		// Input
		this.UpdateInput(this.inputDevice);

		// Reposition Slider over us
		this.RepositionSlider(this.cooldownSlider);
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
		corpse.GetComponent<SpriteRenderer>().sprite = this.deadTorsoSprite;

		// --- Player modifications ---
		// change the sprite to dead legs
		this.spriteRenderer.sprite = this.deadLegsSprite;
		this.transform.position += new Vector3(0f, -deadLegsSprite.bounds.size.y);

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
	}

	/// <summary>
	/// Auxiliary method for handling respositioning the slider bar above the player.
	/// </summary>
	private void RepositionSlider(Slider cooldownSlider) 
	{
		cooldownSlider.transform.position = this.transform.position + Vector3.up * 1f;
		if(this.transform.position.y > 49f || this.transform.position.y < 3f || Time.timeScale == 0f) {
			cooldownSlider.gameObject.SetActive(false);
		}

		else {
			cooldownSlider.gameObject.SetActive(true);
		}
	}
}




