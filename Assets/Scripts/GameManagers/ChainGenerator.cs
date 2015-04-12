using UnityEngine;
using System;
using System.Collections;

public class ChainGenerator : MonoBehaviour
{
	public Player[] characters;
	public GameObject chainLinkPrefab;

	private const float PLAYER_DISTANCE = 0.2f;
	private const int NUM_LINKS = 10;

	public void Start()
	{
		this.characters = this._FindPlayers();
		Debug.Log(this.characters.Length);

		// Reposition players
		for(int i = 1; i < characters.Length; i++) {
			characters[i].transform.Translate(Vector3.right * PLAYER_DISTANCE * i);
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
	}

	public void CreateChain(int playerInd, Player p1, Player p2)
	{
		Vector2 linkSize = ((BoxCollider2D)chainLinkPrefab.GetComponent<Collider2D>()).size;
		float linkOffset = linkSize.x * 0.2f;
		Vector3 pos = p1.transform.position + (Vector3.right * linkOffset) + Vector3.forward;

		GameObject link;
		HingeJoint2D hinge;
		Rigidbody2D prevLink = p1.GetComponent<Rigidbody2D>();

		for(int i = 0; i < NUM_LINKS; i++) {
			link = (GameObject)GameObject.Instantiate(chainLinkPrefab, pos + Vector3.right * linkSize.x * i, Quaternion.identity);
			link.name = "ChainP" + playerInd + "_" + i;
			// Connect current link with previous link (or player)
			hinge = link.AddComponent<HingeJoint2D>();
			hinge.connectedBody = prevLink;
			hinge.anchor = new Vector2(-linkOffset, 0f);
			hinge.connectedAnchor = new Vector2(linkOffset, 0f);
			// Set previous link to current link
			prevLink = link.GetComponent<Rigidbody2D>();
		}

		// Final hinge
		hinge = prevLink.gameObject.AddComponent<HingeJoint2D>();
		hinge.connectedBody = p2.GetComponent<Rigidbody2D>();
		hinge.anchor = new Vector2(linkOffset, 0f);
		hinge.connectedAnchor = new Vector2(-linkOffset, 0f);
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
