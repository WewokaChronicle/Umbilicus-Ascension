using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PressActionScript : MonoBehaviour {

	public Text pressActionText;

	// Use this for initialization
	void Start () {
		this._MakeUIGraphicInvisible(this.pressActionText);
	}
	
	// Update is called once per frame
	void Update () {

		// Activate when all players have chosen a character
		if(MenuPlayerController.allPlayersHaveChosenACharacter()) {
			this._MakeUIGraphicVisible(this.pressActionText);
		}

		else {
			this._MakeUIGraphicInvisible(this.pressActionText);
		}
	}

	/// <summary>
	/// Sets the user interface graphic alpha to 0.0f
	/// </summary>
	private void _MakeUIGraphicInvisible(Graphic g) {
		Color oldElementColor = g.color;
		g.color = new Color(oldElementColor.r, oldElementColor.g, oldElementColor.b, 0.0f);
	}

	/// <summary>
	/// Sets the user interface graphic alpha to 1.0f
	/// </summary>
	private void _MakeUIGraphicVisible(Graphic g) {
		Color oldElementColor = g.color;
		g.color = new Color(oldElementColor.r, oldElementColor.g, oldElementColor.b, 1.0f);
	}
}
