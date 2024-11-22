using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootrunnerDataInitializer : MonoBehaviour
{
    public LootrunnerDataRefs dataRefs;

    public static bool initialized = false;

    private void Awake()
    {
        if (initialized) return;
        initialized = true;

        SaveManager.currentSave = SaveManager.saves.lootrunnerSaves[SaveManager.loadedSave];

        dataRefs.playerData = SaveManager.currentSave.playerData;
        
        // Hands
        if (SaveManager.currentSave.hands != null) dataRefs.handsData.Container = SaveManager.currentSave.hands;
        else Utils.ClearContainerSlots(dataRefs.handsData.Container);

        // Head
        if (SaveManager.currentSave.head != null) dataRefs.headData.Container = SaveManager.currentSave.head;
        else Utils.ClearContainerSlots(dataRefs.headData.Container);

		// Workbench Input
		if (SaveManager.currentSave.workbenchInput != null) dataRefs.worbenchInputData.Container = SaveManager.currentSave.workbenchInput;
		else Utils.ClearContainerSlots(dataRefs.worbenchInputData.Container);

		// Workbench Output
		if (SaveManager.currentSave.workbenchOutput != null) dataRefs.worbenchOutputData.Container = SaveManager.currentSave.workbenchOutput;
		else Utils.ClearContainerSlots(dataRefs.worbenchOutputData.Container);

		// Backpack
		if (SaveManager.currentSave.backpack != null)
        {
			dataRefs.backpackData.size = SaveManager.currentSave.backpack.collectibleSlots.Length;
			dataRefs.backpackData.Container = SaveManager.currentSave.backpack;
        }
        else Utils.ClearContainerSlots(dataRefs.backpackData.Container);

        // Hat locker
        if (SaveManager.currentSave.hatLocker != null)
        {
			dataRefs.hatLockerData.size = SaveManager.currentSave.hatLocker.collectibleSlots.Length;
			dataRefs.hatLockerData.Container = SaveManager.currentSave.hatLocker;
        }
        else Utils.ClearContainerSlots(dataRefs.hatLockerData.Container);

        // Item locker
        if (SaveManager.currentSave.itemLocker != null)
        {
            dataRefs.itemLockerData.size = SaveManager.currentSave.itemLocker.collectibleSlots.Length;
            dataRefs.itemLockerData.Container = SaveManager.currentSave.itemLocker;
        }
        else Utils.ClearContainerSlots(dataRefs.itemLockerData.Container);

        // Loot locker
        if (SaveManager.currentSave.lootLocker != null)
        {
			dataRefs.lootLockerData.size = SaveManager.currentSave.lootLocker.collectibleSlots.Length;
			dataRefs.lootLockerData.Container = SaveManager.currentSave.lootLocker;
        }
        else Utils.ClearContainerSlots(dataRefs.lootLockerData.Container);

		// Van 
		if (SaveManager.currentSave.vanFloor != null)
        {
            dataRefs.vanFloor.floorContainer.Container = SaveManager.currentSave.vanFloor.floorContainer;
            dataRefs.vanFloor.floorContainer.collectibleDict = SaveManager.currentSave.vanFloor.collectibleDict;
        }
        else
        {
            Utils.ClearContainerSlots(dataRefs.vanFloor.floorContainer.Container);
            dataRefs.vanFloor.floorContainer.ClearFloorSpecificVals();
        }


        if (SaveManager.currentSave.unitFloor != null)
        {
            dataRefs.unitFloor.floorContainer.Container = SaveManager.currentSave.unitFloor.floorContainer;
            dataRefs.unitFloor.floorContainer.collectibleDict = SaveManager.currentSave.unitFloor.collectibleDict;
        }
        else
        {
            Utils.ClearContainerSlots(dataRefs.unitFloor.floorContainer.Container);
            dataRefs.unitFloor.floorContainer.ClearFloorSpecificVals();
        }

        if(SaveManager.currentSave.merchantVals != null)
        {
            for (int i = 0; i < dataRefs.marketData.merchants.Length; i++)
            {
                dataRefs.marketData.merchants[i].vals = SaveManager.currentSave.merchantVals[i];
            }
        }
        else
        {
            SaveManager.currentSave.merchantVals = new List<MerchantVals>();
            foreach (var merchant in dataRefs.marketData.merchants)
            {
                merchant.vals = new MerchantVals();
                dataRefs.marketData.RefreshDealingCollectibles(merchant);
                dataRefs.marketData.RefreshMerchantInventory(merchant);
                dataRefs.marketData.RefreshBuyOffers(merchant);
            }
        }
    }


}
