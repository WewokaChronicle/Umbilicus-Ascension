using UnityEngine;
using System.Collections;

public class BlockGenerator:MonoBehaviour
{
	public GameObject blockPrefab;
	private const float SPAWN_TIME = 0.6f;
	private float spawnTimer = 0f;
	private float startTime;
	private bool started = false;
	private const int MAX_BLOCKS = 4;
	private const float MAX_BLOCK_X = 6f;


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
		// Timer
		spawnTimer -= Time.deltaTime;

		if(spawnTimer <= 0f) {
			// Spawn!
			SpawnBlock();

			spawnTimer = SPAWN_TIME + Random.Range(0f, 1f);
		}
	}

	public void SpawnBlock()
	{
		Vector3 pos = new Vector3(Random.Range(-6, 6), -10f);
		GameObject block;

		// randomly select number of blocks in group
		int cnt = Random.Range(1, MAX_BLOCKS);

		for(int i = 0; i < cnt; i++) {
			block = (GameObject)GameObject.Instantiate(blockPrefab, pos, Quaternion.identity);
			block.name = blockPrefab.name;

			// check if block is outside of bounds of level
			if((pos.x += 1.4f) > MAX_BLOCK_X) {
				Destroy(block);
				break; // stop spawning blocks
			}
		}
	}
}
