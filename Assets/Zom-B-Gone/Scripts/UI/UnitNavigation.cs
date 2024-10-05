using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitNavigation : MonoBehaviour
{
    public LootrunnerDataRefs dataRefs;

    public void OpenMainMenu()
    {
        SaveManager.currentSave.hands = SaveManager.DeepCopyContainer(dataRefs.handsData.Container);
        SaveManager.currentSave.head = SaveManager.DeepCopyContainer(dataRefs.headData.Container);
        SaveManager.currentSave.backpack = SaveManager.DeepCopyContainer(dataRefs.backpackData.Container);
        SaveManager.currentSave.hatLocker = SaveManager.DeepCopyContainer(dataRefs.hatLockerData.Container);
        SaveManager.currentSave.itemLocker = SaveManager.DeepCopyContainer(dataRefs.itemLockerData.Container);
        SaveManager.currentSave.lootLocker = SaveManager.DeepCopyContainer(dataRefs.lootLockerData.Container);

        SaveableFloor vanFloor = new SaveableFloor();
        vanFloor.collectibleDict = dataRefs.vanFloor.floorContainer.collectibleDict;
        vanFloor.floorContainer = SaveManager.DeepCopyContainer(dataRefs.vanFloor.floorContainer.Container);

        SaveableFloor unitFloor = new SaveableFloor();
        unitFloor.collectibleDict = dataRefs.unitFloor.floorContainer.collectibleDict;
        unitFloor.floorContainer = SaveManager.DeepCopyContainer(dataRefs.unitFloor.floorContainer.Container);

        SaveManager.currentSave.vanFloor = vanFloor;
        SaveManager.currentSave.unitFloor = unitFloor;

        OdinSaveSystem.Save(SaveManager.saves);
        SceneManager.LoadScene("MainMenu");
    }
}
