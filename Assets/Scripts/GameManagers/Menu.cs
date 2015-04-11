using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

[RequireComponent (typeof (SpriteRenderer))]
public class Menu : MonoBehaviour
{
	public static bool starting = false;

	public AudioClip menuMusic;
	public AudioClip startSound;
	public Sprite fadeToSprite; //this is the sprite we'll fade to

	private Color color;
	private float startTimer = 2f;

	public void Awake()
	{
		// Load the fadeToSprite onto the renderer 
		GetComponent<SpriteRenderer>().sprite = this.fadeToSprite;

		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileArrows()));
			Debug.Log("Attaching Keyboard Arrows");
		}

		
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileWASD()));
			Debug.Log("Attaching Keyboard WASD");
		}


		// Look for device changes
		InputManager.OnDeviceAttached += inputDevice => Application.LoadLevel(0);
		InputManager.OnDeviceDetached += inputDevice => Application.LoadLevel(0);

		starting = false;
		Sound_Manager.Instance.PlayMusicLoop(menuMusic);
	}

	public void Update()
	{
		// Fade out this scene and load the Game when done
		if(starting) {
			startTimer -= Time.deltaTime;
			color.a = 1f - (startTimer / 2f);
			this.GetComponent<SpriteRenderer>().color = color;

			// if we're done fading out
			if(startTimer <= 0f) {
				Application.LoadLevel("Game");
			}
		}

		else {
			for(int i = 0; i < InputManager.Devices.Count && i < PlayerControl.NumberOfPlayers; i++) {
				if(InputManager.Devices[i].Action1) {
					starting = true;
					color = this.GetComponent<SpriteRenderer>().color;
					Sound_Manager.Instance.PlayEffectOnce(startSound);
				}
			}
		}
	}

}

