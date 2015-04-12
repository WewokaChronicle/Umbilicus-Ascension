using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {

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
		this.milkywayMikeIndex = -1;
		this.quasarQuadeIndex = -1;
		this.stardustStanIndex = -1;
		this.cosmonautCarlaIndex = -1;
	}
}
