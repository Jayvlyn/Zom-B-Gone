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
		floorContainer.collectibleDict[posRot.ToStringKey()] = containerIndex;
        floorContainer.collectibleCount++;
    }

    public void RemoveFromContainer(PosRot posRot)
    {
        string balls = posRot.ToStringKey();
        int containerIndex = floorContainer.collectibleDict[posRot.ToStringKey()];
        floorContainer.Container.RemoveAt(containerIndex);
        floorContainer.collectibleCount--;
        floorContainer.collectibleDict.Remove(posRot.ToStringKey());
    }

    public void LoadCollectiblesInWorld()
    {
        foreach (string posRotKey in floorContainer.collectibleDict.Keys)
        {
			int index = floorContainer.collectibleDict[posRotKey];
            string collectibleName = floorContainer.Container.collectibleSlots[index].CollectibleName;
            GameObject prefab = Resources.Load<GameObject>(collectibleName);
            GameObject obj = Instantiate(prefab, transform);

			string[] keyParts = posRotKey.Split(',');
			float posX = float.Parse(keyParts[0]);
			float posY = float.Parse(keyParts[1]);
			float rotX = float.Parse(keyParts[2]);
			float rotY = float.Parse(keyParts[3]);
			float rotZ = float.Parse(keyParts[4]);
			float rotW = float.Parse(keyParts[5]);

			obj.transform.localPosition = new Vector2(posX, posY);
			obj.transform.localRotation = new Quaternion(rotX, rotY, rotZ, rotW);

			Collectible collectible = obj.GetComponent<Collectible>();
            collectible.floorContainer = this;

            if(collectible is Item item)
            {
                item.AddToFloor();
            }
        }
    }
}
