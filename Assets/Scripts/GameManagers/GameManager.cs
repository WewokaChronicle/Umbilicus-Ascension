using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class GameManager : MonoBehaviour {

	// Public attributes
	public float scrollSpeed = -1.0f;
	public float startingDepth = -10000f; // Game Attribute
	public float winningDepth = 0f;
	public GameObject winningPlatformPrefab;

	public AudioClip gameMusic;
	public AudioClip gameOverSound;
	public AudioClip highScoreSound;
	public Text altitudeText;
	public Text highScoreText;

	[HideInInspector]
	public Player winner;

	public static GameManager instance;
	public static int highScore;

	private bool winningPlatformHasBeenRequested;
	private Player[] players;
	private float altitude;
	private bool ended = false;
	private float endTime;
	
	// Runs before Start
	public void Awake()
	{
		// Attach Input Devices
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileIJKL()));
		}

		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileWASD()));
		}

		instance = this;
		this.altitude = startingDepth;
		this.winningPlatformHasBeenRequested = false;
		GameManager.highScore = PlayerPrefs.GetInt("HighScore", 0);
		this.highScoreText.text = "HIGH SCORE: " + highScore;
		Sound_Manager.Instance.PlayMusicLoop(gameMusic);
	}
	
	// Called every frame
	public void Update() 
	{
		if(this.ended) {
			return;
		}

		if(this.altitude > this.winningDepth && !this.winningPlatformHasBeenRequested) {
			PlatformGenerator.instance.generateWinningPlatform = true;
			this.winningPlatformHasBeenRequested = true;
		}

		if(Input.GetKeyUp(KeyCode.Escape)) {
			Application.LoadLevel(0);
			Time.timeScale = 1f;
		}

		altitude += Time.deltaTime * 100f;
		if(! ended) {
			altitudeText.text = "ALTITUDE: " + Mathf.RoundToInt(altitude);
		}

		// check if any players are still alive
		Player[] players = this.FindPlayers();
		bool allPlayersDead = true;
		foreach(Player player in players) {
			if(!player.isDead) {
				allPlayersDead = false;
				return;
			}
		}

		if(allPlayersDead) {
			this.EndLevel();
		}

	}
	
	/// <summary>
	/// Collects the oxygen powerup and adds oxygen to the tank.
	/// </summary>
	public void CollectOxygenTank() {
		OxygenTank.instance.addOxygen(1.0f);
	}

	/// <summary>
	/// Ends the level.
	/// </summary>
	public void EndLevel() {
		if(!ended) {
			ended = true;
			Time.timeScale = 0f;
			endTime = Time.realtimeSinceStartup;

			// mute any loop sounds
			Sound_Manager.Instance.StopEffectLoop(Sound_Manager.GAS_LOOP_CHANNEL);
			Sound_Manager.Instance.StopEffectLoop(Sound_Manager.ALARM_LOOP_CHANNEL);

			OxygenTank.instance.gameObject.SetActive(false);
			StartCoroutine(_EndRoutine());
		}
	}

	/// <returns>All the players that currently exist in this Scene.</returns>
	public Player[] FindPlayers() 
	{
		ArrayList activePlayers = new ArrayList();
		
		// try to find each in-game player and add it to the list
		GameObject milkywayMike = GameObject.Find("Milkyway Mike");
		if(milkywayMike != null) {
			Player milkywayMikePlayer = milkywayMike.GetComponent<Player>();
			if(milkywayMikePlayer.inGame) {
				activePlayers.Add(milkywayMikePlayer);
			}
		}
		
		GameObject quasarQuade = GameObject.Find("Quasar Quade");
		if(quasarQuade != null) {
			Player quasarQuadePlayer = quasarQuade.GetComponent<Player>();
			if(quasarQuadePlayer.inGame) {
				activePlayers.Add(quasarQuadePlayer);
			}
		}
		
		GameObject stardustStan = GameObject.Find("Stardust Stan");
		if(stardustStan != null) {
			Player stardustStanPlayer = stardustStan.GetComponent<Player>();
			if(stardustStanPlayer.inGame) {
				activePlayers.Add(stardustStanPlayer);
			}
		}
		
		GameObject cosmonautCarla = GameObject.Find("Cosmonaut Carla");
		if(cosmonautCarla != null) {
			Player cosmonautCarlaPlayer = cosmonautCarla.GetComponent<Player>();
			if(cosmonautCarlaPlayer.inGame) {
				activePlayers.Add(cosmonautCarlaPlayer);
			}
		}
		
		// compile the list into an array
		object[] activePlayerObjects = activePlayers.ToArray();
		Player[] result = new Player[activePlayerObjects.Length];
		for(int i = 0; i < activePlayerObjects.Length; i++) {
			result[i] = (Player)activePlayerObjects[i];
		}
		
		// return the array
		return result;
	}

	/// <summary>
	/// Displays win/lose text and restarts the level.
	/// </summary>
	private IEnumerator _EndRoutine()
	{
		if(winner) {
			_WinText();
		} else {
			_DeathText();
		}

		while(Time.realtimeSinceStartup - endTime < 3f) {
			yield return false;
		}
		Time.timeScale = 1f;
		Application.LoadLevel(1);
	}

	/// <summary>
	/// Auxiliary method for displaying the Death Text
	/// </summary>
	private void _DeathText() {
		// Save score
		bool high = false;
		if(altitude > highScore) {
			highScore = Mathf.RoundToInt(altitude);
			PlayerPrefs.SetInt("HighScore", highScore);
			highScoreText.text = "HIGH SCORE: " + highScore;
			PlayerPrefs.Save();
			high = true;
			Sound_Manager.Instance.PlayEffectOnce(highScoreSound);
		} else {
			Sound_Manager.Instance.PlayEffectOnce(gameOverSound);
		}
		// Text
		if(high) {
			altitudeText.text = "YOU DIED\n\n" + altitudeText.text + "\n\nHIGH SCORE ACHIEVED!!!";
		} else {
			altitudeText.text = "YOU DIED\n\n" + altitudeText.text;
		}
		// Reposition
		Vector3 pos = altitudeText.rectTransform.localPosition;
		pos.y = 80f;
		altitudeText.rectTransform.localPosition = pos;
	}
	
	/// <summary>
	/// Auxiliary method for displaying the Win Text
	/// </summary>
	private void _WinText() 
	{
		bool high = false;
		if(altitude > highScore) {
			highScore = Mathf.RoundToInt(altitude);
			PlayerPrefs.SetInt("HighScore", highScore);
			highScoreText.text = "HIGH SCORE: " + highScore;
			PlayerPrefs.Save();
			high = true;
			Sound_Manager.Instance.PlayEffectOnce(highScoreSound);
		} else {
			Sound_Manager.Instance.PlayEffectOnce(highScoreSound);
		}

		// Text
		int playerID = winner.GetComponent<Player>().playerNumber + 1;
		if(high) {
			altitudeText.text = "PLAYER " + playerID + " WINS\n\n" + altitudeText.text + "\n\nHIGH SCORE ACHIEVED!!!";
		} 

		else {
			altitudeText.text = "PLAYER " + playerID + " WINS\n\n" + altitudeText.text;
		}

		// Reposition
		Vector3 pos = altitudeText.rectTransform.localPosition;
		pos.y = 80f;
		altitudeText.rectTransform.localPosition = pos;
	}

	/// <summary>
	/// Generates the winning platform to end the game.
	/// </summary>
	private void _GenerateWinningPlatform() {
		Vector3 platformPosition = new Vector3(Random.Range(-PlatformGenerator.MAX_BLOCK_X, PlatformGenerator.MAX_BLOCK_X), 10f);
		GameObject.Instantiate(this.winningPlatformPrefab, platformPosition, Quaternion.identity);
	}
}
