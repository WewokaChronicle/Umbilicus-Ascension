using UnityEngine;
using UnityEngine.UI;
using InControl;
using System.Collections;

public class MenuPlayerController : MonoBehaviour 
{
	public int playerNumber; // the player's id.
	public Image playerMenuPanel; // the image panel that says "P[X]" where X = player number
	public Image[] characterMenuPanels; // the image panels that contain all the game characters

	private static readonly int AVAILABLE = -1; // this denotes a character that is still available
	private static int numberOfCharacters = 4; // there are four potential characters
	private InputDevice inputDevice; // the player's controller.
	private bool inGame = false; // is the player in the game?
	private bool playerHasChosenCharacter; // has the player chosen a character yet?
	private Camera camera;

	[HideInInspector]
	public static int[] selectedCharacters;

	[HideInInspector]
	public int highlightedCharacter;

	/// <returns><c>true</c>, if all registered players have chosen a character, <c>false</c> otherwise.</returns>
	public static bool allPlayersHaveChosenACharacter() {

		// accumulate the number of characters that have been chosen
		int numberOfCharactersThatHaveBeenChosen = 0;
		for(int i = 0; i < MenuPlayerController.selectedCharacters.Length; i++) {
			if(MenuPlayerController.selectedCharacters[i] != AVAILABLE) {
				numberOfCharactersThatHaveBeenChosen++;
			}
		}

		if(numberOfCharactersThatHaveBeenChosen != PlayerControl.NumberOfPlayers) {
			return false;
		}

		else {
			return true;
		}
	}
	
	// Use this for initialization
	public void Start () {
		this.camera = Camera.main;

		this.highlightedCharacter = 0;
		MenuPlayerController.selectedCharacters = new int[4] {AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE};

		// Debug.Log("Controller " + playerNumber + " @ Position: " + playerMenuPanel.transform.position);

		this.inputDevice = (InputManager.Devices.Count > playerNumber && PlayerControl.NumberOfPlayers > playerNumber) ? InputManager.Devices[playerNumber] : null;

		if(this.inputDevice == null) { // we're not playing!
			Destroy(playerMenuPanel.gameObject); // destroy the panel
			Destroy(this.gameObject); // destroy ourselves
		}

		else {
			this.inGame = true;
			this.playerHasChosenCharacter = false;
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
		float xPosition = leftMostXPosition + (this.highlightedCharacter * offset);
		float yPosition = playerMenuPanel.transform.position.y;
		float zPosition = 0.0f;
		playerMenuPanel.transform.position = new Vector3(xPosition, yPosition, zPosition);
	}

	/// <summary>
	/// Manages player input updates.
	/// </summary>
	private void _InputUpdate(InputDevice device) {

		if(device.Direction.Left.WasPressed && !this.playerHasChosenCharacter) {
			this.highlightedCharacter--;
		}

		if(device.Direction.Right.WasPressed && !this.playerHasChosenCharacter) {
			this.highlightedCharacter++;
		}

		// wrap around the total number of characters
		this.highlightedCharacter = _Modulo(this.highlightedCharacter, MenuPlayerController.numberOfCharacters);

		// set the planel representing the player to the color of the character's panel
		this.playerMenuPanel.color = this.characterMenuPanels[this.highlightedCharacter].color;

		// player is attempting to select a character
		if(device.Action1.WasPressed) {

			// can only do so if the character is still available
			if(MenuPlayerController.selectedCharacters[this.highlightedCharacter] == AVAILABLE)
			{
				// record that the highlighted character now belongs to this player
				MenuPlayerController.selectedCharacters[this.highlightedCharacter] = this.playerNumber;

				// gray out the panels representing the player
				this._SetUIGraphicAlpha(this.playerMenuPanel, 1.0f); 

				// gray out the panels representing the character
				Image characterPanel = this.characterMenuPanels[this.highlightedCharacter];
				Text characterPanelText = characterPanel.transform.FindChild("Text").GetComponent<Text>();
				this._SetUIGraphicAlpha(characterPanel, 0.1f);
				this._SetUIGraphicAlpha(characterPanelText, 0.1f);

				// set a flag indicating a character has been selected
				this.playerHasChosenCharacter = true;
			}
		}

		// player is trying to cancel a character selection
		if(device.Action2.WasPressed && this.playerHasChosenCharacter) {

			// record that the player has released the highlighted character
			MenuPlayerController.selectedCharacters[this.highlightedCharacter] = AVAILABLE;

			// bring the panels back to their regular alpha
			this._SetUIGraphicAlpha(this.playerMenuPanel, 0.6f);

			Image characterPanel = this.characterMenuPanels[this.highlightedCharacter];
			Text characterPanelText = characterPanel.transform.FindChild("Text").GetComponent<Text>();
			this._SetUIGraphicAlpha(characterPanel, 0.4f);
			this._SetUIGraphicAlpha(characterPanelText, 1.0f);

			// unset the player has chosen flag
			this.playerHasChosenCharacter = false;
		}
	}

	/// <summary>
	/// Sets the user interface's graphic alpha to the parameter.
	/// </summary>
	private void _SetUIGraphicAlpha(Graphic g, float alpha) {
		Color oldElementColor = g.color;
		g.color = new Color(oldElementColor.r, oldElementColor.g, oldElementColor.b, alpha);
	}


	/// <summary>
	/// Performs a modulo operation a % b and wraps
	/// around both positive and negative values
	/// </summary>
	private int _Modulo(int a, int b) {
		return (a % b + b) % b;
	}
}
