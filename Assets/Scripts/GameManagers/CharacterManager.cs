using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {

	public static readonly int UNASSIGNED = MenuPlayerController.AVAILABLE;

	[HideInInspector]
	public int milkywayMikeIndex;

	[HideInInspector]
	public int quasarQuadeIndex;

	[HideInInspector]
	public int stardustStanIndex;

	[HideInInspector]
	public int cosmonautCarlaIndex;

	// Runs before Start
	public void Awake() {
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	public void Start () {
		this.milkywayMikeIndex = UNASSIGNED;
		this.quasarQuadeIndex = UNASSIGNED;
		this.stardustStanIndex = UNASSIGNED;
		this.cosmonautCarlaIndex = UNASSIGNED;
	}
}
