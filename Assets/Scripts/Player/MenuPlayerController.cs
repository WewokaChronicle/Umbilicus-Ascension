using UnityEngine;
using UnityEngine.UI;
using InControl;
using System.Collections;

public class MenuPlayerController : MonoBehaviour 
{
	public int playerNumber; // the player's id.
	public Image playerMenuPanel; // the image panel that says "P[X]" where X = player number
	public Image[] characterMenuPanels; // the image panels that contain all the game characters
	public AudioClip characterSelectSound;

	private InputDevice inputDevice; // the player's controller.
	private bool playerHasChosenCharacter; // has the player chosen a character yet?

	[HideInInspector]
	public int highlightedCharacter; // this is the character currently being considered by this MenuPlayerController

	/// <returns><c>true</c>, if all registered players have chosen a character, <c>false</c> otherwise.</returns>
	public static bool allPlayersHaveChosenACharacter() {

		// accumulate the number of characters that have been chosen
		int numberOfCharactersThatHaveBeenChosen = 0;
		for(int i = 0; i < CharacterManager.selectedCharacters.Length; i++) {
			if(CharacterManager.selectedCharacters[i] != CharacterManager.UNASSIGNED) {
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
		this.highlightedCharacter = 0;
		// Debug.Log("Controller " + playerNumber + " @ Position: " + playerMenuPanel.transform.position);

		this.inputDevice = (InputManager.Devices.Count > playerNumber && PlayerControl.NumberOfPlayers > playerNumber) ? InputManager.Devices[playerNumber] : null;

		if(this.inputDevice == null) { // we're not playing!
			Destroy(playerMenuPanel.gameObject); // destroy the panel
			Destroy(this.gameObject); // destroy ourselves
		}

		else {
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
			do {
				this.highlightedCharacter--; // wrap around the total number of characters
				this.highlightedCharacter = _Modulo(this.highlightedCharacter, CharacterManager.NUMBER_OF_CHARACTERS);
			} // keep moving if the player is hovering over a character that has been selected
			while(CharacterManager.selectedCharacters[this.highlightedCharacter] != CharacterManager.UNASSIGNED);
		}

		if(device.Direction.Right.WasPressed && !this.playerHasChosenCharacter) {
			do {
				this.highlightedCharacter++; // wrap around the total number of characters
				this.highlightedCharacter = _Modulo(this.highlightedCharacter, CharacterManager.NUMBER_OF_CHARACTERS);
			} // keep moving if the player is hovering over a character that has been selected
			while(CharacterManager.selectedCharacters[this.highlightedCharacter] != CharacterManager.UNASSIGNED);
		}

		// set the planel representing the player to the color of the character's panel
		this.playerMenuPanel.color = this.characterMenuPanels[this.highlightedCharacter].color;

		// player is attempting to select a character
		if(device.Action1.WasPressed) {

			// can only do so if the character is still available
			if(CharacterManager.selectedCharacters[this.highlightedCharacter] == CharacterManager.UNASSIGNED)
			{
				// record that the highlighted character now belongs to this player
				CharacterManager.selectedCharacters[this.highlightedCharacter] = this.playerNumber;

				// gray out the panels representing the player
				this._SetUIGraphicAlpha(this.playerMenuPanel, 1.0f); 

				// gray out the panels representing the character
				Image characterPanel = this.characterMenuPanels[this.highlightedCharacter];
				Text characterPanelText = characterPanel.transform.FindChild("Text").GetComponent<Text>();
				this._SetUIGraphicAlpha(characterPanel, 0.1f);
				this._SetUIGraphicAlpha(characterPanelText, 0.1f);

				// play this character's sound effect
				Sound_Manager.Instance.PlayEffectOnce(characterSelectSound);

				// set a flag indicating a character has been selected
				this.playerHasChosenCharacter = true;
			}
		}

		// player is trying to cancel a character selection
		if(device.Action2.WasPressed && this.playerHasChosenCharacter) {

			// record that the player has released the highlighted character
			CharacterManager.selectedCharacters[this.highlightedCharacter] = CharacterManager.UNASSIGNED;

			// bring the panels back to their regular alpha
			this._SetUIGraphicAlpha(this.playerMenuPanel, 0.6f);

			Image characterPanel = this.characterMenuPanels[this.highlightedCharacter];
			Text characterPanelText = characterPanel.transform.FindChild("Text").GetComponent<Text>();
			this._SetUIGraphicAlpha(characterPanel, 0.4f);
			this._SetUIGraphicAlpha(characterPanelText, 1.0f);

			// unset the player has chosen flag
			this.playerHasChosenCharacter = false;
		}

		// player was already over a character that was selected before she could act
		if(!this.playerHasChosenCharacter && CharacterManager.selectedCharacters[this.highlightedCharacter] != CharacterManager.UNASSIGNED) {
			this.playerMenuPanel.color = Color.black;
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
