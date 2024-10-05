using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorContainer : MonoBehaviour
{
    public FloorContainerData floorContainer;

    private void Awake()
    {
        LoadCollectiblesInWorld();
    }

    public void AddCollectibleToContainer(Collectible c)
    {
        c.floorContainer = this;

        int containerIndex = -1;
        if (c is Item i)
        {
            containerIndex = floorContainer.AddToContainerNSIA(i.itemData, i.Quantity);
            if (containerIndex == -1)
            {
                floorContainer.AddSpace(100);
                containerIndex = floorContainer.AddToContainerNSIA(i.itemData, i.Quantity);
            }
        }
        else if (c is Loot l)
        {
            containerIndex = floorContainer.AddToContainerNSIA(l.lootData, l.quantity);
            if (containerIndex == -1)
            {
                floorContainer.AddSpace(100);
                containerIndex = floorContainer.AddToContainerNSIA(l.lootData, l.quantity);
            }
        }
        else if (c is Hat h)
        {
            containerIndex = floorContainer.AddToContainerNSIA(h.hatData, 1);
            if (containerIndex == -1)
            {
                floorContainer.AddSpace(100);
                containerIndex = floorContainer.AddToContainerNSIA(h.hatData, 1);
            }
        }

        PosRot posRot = new PosRot(c.gameObject.transform.localPosition, c.gameObject.transform.localRotation);
        floorContainer.collectibleDict[posRot] = containerIndex;
        floorContainer.collectibleCount++;
    }

    public void RemoveFromContainer(PosRot posRot)
    {
        int containerIndex = floorContainer.collectibleDict[posRot];
        floorContainer.Container.RemoveAt(containerIndex);
        floorContainer.collectibleCount--;
    }

    public void LoadCollectiblesInWorld()
    {
        foreach (PosRot posRot in floorContainer.collectibleDict.Keys)
        {
            int index = floorContainer.collectibleDict[posRot];
            string collectibleName = floorContainer.Container.collectibleSlots[index].CollectibleName;
            GameObject prefab = Resources.Load<GameObject>(collectibleName);
            GameObject obj = Instantiate(prefab, transform);
			obj.transform.localPosition = posRot.position;
			obj.transform.localRotation = posRot.rotation;

			Collectible collectible = obj.GetComponent<Collectible>();
            collectible.floorContainer = this;

            if (gameObject.CompareTag("VanFloor"))
            {
                obj.transform.parent = transform;
                if(collectible is Item item)
                {
                    item.AddToVan();
                }
            }
        }
    }
}
