using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
	public FloorContainer floorContainer;
	public Collider2D floorCollider;
	public float timeSinceAwake;

	private void Update()
	{
		timeSinceAwake += Time.deltaTime;
	}
}
