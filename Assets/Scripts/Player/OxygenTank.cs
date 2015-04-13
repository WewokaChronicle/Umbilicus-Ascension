using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OxygenTank : MonoBehaviour {

	public static OxygenTank instance;

	public AudioClip alarm;

	public float oxygenLevel;

	[HideInInspector]
	public float oxygenDecay;

	private readonly float DEFAULT_OXYGEN_DECAY = 0.0001f;
	private readonly float WARNING_OXYGEN_LEVEL = 0.50f;
	private readonly float DANGER_OXYGEN_LEVEL = 0.15f;
	private Slider oxygenSlider;
	private Color oxygenSliderOriginalColor;
	private Image oxygenSliderImage;
	private bool triggerWarning;
	private bool triggerDanger;

	// Runs before Start
	public void Awake () {
		this.triggerWarning = false;
		this.triggerDanger = false;
		this.oxygenDecay = DEFAULT_OXYGEN_DECAY;
		this.oxygenSlider = this.GetComponent<Slider>();
		this.oxygenSliderImage = this.oxygenSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
		this.oxygenSliderOriginalColor = this.oxygenSliderImage.color;
		instance = this;
	}

	// Update is called once per frame
	public void Update () {

		this.oxygenLevel -= this.oxygenDecay;

		if(this.oxygenLevel > WARNING_OXYGEN_LEVEL) {
			this.triggerWarning = false;
			this.triggerDanger = false;
			this.oxygenSliderImage.color = this.oxygenSliderOriginalColor;
			Sound_Manager.Instance.StopEffectLoop(1);
		}

		if(!this.triggerWarning && this.oxygenLevel < WARNING_OXYGEN_LEVEL) {
			this.oxygenSliderImage.color = Color.yellow;
			Sound_Manager.Instance.PlayEffectOnce(this.alarm);
			this.triggerWarning = true;
		}

		if(!this.triggerDanger && this.oxygenLevel < DANGER_OXYGEN_LEVEL) {
			this.oxygenSliderImage.color = Color.red;
			Sound_Manager.Instance.PlayEffectLoop(this.alarm, 1);
			this.triggerDanger = true;
		}

		if(this.isEmpty()) {
			this.oxygenLevel = 0.0f; // clamp to 0.0f;
			Sound_Manager.Instance.StopEffectLoop(1);
			this.oxygenSlider.gameObject.SetActive(false);
		}

		if(this.oxygenLevel > 1.0f) {
			this.oxygenLevel = 1.0f;
		}

		this.oxygenSlider.value = this.oxygenLevel;
	}

	/// <returns><c>true</c>, if the oxygen tank is empty, <c>false</c> otherwise.</returns>
	public bool isEmpty() {
		return (this.oxygenLevel < 0.00001f);
	}

	/// <summary>
	/// Adds oxygen to the tank.
	/// </summary>
	/// <param name="oxygenAmount">Oxygen amount (should be between 0.0 and 1.0)</param>
	public void addOxygen(float oxygenAmount) {
		this.oxygenLevel += oxygenAmount;
	}
}
