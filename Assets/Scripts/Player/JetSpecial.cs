using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class JetSpecial:MonoBehaviour {

	// Jet Force power!
	public const float FORCE = 100f;

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionAvailable = true;

	// Jet SFX
	public AudioClip specialSound;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;
	private Color offColor;

	// Leftover force timer
	private const float COOLDOWN_TIME = 3f;
	private float cooldownTimer;
	
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
			if(!this.actionAvailable) {
				this.actionAvailable = true;
				this.cooldownFillImage.color = this.origColor;
			}
		}
	}

	public void FixedUpdate() {
		if(this.actionOn && this.cooldownTimer <= 0f) {
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

	public void DisableSpecial() {
		enabled = false;
	}
}

