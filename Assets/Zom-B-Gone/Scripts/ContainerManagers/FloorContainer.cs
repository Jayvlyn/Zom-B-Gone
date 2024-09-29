using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorContainer : MonoBehaviour
{
    public CollectibleContainerData floorContainer;

    // int is index in the container, PosRot is the local position and rotation of the collectibles on the floor
    private Dictionary<PosRot, int> vanCollectibles = new Dictionary<PosRot, int>();

    public int collectibleCount = 0;

    public void AddColliderToContainer(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out Collectible c))
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
            vanCollectibles[posRot] = containerIndex;
            collectibleCount++;
        }
    }

    public void RemoveFromContainer(PosRot posRot)
    {
        int containerIndex = vanCollectibles[posRot];
        floorContainer.Container.RemoveAt(containerIndex);
        collectibleCount--;
    }
}
