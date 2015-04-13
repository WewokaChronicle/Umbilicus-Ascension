using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class GameManager : MonoBehaviour {
	public AudioClip gameMusic;
	public AudioClip gameOverSound;
	public AudioClip highScoreSound;

	public static GameManager instance;

	public static int highScore;

	public Text scoreText;
	public Text highScoreText;
	private float score;
	private bool ended = false;
	private float endTime;

	public Player winner;

	public void Awake() 
	{
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileIJKL()));
		}

		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileWASD()));
		}

		instance = this;
		score = 0f;
		highScore = PlayerPrefs.GetInt("HighScore", 0);
		highScoreText.text = "HIGH SCORE: " + highScore;

		Sound_Manager.Instance.PlayMusicLoop(gameMusic);
	}

	public void Update() {
		if(this.ended) {
			return;
		}

		if(Input.GetKeyUp(KeyCode.Escape)) {
			Application.LoadLevel(0);
			Time.timeScale = 1f;
		}

		score += Time.deltaTime * 100f;
		if(! ended) {
			scoreText.text = "SCORE: " + Mathf.RoundToInt(score);
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

	/// <returns>All the players that currently exist in this Scene.</returns>
	private Player[] FindPlayers() {
		
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
			result[i] = (Player) activePlayerObjects[i];
		}
		
		// return the array
		return result;
	}

	public void CollectOxygenTank() {
		OxygenTank.instance.addOxygen(0.25f);
	}

	public void EndLevel() {
		if(!ended) {
			ended = true;
			Time.timeScale = 0f;
			endTime = Time.realtimeSinceStartup;
			StartCoroutine(EndRoutine());
		}
	}

	private IEnumerator EndRoutine() {
		if(winner) {
			WinText();
		}
		else {
			DeathText();
		}
		while(Time.realtimeSinceStartup - endTime < 3f) {
			yield return false;
		}
		Time.timeScale = 1f;
		Application.LoadLevel(1);
	}

	private void DeathText() {
		// Save score
		bool high = false;
		if(score > highScore) {
			highScore = Mathf.RoundToInt(score);
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
			scoreText.text = "YOU DIED\n\n" + scoreText.text + "\n\nHIGH SCORE ACHIEVED!!!";
		} else {
			scoreText.text = "YOU DIED\n\n" + scoreText.text;
		}
		// Reposition
		Vector3 pos = scoreText.rectTransform.localPosition;
		pos.y = 80f;
		scoreText.rectTransform.localPosition = pos;
	}

	private void WinText() 
	{
		bool high = false;
		if(score > highScore) {
			highScore = Mathf.RoundToInt(score);
			PlayerPrefs.SetInt("HighScore", highScore);
			highScoreText.text = "HIGH SCORE: " + highScore;
			PlayerPrefs.Save();
			high = true;
			Sound_Manager.Instance.PlayEffectOnce(highScoreSound);
		} else {
			Sound_Manager.Instance.PlayEffectOnce(highScoreSound);
		}
		// Text
		if(high) {
			scoreText.text = winner.name + " WINS\n\n" + scoreText.text + "\n\nHIGH SCORE ACHIEVED!!!";
		} else {
			scoreText.text = winner.name + " WINS\n\n" + scoreText.text;
		}
		// Reposition
		Vector3 pos = scoreText.rectTransform.localPosition;
		pos.y = 80f;
		scoreText.rectTransform.localPosition = pos;
	}
}
