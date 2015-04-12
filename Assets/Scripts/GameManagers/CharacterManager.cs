using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {

	public static readonly int UNASSIGNED = -1;
	public static readonly int NUMBER_OF_CHARACTERS = 4;

	public static readonly int MILKYWAY_MIKE_INDEX = 0;
	public static readonly int QUASAR_QUADE_INDEX = 1;
	public static readonly int STARDUST_STAN_INDEX = 2;
	public static readonly int COSMONAUT_CARLA_INDEX = 3;
	public static int[] selectedCharacters;

	// Runs before Start
	public void Awake() {
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	public void Start () {
		selectedCharacters = new int[4]{UNASSIGNED, UNASSIGNED, UNASSIGNED, UNASSIGNED};
	}
}
