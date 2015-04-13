using UnityEngine;
using System;
using System.Collections;
using InControl;

public class ChainGenerator : MonoBehaviour
{
	[HideInInspector]
	public Player[] characters;

	public GameObject chainLinkPrefab;
	public AudioClip chainBurstSound;
	public AudioClip oxygenLeakSound;

	private const float PLAYER_DISTANCE = 1f;
	private const int NUM_LINKS = 10;
	private InputDevice lastActiveInputDevice;
	private GameObject[] instantiatedChainLinks;
	private bool chainHasBeenSevered;

	// Init
	public void Start()
	{
		this.characters = this._FindPlayers();

		// the total possible number of chain links is given by the
		// number of links per character * the number of character in the scene
		this.instantiatedChainLinks = new GameObject[NUM_LINKS*this.characters.Length];
		this.lastActiveInputDevice = InputManager.ActiveDevice;

		// Reposition players
		Vector3 startingPosition = characters[0].transform.position;

		for(int i = 1; i < characters.Length; i++) {
			characters[i].transform.position = characters[i-1].transform.position + (PLAYER_DISTANCE * Vector3.right);
//			characters[i].transform.Translate(Vector3.right * PLAYER_DISTANCE * i);

		}

		// Create prefabs for chain between players
		characters[0].playerOnTheEnd = true;
		for(int i = 1; i < characters.Length; i++) {
			if(characters[i].inGame) {
				CreateChain(i, characters[i - 1], characters[i]);
				// Set myself to the new player on the end and unless it's player 0, set the player behind me to no longer on the end
				if(i > 1) {
					characters[i - 1].playerOnTheEnd = false;
				}
				characters[i].playerOnTheEnd = true;
			}
		}

		this.chainHasBeenSevered = false;
	}

	// FixedUpdate is called every fixed framerate frame
	public void FixedUpdate()
	{
		this.lastActiveInputDevice = InputManager.ActiveDevice;

		// the player has manually severed the connection
		if(this.lastActiveInputDevice.Action2.WasPressed && !this.chainHasBeenSevered) {

			// chain has been severed!
			this.chainHasBeenSevered = true;

			// get every link
			for(int linkIndex = 0; linkIndex < this.instantiatedChainLinks.Length; linkIndex++) {
				GameObject link = this.instantiatedChainLinks[linkIndex];

				// if it exists, destroy the link's hinge
				if(link != null) {
					HingeJoint2D linkHinge = link.GetComponent<HingeJoint2D>();
					Destroy(linkHinge);

					// and blast it to oblivion
					Vector2 force = -Vector2.up * UnityEngine.Random.Range(1.0f, 5.0f);
					link.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
				}
			}

			// Because the oxygen tank has burst, the oxygen falls quicker by a factor of 10
			OxygenTank.instance.oxygenDecay *= 10.0f;

			Sound_Manager.Instance.PlayEffectOnce(this.chainBurstSound); // chain burst!
			Sound_Manager.Instance.PlayEffectLoop(this.oxygenLeakSound, 2); // oxygen leak!
		}

		// stop the oxygen leak sound when there's no more oxygen
		if(OxygenTank.instance.isEmpty()) {
			Sound_Manager.Instance.StopEffectLoop(2);
		}


	}

	/// <summary>
	/// Creates a link chain between Players p1 and p2. Also stores every instantiated
	/// link prefab into the class array of instantiatedChainLinks
	/// </summary>
	public void CreateChain(int playerIndex, Player p1, Player p2)
	{
		Vector2 linkSize = ((BoxCollider2D)chainLinkPrefab.GetComponent<Collider2D>()).size;
		float linkOffset = linkSize.x * 0.1f;
		Vector3 pos = p1.transform.position + (Vector3.right * linkOffset) + Vector3.forward;

		GameObject link;
		HingeJoint2D hinge;
		Rigidbody2D prevLink = p1.GetComponent<Rigidbody2D>();

		for(int i = 0; i < NUM_LINKS; i++) {
			link = (GameObject)GameObject.Instantiate(chainLinkPrefab, pos + Vector3.right * linkSize.x * i, Quaternion.identity);
			link.name = "ChainP" + playerIndex + "_" + i;
			// Connect current link with previous link (or player)
			hinge = link.AddComponent<HingeJoint2D>();
			hinge.connectedBody = prevLink;
			hinge.anchor = new Vector2(-linkOffset, 0f);
			hinge.connectedAnchor = new Vector2(linkOffset, 0f);
			// Set previous link to current link
			this.instantiatedChainLinks[i * playerIndex] = link;
			prevLink = link.GetComponent<Rigidbody2D>();
		}

		// Final hinge
		hinge = prevLink.gameObject.AddComponent<HingeJoint2D>();
		hinge.transform.position = p2.transform.position + (Vector3.right * linkOffset) + Vector3.forward;
		hinge.connectedBody = p2.GetComponent<Rigidbody2D>();
		hinge.anchor = new Vector2(-linkOffset, 0f);
		hinge.connectedAnchor = new Vector2(linkOffset, 0f);
	}

	/// <returns>All the players that currently exist in this Scene.</returns>
	private Player[] _FindPlayers() {

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
}
