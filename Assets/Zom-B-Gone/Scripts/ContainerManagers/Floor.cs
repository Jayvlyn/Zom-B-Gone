using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
	public FloorContainer floorContainer;
	public Collider2D floorCollider;

	public IEnumerator AddToFloorContainer(Collider2D collision)
	{
		yield return new WaitForSeconds(2.2f);
		if (collision.bounds.Intersects(floorCollider.bounds))
		{
			if (collision.TryGetComponent(out Collectible c))
			{
				collision.gameObject.transform.parent = transform;
				floorContainer.AddCollectibleToContainer(c);
			}
		}
	}
}
