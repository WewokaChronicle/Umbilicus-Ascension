
using UnityEngine;
using System.Collections;

public class PlatformGenerator : MonoBehaviour
{
	public GameObject blockPrefab;
	private const float SPAWN_TIME = 4f;
	private float spawnTimer = 0f;
	private float startTime;
	private bool started = false;
	private const int MAX_BLOCKS = 4;
	private const float MAX_BLOCK_X = 6f;
	private const float BLOCK_OFFSET_X = 1.2f;
	private EdgeCollider2D edgeCollider;
	private GameObject[] blocks;
	private float blockWidth;


	public void Start()
	{
		this.blocks = new GameObject[MAX_BLOCKS];
		Time.timeScale = 0f;
		startTime = Time.realtimeSinceStartup;
		// calculate the width of a block
		GameObject initBlock = (GameObject)GameObject.Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
		this.blockWidth = initBlock.GetComponent<Renderer>().bounds.size.x;
		Destroy(initBlock);
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
		GameObject platform = new GameObject("Platform");
		Debug.Log("spawning platform");
		int blockCount = Random.Range(1, MAX_BLOCKS+1);
		Vector3 pos = new Vector3(Random.Range(-MAX_BLOCK_X, MAX_BLOCK_X), 10f);
		for(int i=0; i < blockCount; i++) {
			this.blocks[i] = (GameObject)GameObject.Instantiate(blockPrefab, pos, Quaternion.identity);
			pos.x += BLOCK_OFFSET_X;
		}

		platform.AddComponent<EdgeCollider2D>();
		Vector2 startPoint = this.blocks[0].transform.position;
		Vector2 endPoint = startPoint + 
			new Vector2(this.blockWidth * blockCount, startPoint.y);
		Vector2[] points = new Vector2[]{startPoint, endPoint};
		platform.GetComponent<EdgeCollider2D>().points = points;
	}
}
