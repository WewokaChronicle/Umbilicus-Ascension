using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class RockSpecial : MonoBehaviour {
	public const float FORCE = 80f;
	
	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionAvailable = true;
	public bool rocking = false;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;
	private Color offColor;

	// Rocking
	private const float ROCK_TIME = 1f;
	private float rockTimer;

	// Leftover force timer
	private const float COOLDOWN_TIME = 2f;
	private float cooldownTimer;

	// Special SFX
	public AudioClip specialSound;
	
	public void Start() {
		this.player = GetComponent<Player>();
		if(this.player.inGame) {
			this.cooldownSlider = this.player.cooldownSlider;
			this.cooldownFillImage = this.cooldownSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
			this.origColor = this.cooldownFillImage.color;
			this.offColor = this.origColor * Color.gray;
			this.inputDevice = this.player.inputDevice;
		}
	}

	public void Update() {
		// Action
		this.actionOn = this.inputDevice.Action1;
		// Cooldown
		if(this.cooldownTimer > 0f) {
			this.cooldownTimer -= Time.deltaTime;
			this.cooldownSlider.value = 1f - (this.cooldownTimer / COOLDOWN_TIME);
		} 

		else {
			this.cooldownSlider.value = 1f;
			if(!this.actionAvailable) {
				this.actionAvailable = true;
				this.cooldownFillImage.color = origColor;
			}
		}

		// Rock action
		if(this.rockTimer > 0f) {
			this.rockTimer -= Time.deltaTime;
		} 

		else if(this.rocking) {
			this.rocking = false;
			this.player.spriteAnimator.SetTrigger("smashHit");
		}
	}

	public void FixedUpdate() {

		if(this.actionOn && this.cooldownTimer <= 0f) {

			// Cooldown
			this.cooldownTimer = COOLDOWN_TIME;
			this.cooldownFillImage.color = offColor;
			this.actionAvailable = false;

			// Turn on rock
			this.rocking = true;
			this.rockTimer = ROCK_TIME;

			// Anim
			this.player.spriteAnimator.SetTrigger("smashStart");

			// Sound
			Sound_Manager.Instance.PlayEffectOnce(specialSound);

			// Initial impulse force
			GetComponent<Rigidbody2D>().AddForce(-Vector2.up * FORCE * 0.5f, ForceMode2D.Impulse);
		}

		if(this.rocking) {
			// Jet down force
			GetComponent<Rigidbody2D>().AddForce(-Vector2.up * FORCE, ForceMode2D.Force);
		}
	}

	public void DisableSpecial() {
		this.enabled = false;
	}

	public void Destroy() {
		Destroy(this.gameObject);
	}
}

