using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
	public Sprite fade;
	private Color color;
	private float startTime;

	public void Start()
	{
		startTime = Time.realtimeSinceStartup;
	}

	public void Update()
	{
		float t = Time.realtimeSinceStartup - startTime;
		color.a = 1f - (t / 2f);
		this._SetFadeColor();
		if(t > 2f) {
			Destroy(fade);
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Sets fade texture color
	/// </summary>
	/// <returns><c>true</c>, if set fade color was set, <c>false</c> otherwise.</returns>
	private bool _SetFadeColor()
	{
		Texture2D text = this.fade.texture;
		int pixelCount = text.width * text.height;
		if(pixelCount > 0) {
			Color[] colors = new Color[pixelCount];
			text.SetPixels(0, 0, text.width, text.height, colors);
			return true;
		}
		return false;
	}
}
