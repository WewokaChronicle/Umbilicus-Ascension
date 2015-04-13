using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class JetSpecial : MonoBehaviour {

	// Jet Force power!
	public const float FORCE = 75.0f;

	// Cooldown Attributes
	private const float COOLDOWN_TIME = 2f;
	private float cooldownTimer;

	// Cooldown Slider
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;
	private Color offColor;

	// Jet SFX
	public AudioClip specialSound;

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionAvailable = true;

	// Init
	public void Start() {
		this.player = GetComponent<Player>();
		if(this.player.inGame) {
			this.cooldownSlider = player.cooldownSlider;
			this.cooldownFillImage = this.cooldownSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
			this.origColor = this.cooldownFillImage.color;
			this.offColor = this.origColor * Color.gray;
			this.inputDevice = this.player.inputDevice;
		}
	}

	// Called every frame
	public void Update() {

		// Action
		this.actionOn = inputDevice.Action1;

		// Cooldown
		if(this.cooldownTimer > 0f) {
			this.cooldownTimer -= Time.deltaTime;
			this.cooldownSlider.value = 1f - (this.cooldownTimer / COOLDOWN_TIME);
		} 

		else {
			this.cooldownSlider.value = 1f;
			if(!this.actionAvailable && this.player.IsOnTheGround()) {
				this.actionAvailable = true;
				this.cooldownFillImage.color = this.origColor;
			}
		}
	}

	// Called at a fixed framerate
	public void FixedUpdate() {
		if(this.actionOn && this.actionAvailable && this.cooldownTimer <= 0f) {
			// Sound
			Sound_Manager.Instance.PlayEffectOnce(specialSound);

			// Jet up force
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * FORCE, ForceMode2D.Impulse);

			// Cooldown
			this.cooldownTimer = COOLDOWN_TIME;
			
			this.cooldownFillImage.color = this.offColor;
			this.actionAvailable = false;

			// Animation
			this.player.spriteAnimator.SetTrigger("triggerBoost");
		}
	}

	/// <summary>
	/// Disables the JetSpecial
	/// </summary>
	public void DisableSpecial() {
		enabled = false;
	}
}

