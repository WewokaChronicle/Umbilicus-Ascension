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
//			cooldownSlider = player.cooldownSlider;
//			cooldownFillImage = cooldownSlider.transform.FindChild("Fill Area").GetComponentInChildren<Image>();
//			origColor = cooldownFillImage.color;
			this.inputDevice = player.inputDevice;

			// Collider offsets
			BoxCollider2D coll = GetComponent<BoxCollider2D>();
			this.bottomRightOffset = new Vector2(coll.size.x / 2f * 1.2f, coll.size.y / 2f * 1.2f);
		}
	}

	public void Update() 
	{
		// Action
		if(!actionDisabled && floatPower > (MAX_STICK_POWER * 0.1f)) {
			if(!actionOn && inputDevice.Action1) {
				// Turn on
				Collider2D coll = Physics2D.OverlapArea((Vector2) transform.position - bottomRightOffset, (Vector2) transform.position + bottomRightOffset, player.stickMask);
				if(coll != null) {
					actionOn = true;
					Sound_Manager.Instance.PlayEffectOnce(specialSound);

					this.player.spriteAnimator.SetTrigger("grabStart");
					this.player.spriteAnimator.SetTrigger("grabLoopStart");
					this.player.spriteAnimator.SetBool("isGrabbing", true);


					// Anim
//					player.spAnim.Play("GrabStart");
//					player.spAnim.AnimationCompleted += delegate(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip) {
//						player.spAnim.Play("Grab");
//					};
				}
			} else if(actionOn && !inputDevice.Action1) {
				// Turn off
				actionDisabled = true;
			}
			if(actionOn) {
				floatPower -= Time.deltaTime;
			}
		} else {
			if(!actionDisabled) {
				actionDisabled = true;
//				cooldownFillImage.color = Color.red;
			}
			if(actionOn) {
				this.player.spriteAnimator.SetBool("isGrabbing", false);
			}
			actionOn = false;
		}
		// Un-disable
		if(actionDisabled && floatPower > (MAX_STICK_POWER * 0.9f)) {
			actionDisabled = false;
//			cooldownFillImage.color = origColor;
		}
		// Recover sticky power
		if(!actionOn) {
			floatPower += Time.deltaTime * 1.5f;
			floatPower = Mathf.Min(floatPower, MAX_STICK_POWER);
		}
		// Display remaining sticky power
//		cooldownSlider.value = floatPower / MAX_STICK_POWER;
	}
	
	public void FixedUpdate() {
		if(actionOn) {
			// Stick in place
			GetComponent<Rigidbody2D>().velocity = Vector2.up * Block.SPEED;
		}
	}

	public void DisableSpecial() {
		enabled = false;
	}
}

