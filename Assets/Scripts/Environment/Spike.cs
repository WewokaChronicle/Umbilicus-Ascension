using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	public bool bloody = false;

	public GameObject particlePrefab;

	// Called before Start
	public void Awake()
	{
		// Sprite
		this.spriteRenderer = GetComponent<SpriteRenderer>(); 

		// Set random sprite
		this.spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Environment/Spikes" + Random.Range(1, 3)) as Sprite;
	}

	// Called on Collision
	public void OnCollisionEnter2D(Collision2D coll)
	{
		// someone has already died here - let no more die here
		if(bloody) {
			return;
		}

		if(Vector2.Dot(coll.contacts[0].normal, -Vector2.up) > 0.2f) {

			// let's see if the thing colliding with us has a RockSpecial
			RockSpecial rock = coll.gameObject.GetComponent<RockSpecial>();
			if(rock != null && rock.rocking) {
				((GameObject) Instantiate(this.particlePrefab, transform.position + Vector3.back * 2f, particlePrefab.transform.rotation))
					.GetComponent<DestroyParticlesOnFinish>().followTarget = transform;
				Destroy(this.gameObject); // if it does, and it's active, the spike will blow up!
			}

			// if it doesn't have a rock special, we will die here
			else {
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
}
