using UnityEngine;
using System.Collections;

public class BlockGenerator:MonoBehaviour
{
	public GameObject blockPrefab;
	private const float SPAWN_TIME = 4f;
	private float spawnTimer = 0f;
	private float startTime;
	private bool started = false;
	private const int MAX_BLOCKS = 5;
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
			Debug.Log("spawn blocks");
			// Spawn!
			SpawnBlock();

			spawnTimer = SPAWN_TIME + Random.Range(0f, 1f);
		}
	}

	public void SpawnBlock()
	{
		Vector3 pos = new Vector3(Random.Range(-MAX_BLOCK_X, MAX_BLOCK_X), 10f);
		GameObject block;

		// randomly select number of blocks in group
		int count = Random.Range(1, MAX_BLOCKS);

		for(int i = 0; i < count; i++) {
			if(pos.x <= MAX_BLOCK_X) {
				block = (GameObject)GameObject.Instantiate(blockPrefab, pos, Quaternion.identity);
				block.name = blockPrefab.name;
				pos.x += 1.4f;
			} else {
				break; // don't generate any more blocks in this group
			}
		}
	}
}
