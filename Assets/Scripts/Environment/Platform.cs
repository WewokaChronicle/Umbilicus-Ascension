using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
	public const float SPEED = -1f;

	void Awake()
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.up * SPEED;
	}

	public void Update()
	{
		// Destroy when we leave the screen
		if(transform.position.y < -70f) {
			Destroy(gameObject);
		}
	}
	
	public void OnCollisionEnter2D(Collision2D coll)
	{
		Debug.Log("platform collision");
	}
}

