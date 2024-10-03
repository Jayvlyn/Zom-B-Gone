using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootrunnerDataInitializer : MonoBehaviour
{
    public LootrunnerDataRefs dataRefs;

    private void Awake()
    {
        SaveManager.currentSave = SaveManager.saves.lootrunnerSaves[SaveManager.loadedSave];
        if(SaveManager.currentSave.hands != null) dataRefs.handsData.Container = SaveManager.currentSave.hands;
        if (SaveManager.currentSave.head != null) dataRefs.headData.Container = SaveManager.currentSave.head;
        if (SaveManager.currentSave.backpack != null) dataRefs.backpackData.Container = SaveManager.currentSave.backpack;
        if (SaveManager.currentSave.hatLocker != null) dataRefs.hatLockerData.Container = SaveManager.currentSave.hatLocker;
        if (SaveManager.currentSave.itemLocker != null) dataRefs.itemLockerData.Container = SaveManager.currentSave.itemLocker;
        if (SaveManager.currentSave.lootLocker != null) dataRefs.lootLockerData.Container = SaveManager.currentSave.lootLocker;
    }
}
