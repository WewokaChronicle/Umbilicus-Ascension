using UnityEngine;
using System.Collections;

public class Battery:MonoBehaviour {
	public bool collected = false;
	public AudioClip collectSnd;

	public void Awake() {
		Animator anim = GetComponent<Animator>();
		anim.Play("default");
	}

	public void OnTriggerEnter2D() {
		if(collected)
			return;
		collected = true;
//		GameManager.instance.Collect();
//		Sound_Manager.Instance.PlayEffectOnce(collectSnd);
		Destroy(gameObject);
	}
}
