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
		cam = Camera.main;
		spriteAnimator = GetComponent<Animator>();
	}


	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Math.Abs(Input.GetAxis("Vertical")) != 0) {
			spriteAnimator.SetBool("thereIsGroundUnderneath", false);
		}

		else {
			spriteAnimator.SetBool("thereIsGroundUnderneath", true);
		}

	}
}




