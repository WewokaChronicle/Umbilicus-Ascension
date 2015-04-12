using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
	public const float SPEED = -1f;
	public bool starterPlatform = false;
	public GameObject blockPrefab;
	private const int MAX_BLOCKS = 4;
	private const float MAX_BLOCK_X = 6f;
	private const float BLOCK_OFFSET_X = 1.2f;
	private GameObject[] blocks;

	void Awake()
	{
		this.blocks = new GameObject[MAX_BLOCKS];
		this.Spawn();
		GetComponent<Rigidbody2D>().velocity = Vector2.up * SPEED;
	}

	public void Update()
	{
		// Destroy when we leave the screen
		if(transform.position.y < -70f) {
			Destroy(gameObject);
		}
	}
	
	public void OnCollisionEnter2D(Collision2D coll)
	{
		Debug.Log("platform collision");
	}

	public void Spawn()
	{
		Debug.Log("spawning platform");
		//		GameObject platform = new GameObject("Platform");
		
		int blockCount = Random.Range(1, MAX_BLOCKS + 1);

		Vector3 blockPosition = new Vector3(transform.position.x, transform.position.y);
		for(int i=0; i < blockCount; i++) {
			if(blockPosition.x <= MAX_BLOCK_X) {
				this.blocks[i] = (GameObject)GameObject.Instantiate(blockPrefab, blockPosition, Quaternion.identity);
				blockPosition.x += BLOCK_OFFSET_X;
			}
		}
		
		// Add edge collider with length based on block count
		gameObject.AddComponent<EdgeCollider2D>();
		Vector2 startPoint = transform.position + new Vector3(-BLOCK_OFFSET_X / 2f, 1.2f / 2f);
		Vector2 endPoint = startPoint + new Vector2(BLOCK_OFFSET_X * blockCount, 0);
		Vector2[] points = new Vector2[]{startPoint, endPoint};
		gameObject.GetComponent<EdgeCollider2D>().points = points;
	}
}

