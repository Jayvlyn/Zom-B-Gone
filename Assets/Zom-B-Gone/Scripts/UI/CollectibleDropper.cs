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
        container.Container.RemoveAt(slotIndex);

        switch(container.containerType)
        {
            case ContainerType.HANDS:
				if (playerController == null) playerController = FindObjectOfType<PlayerController>();
				if (slotIndex == 0) playerController.DropLeft();
				else if (slotIndex == 1) playerController.DropRight();
                break;
            case ContainerType.HEAD:
				if (playerController == null) playerController = FindObjectOfType<PlayerController>();
                playerController.DropHat();

				break;

            default: // LOCKER or BACKPACK

                break;

        }

        container.onContainerCollectibleUpdated.Raise();
    }
}