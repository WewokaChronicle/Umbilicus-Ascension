using UnityEngine;
using UnityEngine.UI;
using InControl;
using System.Collections;

public class MenuPlayerController : MonoBehaviour 
{
	public int playerNumber; // the player's id.
	public Image playerMenuPanel; // the image panel that says "P[X]" where X = player number

	private static int numberOfCharacters = 4; // there are four potential characters
	private InputDevice inputDevice; // the player's controller.
	private bool inGame = false; // is the player in the game?
	private Camera camera;

	[HideInInspector]
	public int[] characters;

	[HideInInspector]
	public int selectedCharacter;
	
	// Use this for initialization
	public void Start () {
		this.camera = Camera.main;
		this.selectedCharacter = 0;
		Debug.Log("Controller " + playerNumber + " @ Position: " + playerMenuPanel.transform.position);

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
	public void Update () {
		this._InputUpdate(this.inputDevice);
		this._PlayerMenuPanelUpdate(this.playerMenuPanel);
	}


	/// <summary>
	/// Updates the position of the parameter playerMenuPanel based
	/// on the currently selectedCharacter.
	/// </summary>
	private void _PlayerMenuPanelUpdate(Image playerMenuPanel) {
		float leftMostXPosition = 180.0f;
		float offset = 200.0f;
		float xPosition = leftMostXPosition + (this.selectedCharacter * offset);
		float yPosition = playerMenuPanel.transform.position.y;
		float zPosition = 0.0f;
		playerMenuPanel.transform.position = new Vector3(xPosition, yPosition, zPosition);
	}

	/// <summary>
	/// Manages player input update.
	/// </summary>
	private void _InputUpdate(InputDevice device) {

		float xInput = device.Direction.X; // get horizontal input

		if(device.Direction.Right.WasPressed) { // moving right
			this.selectedCharacter++;
		}

		if(device.Direction.Left.WasPressed) { // moving left
			this.selectedCharacter--;
		}

		// wrap around the total number of characters
		this.selectedCharacter = _Modulo(this.selectedCharacter, MenuPlayerController.numberOfCharacters);
	}

	/// <summary>
	/// Performs a modulo operation a % b and wraps
	/// around both positive and negative values
	/// </summary>
	private int _Modulo(int a, int b) {
		return (a % b + b) % b;
	}
}
