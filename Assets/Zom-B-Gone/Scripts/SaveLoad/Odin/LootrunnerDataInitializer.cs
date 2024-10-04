using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootrunnerDataInitializer : MonoBehaviour
{
    public LootrunnerDataRefs dataRefs;

    private void Awake()
    {
        SaveManager.currentSave = SaveManager.saves.lootrunnerSaves[SaveManager.loadedSave];
        
        if (SaveManager.currentSave.hands != null) dataRefs.handsData.Container = SaveManager.currentSave.hands;
        else ClearContainerSlots(dataRefs.handsData.Container);

        if (SaveManager.currentSave.head != null) dataRefs.headData.Container = SaveManager.currentSave.head;
        else ClearContainerSlots(dataRefs.headData.Container);

        if (SaveManager.currentSave.backpack != null) dataRefs.backpackData.Container = SaveManager.currentSave.backpack;
        else ClearContainerSlots(dataRefs.backpackData.Container);

        if (SaveManager.currentSave.hatLocker != null) dataRefs.hatLockerData.Container = SaveManager.currentSave.hatLocker;
        else ClearContainerSlots(dataRefs.hatLockerData.Container);

        if (SaveManager.currentSave.itemLocker != null) dataRefs.itemLockerData.Container = SaveManager.currentSave.itemLocker;
        else ClearContainerSlots(dataRefs.itemLockerData.Container);

        if (SaveManager.currentSave.lootLocker != null) dataRefs.lootLockerData.Container = SaveManager.currentSave.lootLocker;
        else ClearContainerSlots(dataRefs.lootLockerData.Container);
    }

    private void ClearContainerSlots(CollectibleContainer container)
    {
        for (int i = 0; i < container.collectibleSlots.Length; i++)
        {
            //container.collectibleSlots[i].Collectible = null;
            container.collectibleSlots[i].CollectibleName = null;
            container.collectibleSlots[i].quantity = 0;
        }
    }
}
