using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawn : MonoBehaviour
{
    void Start()
    {
		if(Random.Range(0,2) == 1)
		{
			CollectibleData coll = GameManager.currentZoneLootTable.GetRandomCollectible();
			if (coll == null) return;
			GameObject prefab = Resources.Load<GameObject>(coll.name);
			GameObject obj = Instantiate(prefab, transform);

			float randomZRotation = Random.Range(0f, 360f);

			obj.transform.position = transform.position;
			obj.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);

			if(coll is LootData l)
			{
				int quantity = GameManager.currentZoneLootTable.GetRandomQuantity(coll);
				obj.GetComponent<Loot>().quantity = quantity;
			}
			else if (coll is ItemData i)
			{
				int quantity = GameManager.currentZoneLootTable.GetRandomQuantity(coll);
				obj.GetComponent<Item>().Quantity = quantity;
			}
		}
	}
}
