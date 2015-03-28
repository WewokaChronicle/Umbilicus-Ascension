using UnityEngine;
using System.Collections;

public class Spike:MonoBehaviour {
	public AudioClip flattenSound;

//	private Sprite sp;
	private SpriteRenderer spriteRenderer;
	
	public bool bloody = false;
	private bool flattened = false;

	public GameObject particlePrefab;

	public void Awake() {
		// Sprite
		spriteRenderer = GetComponent<SpriteRenderer>(); 
		// Set random sprite
		spriteRenderer.sprite = Resources.Load<Sprite>("Spikes" + Random.Range(1, 3));
	}

	public void OnCollisionEnter2D(Collision2D coll) {
		if(flattened || bloody) {
			return;
		}
		if(Vector2.Dot(coll.contacts[0].normal, -Vector2.up) > 0.2f) {

			/***
			 * ROUTINE FOR DESTROYING SPIKED BLOCK?
			 */
//			RockSpecial rock = coll.gameObject.GetComponent<RockSpecial>();
			GameObject rock = null;
			if(rock != null /* && rock.rocking */) {
				Destroy(gameObject);
			} else if(!flattened) {

				/****************************
				* KILL PLAYER ROUTINE
				****************************/
//				Player player = coll.gameObject.GetComponent<Player>();
//				if(player != null) {
//					player.Kill();
//					bloody = true;
//					sp.SetSprite(sp.CurrentSprite.name + "_Blood");
//				}
			}
		}
		if(!bloody) {
			Sound_Manager.Instance.PlayEffectOnce(flattenSound);
			flattened = true;
			GetComponent<Collider2D>().isTrigger = true;
			// Spawn particles
			((GameObject) Instantiate(particlePrefab, transform.position + Vector3.back * 2f, particlePrefab.transform.rotation)).GetComponent<DestroyParticlesOnFinish>().followTarget = transform;
		}
	}
}
