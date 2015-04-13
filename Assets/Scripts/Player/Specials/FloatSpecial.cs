using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class FloatSpecial : MonoBehaviour {
	public const float MAX_SPEED = 5f;
	public const float FORCE = 100f;

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionDisabled = false;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;

	// Float power
	private const float MAX_FLOAT_POWER = 1f;
	private float floatPower = MAX_FLOAT_POWER;
	public AudioClip specialSound;

	// Leftover force timer
	private const float WIND_DOWN_TIME = 0.2f;
	private float leftoverForceTimer;

	// Physics
	private float velY;
	private Vector2 velocity;

	public void Start() {
		this.player = GetComponent<Player>();
		if(this.player.inGame) {
			this.cooldownSlider = player.cooldownSlider;
			this.cooldownFillImage = this.cooldownSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
			this.origColor = this.cooldownFillImage.color;
			this.inputDevice = this.player.inputDevice;
		}
	}

	public void Update() {

		// If our action is not disabled and we have power available,
		if(!actionDisabled && floatPower > (MAX_FLOAT_POWER * 0.1f)) {

			// If we hadn't activated our power, and we now activated it
			if(!actionOn && inputDevice.Action1) {
				Sound_Manager.Instance.PlayEffectOnce(specialSound);
				this.player.spriteAnimator.SetBool("isJetting", true);
			} 

			// If we had activated our power, and we stopped
			else if(actionOn && !inputDevice.Action1) {
				this.player.spriteAnimator.SetBool("isJetting", false);
			}

			// Decrease power 
			actionOn = inputDevice.Action1;
			if(actionOn) {
				floatPower -= Time.deltaTime;
			}
		} 

		else {

			// if our action was not disabled, we must disable it.
			if(!actionDisabled) {
				actionDisabled = true;
				cooldownFillImage.color = Color.red;
			}

			// if we were floating, stop it!
			if(actionOn) {
				actionOn = false;
				this.player.spriteAnimator.SetBool("isJetting", false);
			}
		}

		// Undisable
		if(actionDisabled && floatPower > (MAX_FLOAT_POWER * 0.9f)) {
			actionDisabled = false;
			cooldownFillImage.color = origColor;
		}

		// Recover float power only if we're touching the ground
		if(!actionOn && this.player.IsOnTheGround()) {
			floatPower += 2*Time.deltaTime;
			floatPower = Mathf.Min(floatPower, MAX_FLOAT_POWER);
		}

		// Display remaining float power
		cooldownSlider.value = floatPower / MAX_FLOAT_POWER;
	}
	
	public void FixedUpdate() {
		if(actionOn) {
			// Limit falling speed
			velY = GetComponent<Rigidbody2D>().velocity.y;
			if(velY <= MAX_SPEED) {
				leftoverForceTimer = WIND_DOWN_TIME;
			}
			// "wind down" our counter force
			if(leftoverForceTimer > 0f) {
				GetComponent<Rigidbody2D>().AddForce(Vector2.up * FORCE * (MAX_SPEED - velY) * (leftoverForceTimer / WIND_DOWN_TIME), ForceMode2D.Force);
				leftoverForceTimer -= Time.deltaTime;
			}
		}
	}

	public void DisableSpecial() {
		enabled = false;
	}
}

