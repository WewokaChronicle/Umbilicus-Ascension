using UnityEngine;
using System;
using System.Collections;
using InControl;

public class ChainGenerator : MonoBehaviour
{
	public static ChainGenerator instance;

	[HideInInspector]
	public Player[] characters;

	[HideInInspector]
	public bool chainHasBeenSevered;

	public GameObject chainLinkPrefab;
	public AudioClip chainBurstSound;
	public AudioClip oxygenLeakSound;

	private const float PLAYER_DISTANCE = 1f;
	private const int NUM_LINKS = 10;
	private InputDevice lastActiveInputDevice;
	private GameObject[] instantiatedChainLinks;


	// Init
	public void Start()
	{
		instance = this;
		this.characters = GameManager.instance.FindPlayers();

		// the total possible number of chain links is given by the
		// number of links per character * the number of character in the scene
		this.instantiatedChainLinks = new GameObject[NUM_LINKS*this.characters.Length];
		this.lastActiveInputDevice = InputManager.ActiveDevice;

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
			this.SeverChain();
		}

		// stop the oxygen leak sound when there's no more oxygen
		if(OxygenTank.instance.isEmpty()) {
			Sound_Manager.Instance.StopEffectLoop(Sound_Manager.GAS_LOOP_CHANNEL);
		}
	}

	/// <summary>
	/// Severs the chain.
	/// </summary>
	public void SeverChain()
	{
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
		
		// Because the oxygen tank has burst, the oxygen falls quicker by a factor
		OxygenTank.instance.oxygenDecay *= 50.0f;
		
		Sound_Manager.Instance.PlayEffectOnce(this.chainBurstSound); // chain burst!
		Sound_Manager.Instance.PlayEffectLoop(this.oxygenLeakSound, 2); // oxygen leak!
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
}
