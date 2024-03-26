using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootDestroyer : MonoBehaviour
{
    [SerializeField] private Backpack backpack = null;
    [SerializeField] private GameObject destroyPanel = null;
    [SerializeField] private TextMeshProUGUI confirmText = null;

    private int slotIndex = 0;

    private void OnDisable()
    {
        slotIndex = -1;
    }

    public void Activate(LootSlot slot, int slotIndex)
    {
        this.slotIndex = slotIndex;
        confirmText.text = $"Are you sure you wish to destroy {slot.quantity}x {slot.loot.ColoredName}?";

        //gameObject.SetActive(true);
        destroyPanel.SetActive(true);
    }

    public void Destroy()
    {
        backpack.Container.RemoveAt(slotIndex);

        //gameObject.SetActive(false);
        destroyPanel.SetActive(false);
    }
}