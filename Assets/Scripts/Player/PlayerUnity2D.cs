using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

/// <summary>
/// Reimplementation of the Player.cs that uses Unity2D's native tools.
/// </summary>
[RequireComponent (typeof (Animator))]
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
	private float velX;
	private Vector2 velocity;
	private RaycastHit2D hit;
	
	void Awake() 
	{
		this.cam = Camera.main;
		this.spriteAnimator = GetComponent<Animator>();

		// Get the input device corresponding to the player number
//		this.inputDevice = (InputManager.Devices.Count > playerNum && PlayerControl.NumberOfPlayers > playerNum) ? InputManager.Devices[playerNum] : null;
//		Debug.Log(InputManager.Devices.Count);
//		this.inputDevice = InputManager.Devices[playerNum];
		this.inputDevice = InputManager.ActiveDevice;
		Debug.Log(this.inputDevice.Name);
		if(this.inputDevice == null) {
			//this.cooldownSlider.gameObject.SetActive(false);
			// If no controller exists for this player, destroy it
			Destroy(gameObject);
		} 

		else {
			inGame = true;
		}
	}


	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		//UpdateInput(this.inputDevice);
	}

	void FixedUpdate()
	{
		// Check float vs. stand
		this.hit = Physics2D.Raycast(this.transform.position, -Vector2.up, 3f, this.stickMask);
		
		if(this.hit.collider != null) { // if we hit something, then there is ground underneath
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", true);
		}
		
		else { // otherwise, we're not above anything
			this.spriteAnimator.SetBool("thereIsGroundUnderneath", false);
		}
	}


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
			this.transform.localScale = -this.transform.localScale;
		}

		// if we're moving left and we are facing right,
		else if(this.forceX < 0.0f && this.transform.localScale.x > 0) {
			// invert!
			this.transform.localScale = -this.transform.localScale;
		}



	}
}




