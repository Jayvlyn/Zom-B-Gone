using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
	public float breakRequirement = 5;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.TryGetComponent(out Rigidbody2D rb))
		{
			if (collision.relativeVelocity.magnitude > breakRequirement) Destroy(gameObject);
		}
	}
}
