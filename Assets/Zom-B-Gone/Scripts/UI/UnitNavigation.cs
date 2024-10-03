using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitNavigation : MonoBehaviour
{
    public LootrunnerDataRefs dataRefs;

    public void OpenMainMenu()
    {
        SaveManager.currentSave.hands = dataRefs.handsData.Container;
        SaveManager.currentSave.head = dataRefs.headData.Container;
        SaveManager.currentSave.backpack = dataRefs.backpackData.Container;
        SaveManager.currentSave.hatLocker = dataRefs.hatLockerData.Container;
        SaveManager.currentSave.itemLocker = dataRefs.itemLockerData.Container;
        SaveManager.currentSave.lootLocker = dataRefs.lootLockerData.Container;
        OdinSaveSystem.Save(SaveManager.saves);
        if(SaveManager.currentSave.head.collectibleSlots[0].collectible) Debug.Log(SaveManager.currentSave.head.collectibleSlots[0].collectible.name);
        SceneManager.LoadScene("MainMenu");
    }
}
