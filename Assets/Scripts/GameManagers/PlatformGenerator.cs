
using UnityEngine;
using System.Collections;

public class PlatformGenerator : MonoBehaviour
{
	public static PlatformGenerator instance;

	public GameObject platformPrefab;
	public GameObject winningPlatformPrefab;

	[HideInInspector]
	public bool generateWinningPlatform;

	private const float SPAWN_TIME = 4f;
	private float spawnTimer = 0f;
	private float startTime;
	private bool started = false;
	private const int MAX_BLOCKS = 4;
	public static readonly float MAX_BLOCK_X = 6f;
	private const float BLOCK_OFFSET_X = 1.2f;

	public void Start()
	{
		Time.timeScale = 0f;
		startTime = Time.realtimeSinceStartup;
		instance = this;
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

			// generate the last one!
			if(generateWinningPlatform == true) {
				this.SpawnWinningPlatform();
				this.generateWinningPlatform = false;
			}

			else {
				this.SpawnPlatform();
			}


			spawnTimer = SPAWN_TIME + Random.Range(0f, 1f);
		}
	}
	
	public void SpawnPlatform()
	{
		Vector3 platformPosition = new Vector3(Random.Range(-MAX_BLOCK_X, MAX_BLOCK_X), 10f);
		GameObject.Instantiate(platformPrefab, platformPosition, Quaternion.identity);
	}

	public void SpawnWinningPlatform()
	{
		Vector3 platformPosition = new Vector3(Random.Range(-MAX_BLOCK_X, MAX_BLOCK_X), 10f);
		GameObject.Instantiate(winningPlatformPrefab, platformPosition, Quaternion.identity);
	}
}
