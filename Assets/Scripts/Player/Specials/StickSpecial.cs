using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

[RequireComponent (typeof (Player))]
public class StickSpecial : MonoBehaviour {

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionDisabled = false;

	// Sound
	public AudioClip specialSound;

	// Collider offsets
	private Vector2 bottomRightOffset;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;

	// Stick power
	private const float MAX_STICK_POWER = 1f;
	private float floatPower = MAX_STICK_POWER;

	// Wall to grip onto
	private Rigidbody2D gripWall;

	// Physics
	private float velY;
	private Vector2 velocity;

	public void Start() 
	{
		this.player = GetComponent<Player>();
		if(this.player.inGame) {

			this.inputDevice = player.inputDevice;

			// Setup the slider
			this.cooldownSlider = this.player.cooldownSlider;
			this.cooldownFillImage = this.cooldownSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
			this.origColor = cooldownFillImage.color;

			// Collider offsets
			BoxCollider2D coll = GetComponent<BoxCollider2D>();
			this.bottomRightOffset = new Vector2(coll.size.x / 2f * 1.2f, coll.size.y / 2f * 1.2f);
		}
	}

	public void Update() 
	{
		// Action
		if(!this.actionDisabled && this.floatPower > (MAX_STICK_POWER * 0.1f)) {
		
			if(!this.actionOn && this.inputDevice.Action1) {
				// Turn on

				// Get the bounding box of this game object
				Vector2 boundingBoxTopLeftCoordinate = ((Vector2) this.transform.position - this.bottomRightOffset);
				Vector2 boundingBoxBottomRightCoordinate = ((Vector2) this.transform.position + this.bottomRightOffset);

				// Check for anything colliding with this bounding box
				Collider2D coll = Physics2D.OverlapArea(boundingBoxTopLeftCoordinate, boundingBoxBottomRightCoordinate, this.player.stickMask);

				if(coll != null) { // we're colliding with the environment
					this.actionOn = true;
					Sound_Manager.Instance.PlayEffectOnce(specialSound);

					this.player.spriteAnimator.SetTrigger("grabStart");
					this.player.spriteAnimator.SetTrigger("grabLoopStart");
					this.player.spriteAnimator.SetBool("isGrabbing", true);
				}
			} 

			else if(this.actionOn && !this.inputDevice.Action1) {
				this.actionDisabled = true; // Turn off
			}

			if(this.actionOn) {
				this.floatPower -= Time.deltaTime;
			}
		} 

		else {

			if(!this.actionDisabled) {
				this.actionDisabled = true;
				this.cooldownFillImage.color = Color.red;
			}

			if(this.actionOn) {
				this.player.spriteAnimator.SetBool("isGrabbing", false);
			}

			this.actionOn = false;
		}

		// Un-disable
		if(this.actionDisabled && this.floatPower > (MAX_STICK_POWER * 0.9f)) {
			this.actionDisabled = false;
			this.cooldownFillImage.color = origColor;
		}

		// Recover sticky power
		if(!actionOn) {
			this.floatPower += Time.deltaTime * 1.5f;
			this.floatPower = Mathf.Min(this.floatPower, MAX_STICK_POWER);
		}

		// Display remaining sticky power
		this.cooldownSlider.value = this.floatPower / MAX_STICK_POWER;
	}
	
	public void FixedUpdate() {
		if(this.actionOn) {
			// Stick in place
			GetComponent<Rigidbody2D>().velocity = Vector2.up * Block.SPEED;
		}
	}

	public void DisableSpecial() {
		this.enabled = false;
	}

	public void Destroy() {
		Destroy(this.gameObject);
	}
}

