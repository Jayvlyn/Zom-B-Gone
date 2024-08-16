using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectibleDropper : MonoBehaviour
{
    [SerializeField] private CollectibleContainerData container = null;

    private int slotIndex = 0;

    private PlayerController playerController;

    private void OnDisable()
    {
        slotIndex = -1;
    }

    public void Activate(int slotIndex)
    {
        this.slotIndex = slotIndex;
        Drop();
    }

    public void Drop()
    {
		if (playerController == null) playerController = FindObjectOfType<PlayerController>();

		switch (container.containerType)
        {
            case ContainerType.HANDS:
				if (slotIndex == 0) playerController.DropLeft();
				else if (slotIndex == 1) playerController.DropRight();
                break;
            case ContainerType.HEAD:
				
                playerController.DropHat();

				break;

            default: // LOCKER or BACKPACK
                // Need to instantiate a collectible into the world
                CollectibleData droppedCollectibleData = container.Container.collectibleSlots[slotIndex].collectible;
                int quantity = container.Container.collectibleSlots[slotIndex].quantity;
				GameObject prefab = Resources.Load<GameObject>(droppedCollectibleData.name);
				GameObject collectibleObject = Instantiate(prefab, playerController.transform.position, playerController.transform.rotation);

				if (droppedCollectibleData as HatData)
                {
                    Hat hat = collectibleObject.GetComponent<Hat>();
					hat.StartTransferPosition(playerController.transform.position + playerController.transform.up, hat.transform.rotation);
				}
				else if (droppedCollectibleData as LootData)
				{
					Loot loot = collectibleObject.GetComponent<Loot>();

                    CollectibleContainerSlot slot = transform.parent.GetComponent<CollectibleContainerSlot>();
                    int slotIndex = this.transform.GetSiblingIndex();
                    loot.lootCount = quantity;

					loot.StartTransferPosition(playerController.transform.position + playerController.transform.up, loot.transform.rotation);
				}
				else if (droppedCollectibleData as ItemData)
				{
                    collectibleObject.transform.rotation *= Quaternion.Euler(0, 0, -90);

 					Item item = collectibleObject.GetComponent<Item>();
                    item.InventoryDrop();
				}

				break;

        }

        container.Container.RemoveAt(slotIndex);
        container.onContainerCollectibleUpdated.Raise();
    }
}