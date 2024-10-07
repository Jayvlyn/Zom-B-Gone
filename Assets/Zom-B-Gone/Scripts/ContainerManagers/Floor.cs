using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
	public FloorContainer floorContainer;
	public Collider2D floorCollider;
	public float timeSinceAwake;
	private List<Collider2D> collidersInQueue = new List<Collider2D>();
	private List<Item> itemsInQueue = new List<Item>();

	private void Update()
	{
		timeSinceAwake += Time.deltaTime;
	}

	public IEnumerator AddToFloorContainer(Collider2D collision)
	{
		foreach(var collider in collidersInQueue)
		{
			if (collider == collision)
			{
				Debug.Log("Dupe found");
				yield return null;
			}
		}
		collidersInQueue.Add(collision);
		yield return new WaitForSeconds(2.2f);
		if (collision.bounds.Intersects(floorCollider.bounds))
		{
			if (collision.TryGetComponent(out Collectible c))
			{
				collision.gameObject.transform.parent = transform;
				floorContainer.AddCollectibleToContainer(c);
			}
		}
		collidersInQueue.Remove(collision);
	}

	public IEnumerator AddToFloorContainer(Item item)
	{
		foreach (var i in itemsInQueue)
		{
			if (i == item)
			{
				Debug.Log("Itme Dupe found");
				yield return null;
			}
		}
		itemsInQueue.Add(item);
		yield return new WaitForSeconds(2.2f);
		if (item.fullCollider.bounds.Intersects(floorCollider.bounds))
		{
			item.fullCollider.gameObject.transform.parent = transform;
			floorContainer.AddCollectibleToContainer(item);
			
		}
		itemsInQueue.Remove(item);
	}
}
