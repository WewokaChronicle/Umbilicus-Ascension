using UnityEngine;
using UnityEngine.UI;
using InControl;
using System.Collections;

public class MenuPlayerController : MonoBehaviour 
{
	public int playerNumber; // the player's id.
	public Image playerMenuPanel; // the image panel that says "P[X]" where X = player number

	private InputDevice inputDevice; // the player's controller.
	private bool inGame = false; // is the player in the game?

	[HideInInspector]
	public int selectedCharacter = -1; // sentinel
	
	// Use this for initialization
	void Start () {
		this.inputDevice = (InputManager.Devices.Count > playerNumber && PlayerControl.NumberOfPlayers > playerNumber) ? InputManager.Devices[playerNumber] : null;

		if(this.inputDevice == null) { // we're not playing!
			Destroy(playerMenuPanel.gameObject); // destroy the panel
			Destroy(this.gameObject); // destroy ourselves
		}

		else {
			this.inGame = true;
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
