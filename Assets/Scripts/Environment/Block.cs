using UnityEngine;
using System.Collections;

public class Block:MonoBehaviour
{
	public bool starterBlock = false;

	// Sounds
	public AudioClip[] breakSnd;
	public AudioClip[] crumbleSnd;

	private int blockNum;
	private Sprite sp;
	private Sprite spikeSprite;
	private SpriteRenderer spriteRenderer;
	private Color c;

	private bool spikes = false;

	public GameObject batteryPrefab;
	public GameObject spikePrefab;

	public GameObject particlesPrefab;

	private Spike spike = null;
	private GameObject battery = null;

	public void Awake()
	{
		this.spriteRenderer = GetComponent<SpriteRenderer>();
		// Set random sprite
		blockNum = Random.Range(1, 4);
		this.spriteRenderer.sprite = Instantiate(Resources.Load<Sprite>("Sprites/Environment/block" + blockNum)) as Sprite;


		// Spikes
		if(!starterBlock) {
			if(Random.Range(0, 10) >= 0) {
				GameObject spikeGO = (GameObject)Instantiate(spikePrefab, transform.position + Vector3.up + Vector3.back, Quaternion.Euler(new Vector3(0f, 0f, -90f)));
				spikeGO.transform.parent = transform;
				spikeSprite = spikeGO.GetComponent<Sprite>();
				spike = spikeGO.GetComponent<Spike>();
				spikes = true;
			} else if(Random.Range(0, 8) == 0) {
				battery = (GameObject)Instantiate(batteryPrefab, transform.position + Vector3.up * 1.5f + Vector3.back, Quaternion.identity);
				((Rigidbody2D)battery.GetComponent<Rigidbody2D>()).velocity = Vector2.up * GameManager.instance.SCROLL_SPEED;
			}
		}
		// Velocity
		GetComponent<Rigidbody2D>().velocity = Vector2.up * GameManager.instance.SCROLL_SPEED;
	}

	public void Update()
	{
		// Destroy when we leave the screen
		if(transform.position.y < -10f) {
			Destroy(gameObject);
			if(battery != null) {
				Destroy(battery);
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D coll)
	{
		if(spikes && spike.bloody) {
			return;
		}
	}
}
