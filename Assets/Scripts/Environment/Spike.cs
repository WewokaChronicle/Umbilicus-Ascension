using UnityEngine;
using System.Collections;

public class Spike:MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	
	public bool bloody = false;

	public GameObject particlePrefab;

	public void Awake()
	{
		// Sprite
		this.spriteRenderer = GetComponent<SpriteRenderer>(); 
		// Set random sprite
		this.spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Environment/Spikes" + Random.Range(1, 3)) as Sprite;
	}

	public void OnCollisionEnter2D(Collision2D coll)
	{
		if(bloody) {
			return;
		}
		if(Vector2.Dot(coll.contacts[0].normal, -Vector2.up) > 0.2f) {
			// Kill player
			Player player = coll.gameObject.GetComponent<Player>();
			if(player != null) {
				player.Kill();
				bloody = true;
				this.spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Environment/" + this.spriteRenderer.sprite.name + "_Blood") as Sprite;
			}
		}
	}
}
