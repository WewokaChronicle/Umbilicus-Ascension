using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
//	public Sprite fade;
	private Color color;
	private float alpha;
	private float startTime;

	public void Awake()
	{
		this.color = GetComponent<SpriteRenderer>().color;
		this.alpha = 0f;
		this.color.a = this.alpha;
		GetComponent<SpriteRenderer>().color = this.color;
	}

	public void Start()
	{
		startTime = Time.realtimeSinceStartup;
	}

	public void Update()
	{
		if(this.alpha < 1f)
		{
			this._FadeIn();
		}
	}

	/// <summary>
	/// Set alpha of sprite based on time since scene start
	/// </summary>
	private void _FadeIn() 
	{
		float t = Time.realtimeSinceStartup - startTime;
		this.color = GetComponent<SpriteRenderer>().color;
		this.alpha += t/100f;
		this.color.a = this.alpha;
		GetComponent<SpriteRenderer>().color = this.color;
	}
}
