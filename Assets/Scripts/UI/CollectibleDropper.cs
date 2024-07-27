using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectibleDropper : MonoBehaviour
{
    [SerializeField] private CollectibleContainerData container = null;
    public CollectibleContainerData handContainerData;

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

        if(container == handContainerData) // dropping out of hand slots
        {
            if (playerController == null) playerController = FindObjectOfType<PlayerController>();

            if (slotIndex == 0) playerController.DropLeft();
            else if (slotIndex == 1) playerController.DropRight();

        }

        container.onContainerCollectibleUpdated.Raise();
    }
}