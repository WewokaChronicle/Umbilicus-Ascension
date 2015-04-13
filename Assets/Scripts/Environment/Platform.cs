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
	public int blockCount = 1;
	public bool isWinCondition = false;

	void Awake()
	{
		if(this.blockCount < 1) {
			this.blockCount = Random.Range(1, MAX_BLOCKS + 1);
		}
		this.blocks = new GameObject[this.blockCount];
		this.Spawn();
		GetComponent<Rigidbody2D>().velocity = Vector2.up * SPEED;
	}

	public void Update()
	{
		// Destroy when we leave the screen
		if(transform.position.y < -10f) {
			Destroy(gameObject);
		}
	}
	
	public void OnCollisionEnter2D(Collision2D coll)
	{
		if(this.isWinCondition) {
			GameManager.instance.winner = coll.gameObject.GetComponent<Player>();
			GameManager.instance.EndLevel();
		}
	}

	public void Spawn()
	{

		Vector3 blockPosition = new Vector3(transform.position.x, transform.position.y);
		for(int i=0; i < this.blockCount; i++) {
			if(blockPosition.x <= MAX_BLOCK_X) {
				this.blocks[i] = (GameObject)GameObject.Instantiate(blockPrefab, blockPosition, Quaternion.identity);
				this.blocks[i].transform.parent = transform;
				if(this.starterPlatform) {
					// destroy children of all blocks (spikes, etc)
					foreach(Transform comp in this.blocks[i].transform) {
						Destroy(comp.gameObject);
					}
				}
				this.blocks[i].GetComponent<Block>().starterBlock = starterPlatform;
				blockPosition.x += BLOCK_OFFSET_X;
			}
		}
		
		// Add edge collider with length based on block count
		gameObject.AddComponent<EdgeCollider2D>();
		float collider_y = 1.2f / 2f;
		float edgeColliderAdjustment = 0.1f;
		Vector2 startPoint = new Vector3(-BLOCK_OFFSET_X / 2f + edgeColliderAdjustment, collider_y);
		Vector2 endPoint = new Vector2(BLOCK_OFFSET_X * this.blockCount - BLOCK_OFFSET_X / 2f - edgeColliderAdjustment, collider_y);
		Vector2[] points = new Vector2[]{startPoint, endPoint};
		gameObject.GetComponent<EdgeCollider2D>().points = points;
	}
}

