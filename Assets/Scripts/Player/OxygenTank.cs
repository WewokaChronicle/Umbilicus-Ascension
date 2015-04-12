using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OxygenTank : MonoBehaviour {

	public static OxygenTank instance;

	[HideInInspector]
	public float oxygenLevel;

	[HideInInspector]
	public float oxygenDecay;

	private readonly float DEFAULT_OXYGEN_DECAY = 0.001f;
	private Slider oxygenSlider;
	private Image oxygenSliderImage;

	// Runs before Start
	public void Awake () {
		this.oxygenDecay = DEFAULT_OXYGEN_DECAY;
		this.oxygenLevel = 0.6f;
		this.oxygenSlider = this.GetComponent<Slider>();
		this.oxygenSliderImage = this.oxygenSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
		instance = this;
	}


	
	// Update is called once per frame
	public void Update () {

		this.oxygenLevel -= this.oxygenDecay;

		if(this.oxygenLevel < 0.66f) {
			this.oxygenSliderImage.color = Color.yellow;
		}

		if(this.oxygenLevel < 0.33f) {
			this.oxygenSliderImage.color = Color.red;
		}

		if(this.oxygenLevel < 0.0f) {
			this.oxygenLevel = 0.0f; // clamp to 0.0f;
		}

		if(this.isEmpty()) {
			this.oxygenSlider.gameObject.SetActive(false);
		}

		this.oxygenSlider.value = this.oxygenLevel;
	}

	/// <returns><c>true</c>, if the oxygen tank is empty, <c>false</c> otherwise.</returns>
	public bool isEmpty() {
		return (this.oxygenLevel < 0.00001f);
	}


}
