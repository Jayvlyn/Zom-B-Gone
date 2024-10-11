using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static Saves saves;
    public static string loadedSave;
    public static LootrunnerSave currentSave;
    private static bool hasInitialized = false;

    private void Awake()
    {
        LootrunnerDataInitializer.initialized = false;
        
        if (hasInitialized) return;

        try
        {
            saves = OdinSaveSystem.Load();
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to load lootrunner saves, creating new saves");
            Saves newSaves = new Saves();
            OdinSaveSystem.Save(newSaves);
            saves = newSaves; // probably redundant but its okay
        }

        hasInitialized = true;
    }

    public void DeleteLootrunner()
    {
        string name = DeletePopupShower.saveToDelete;
        Destroy(SaveOptionDeleteReserve.reserve); // destroys option button from list

        saves.lootrunnerSaves.Remove(name);
		OdinSaveSystem.Save(saves);
	}

	public static CollectibleContainer DeepCopyContainer(CollectibleContainer original)
	{
		CollectibleContainer copy = new CollectibleContainer(original.collectibleSlots.Length);
		copy.collectibleSlots = new CollectibleSlot[original.collectibleSlots.Length];

		for (int i = 0; i < original.collectibleSlots.Length; i++)
		{
			copy.collectibleSlots[i] = new CollectibleSlot();
			//copy.collectibleSlots[i].Collectible = original.collectibleSlots[i].Collectible;
			copy.collectibleSlots[i].CollectibleName = original.collectibleSlots[i].CollectibleName;
			copy.collectibleSlots[i].quantity = original.collectibleSlots[i].quantity;
            copy.collectibleSlots[i].allowLoot = original.collectibleSlots[i].allowLoot;
            copy.collectibleSlots[i].allowItems = original.collectibleSlots[i].allowItems;
            copy.collectibleSlots[i].allowHats = original.collectibleSlots[i].allowHats;
        }

		return copy;
	}

    public static void UpdateCurrentSave(LootrunnerDataRefs dataRefs)
    {
        currentSave.hands = DeepCopyContainer(dataRefs.handsData.Container);
        currentSave.head = DeepCopyContainer(dataRefs.headData.Container);
        currentSave.backpack = DeepCopyContainer(dataRefs.backpackData.Container);
        currentSave.hatLocker = DeepCopyContainer(dataRefs.hatLockerData.Container);
        currentSave.itemLocker = DeepCopyContainer(dataRefs.itemLockerData.Container);
        currentSave.lootLocker = DeepCopyContainer(dataRefs.lootLockerData.Container);

        SaveableFloor vanFloor = new SaveableFloor();
        vanFloor.collectibleDict = dataRefs.vanFloor.floorContainer.collectibleDict;
        vanFloor.floorContainer = DeepCopyContainer(dataRefs.vanFloor.floorContainer.Container);

        SaveableFloor unitFloor = new SaveableFloor();
        unitFloor.collectibleDict = dataRefs.unitFloor.floorContainer.collectibleDict;
        unitFloor.floorContainer = DeepCopyContainer(dataRefs.unitFloor.floorContainer.Container);

        currentSave.vanFloor = vanFloor;
        currentSave.unitFloor = unitFloor;


        if(currentSave.merchantVals.Count != dataRefs.marketData.merchants.Length)
        {
            currentSave.merchantVals.Clear();
			for (int i = 0; i < dataRefs.marketData.merchants.Length; i++)
			{
				currentSave.merchantVals.Add(dataRefs.marketData.merchants[i].vals);
			}
		}
        else
        {
            for(int i = 0; i < dataRefs.marketData.merchants.Length; i++)
            {
                currentSave.merchantVals[i] = dataRefs.marketData.merchants[i].vals;
            }
        }

    }
}
