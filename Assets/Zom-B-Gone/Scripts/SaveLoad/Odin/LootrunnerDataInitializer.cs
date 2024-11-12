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
        
        if (SaveManager.currentSave.hands != null) dataRefs.handsData.Container = SaveManager.currentSave.hands;
        else ClearContainerSlots(dataRefs.handsData.Container);

        if (SaveManager.currentSave.head != null) dataRefs.headData.Container = SaveManager.currentSave.head;
        else ClearContainerSlots(dataRefs.headData.Container);

        if (SaveManager.currentSave.backpack != null)
        {
			dataRefs.backpackData.size = SaveManager.currentSave.backpack.collectibleSlots.Length;
			dataRefs.backpackData.Container = SaveManager.currentSave.backpack;
        }
        else ClearContainerSlots(dataRefs.backpackData.Container);

        if (SaveManager.currentSave.hatLocker != null)
        {
			dataRefs.hatLockerData.size = SaveManager.currentSave.hatLocker.collectibleSlots.Length;
			dataRefs.hatLockerData.Container = SaveManager.currentSave.hatLocker;
        }
        else ClearContainerSlots(dataRefs.hatLockerData.Container);

        if (SaveManager.currentSave.itemLocker != null)
        {
            dataRefs.itemLockerData.size = SaveManager.currentSave.itemLocker.collectibleSlots.Length;
            dataRefs.itemLockerData.Container = SaveManager.currentSave.itemLocker;
        }
        else ClearContainerSlots(dataRefs.itemLockerData.Container);

        if (SaveManager.currentSave.lootLocker != null)
        {
			dataRefs.lootLockerData.size = SaveManager.currentSave.lootLocker.collectibleSlots.Length;
			dataRefs.lootLockerData.Container = SaveManager.currentSave.lootLocker;
        }
        else ClearContainerSlots(dataRefs.lootLockerData.Container);

        if (SaveManager.currentSave.vanFloor != null)
        {
            dataRefs.vanFloor.floorContainer.Container = SaveManager.currentSave.vanFloor.floorContainer;
            dataRefs.vanFloor.floorContainer.collectibleDict = SaveManager.currentSave.vanFloor.collectibleDict;
        }
        else
        {
            ClearContainerSlots(dataRefs.vanFloor.floorContainer.Container);
            dataRefs.vanFloor.floorContainer.ClearFloorSpecificVals();
        }

        if (SaveManager.currentSave.unitFloor != null)
        {
            dataRefs.unitFloor.floorContainer.Container = SaveManager.currentSave.unitFloor.floorContainer;
            dataRefs.unitFloor.floorContainer.collectibleDict = SaveManager.currentSave.unitFloor.collectibleDict;
        }
        else
        {
            ClearContainerSlots(dataRefs.unitFloor.floorContainer.Container);
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
