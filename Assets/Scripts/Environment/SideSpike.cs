using UnityEngine;
using System.Collections;

public class SideSpike:MonoBehaviour {
	public AudioClip flattenSound;

	private SpriteRenderer spriteRenderer;
	
	public bool bloody = false;
	private bool flattened = false;

	public void Awake() {
		// Sprite
		this.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if(flattened || bloody) {
			return;
		}
		RockSpecial rock = coll.gameObject.GetComponent<RockSpecial>();
		if(rock != null && rock.rocking) {
			Destroy(gameObject);
		} else if(!flattened) {
			Player player = coll.gameObject.GetComponent<Player>();
			if(player != null) {
				player.Kill();
				bloody = true;
				this.spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Environment/" + this.spriteRenderer.sprite.name + "_Blood") as Sprite;
			}
		}
		if(!bloody) {
			flattened = true;
			GetComponent<Collider2D>().isTrigger = true;
		}
	}
}
