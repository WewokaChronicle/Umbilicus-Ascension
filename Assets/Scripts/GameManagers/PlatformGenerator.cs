
using UnityEngine;
using System.Collections;

public class PlatformGenerator : MonoBehaviour
{
	public GameObject blockPrefab;
	private const float SPAWN_TIME = 4f;
	private float spawnTimer = 0f;
	private float startTime;
	private bool started = false;
	private const int MAX_BLOCKS = 5;
	private const float MAX_BLOCK_X = 6f;
	private const float BLOCK_OFFSET_X = 1.2f;
	private EdgeCollider2D edgeCollider;
	private Block[] blocks;

	public void Awake()
	{
		this.blocks = new Block[MAX_BLOCKS];
	}

	public void Start()
	{
		Time.timeScale = 0f;
		startTime = Time.realtimeSinceStartup;
	}
	
	public void Update()
	{
		// Start
		if(!started && (Time.realtimeSinceStartup - startTime) >= 1f) {
			Time.timeScale = 1f;
			started = true;
		}
		spawnTimer -= Time.deltaTime;
		
		if(spawnTimer <= 0f) {
			SpawnPlatform();			
			spawnTimer = SPAWN_TIME + Random.Range(0f, 1f);
		}
	}
	
	public void SpawnPlatform()
	{
		Debug.Log("spawning platform");
		int blockCount = Random.Range(1, MAX_BLOCKS);
		Block block;
		Vector2 pos = transform.position;
		for(int i=0; i < blockCount; i++) {
			this.blocks[i] = GameObject.Instantiate(blockPrefab, pos, Quaternion.identity) as Block;
			pos.x += BLOCK_OFFSET_X;
		}

//		this.edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
//		Vector2 startPoint = new Vector2(transform.position.x, transform.position.y);
//		Vector2 endPoint = startPoint + 
//			new Vector2(blockPrefab.GetComponent<SpriteRenderer>().sprite.textureRect.width * count, 0);
//		Vector2[] points = new Vector2[]{startPoint, endPoint};
//		this.edgeCollider.points = points;
	}
}
