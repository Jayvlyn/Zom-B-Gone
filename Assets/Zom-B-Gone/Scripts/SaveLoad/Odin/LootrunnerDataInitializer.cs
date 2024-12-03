using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootrunnerDataInitializer : MonoBehaviour
{
    public SuperTextMesh crucialAquireText;
    public SuperTextMesh ZBGAquireText;

    public static bool initialized = false;

    private void Awake()
    {
        if (initialized)
        {
            if (GameManager.checkZoneUnlock) CheckZoneUnlock();
            SetUnlockedZones();
			if (SaveManager.currentSave.playerData.zbgUnlocked) ShowZomBGoneOnSchematic();
            SetInfoText();
			return;
        }

        initialized = true;

        SaveManager.currentSave = SaveManager.saves.lootrunnerSaves[SaveManager.loadedSave];

        SaveManager.SetPlayerData(GameManager.Instance.dataRefs.playerData, SaveManager.currentSave.playerData);
        
        // Hands
        if (SaveManager.currentSave.hands != null) GameManager.Instance.dataRefs.handsData.Container = SaveManager.currentSave.hands;
        else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.handsData.Container);

        // Head
        if (SaveManager.currentSave.head != null) GameManager.Instance.dataRefs.headData.Container = SaveManager.currentSave.head;
        else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.headData.Container);

		// Workbench Input
		if (SaveManager.currentSave.workbenchInput != null) GameManager.Instance.dataRefs.worbenchInputData.Container = SaveManager.currentSave.workbenchInput;
		else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.worbenchInputData.Container);

		// Workbench Output
		if (SaveManager.currentSave.workbenchOutput != null) GameManager.Instance.dataRefs.worbenchOutputData.Container = SaveManager.currentSave.workbenchOutput;
		else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.worbenchOutputData.Container);

		// Backpack
		if (SaveManager.currentSave.backpack != null)
        {
			GameManager.Instance.dataRefs.backpackData.size = SaveManager.currentSave.backpack.collectibleSlots.Length;
			GameManager.Instance.dataRefs.backpackData.Container = SaveManager.currentSave.backpack;
        }
        else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.backpackData.Container);

        // Hat locker
        if (SaveManager.currentSave.hatLocker != null)
        {
			GameManager.Instance.dataRefs.hatLockerData.size = SaveManager.currentSave.hatLocker.collectibleSlots.Length;
			GameManager.Instance.dataRefs.hatLockerData.Container = SaveManager.currentSave.hatLocker;
        }
        else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.hatLockerData.Container);

        // Item locker
        if (SaveManager.currentSave.itemLocker != null)
        {
            GameManager.Instance.dataRefs.itemLockerData.size = SaveManager.currentSave.itemLocker.collectibleSlots.Length;
            GameManager.Instance.dataRefs.itemLockerData.Container = SaveManager.currentSave.itemLocker;
        }
        else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.itemLockerData.Container);

        // Loot locker
        if (SaveManager.currentSave.lootLocker != null)
        {
			GameManager.Instance.dataRefs.lootLockerData.size = SaveManager.currentSave.lootLocker.collectibleSlots.Length;
			GameManager.Instance.dataRefs.lootLockerData.Container = SaveManager.currentSave.lootLocker;
        }
        else Utils.ClearContainerSlots(GameManager.Instance.dataRefs.lootLockerData.Container);

		// Van 
		if (SaveManager.currentSave.vanFloor != null)
        {
            GameManager.Instance.dataRefs.vanFloor.floorContainer.Container = SaveManager.currentSave.vanFloor.floorContainer;
            GameManager.Instance.dataRefs.vanFloor.floorContainer.collectibleDict = SaveManager.currentSave.vanFloor.collectibleDict;
        }
        else
        {
            Utils.ClearContainerSlots(GameManager.Instance.dataRefs.vanFloor.floorContainer.Container);
            GameManager.Instance.dataRefs.vanFloor.floorContainer.ClearFloorSpecificVals();
        }


        if (SaveManager.currentSave.unitFloor != null)
        {
            GameManager.Instance.dataRefs.unitFloor.floorContainer.Container = SaveManager.currentSave.unitFloor.floorContainer;
            GameManager.Instance.dataRefs.unitFloor.floorContainer.collectibleDict = SaveManager.currentSave.unitFloor.collectibleDict;
        }
        else
        {
            Utils.ClearContainerSlots(GameManager.Instance.dataRefs.unitFloor.floorContainer.Container);
            GameManager.Instance.dataRefs.unitFloor.floorContainer.ClearFloorSpecificVals();
        }

        if(SaveManager.currentSave.merchantVals != null)
        {
            for (int i = 0; i < GameManager.Instance.dataRefs.marketData.merchants.Length; i++)
            {
                GameManager.Instance.dataRefs.marketData.merchants[i].vals = SaveManager.currentSave.merchantVals[i];
            }
            GameManager.Instance.dataRefs.marketData.Day = SaveManager.currentSave.marketDay;
            GameManager.Instance.dataRefs.marketData.cycleCount = SaveManager.currentSave.marketCycles;
        }
        else
        {
            SaveManager.currentSave.merchantVals = new List<MerchantVals>();
            foreach (var merchant in GameManager.Instance.dataRefs.marketData.merchants)
            {
                merchant.vals = new MerchantVals();
                GameManager.Instance.dataRefs.marketData.RefreshDealingCollectibles(merchant);
                GameManager.Instance.dataRefs.marketData.RefreshMerchantInventory(merchant);
                GameManager.Instance.dataRefs.marketData.RefreshBuyOffers(merchant);
            }
            GameManager.Instance.dataRefs.marketData.Day = 1;
            GameManager.Instance.dataRefs.marketData.cycleCount = 0;
		}

        SetUnlockedZones();

        if (SaveManager.currentSave.playerData.zbgUnlocked) 
            ShowZomBGoneOnSchematic();

        SetInfoText();
    }

    public void SetUnlockedZones()
    {
		for (int i = 0; i < GameManager.Instance.dataRefs.playerData.unlockedZones.Length; i++)
		{
			GameManager.Instance.dataRefs.zoneButtons[i].interactable = GameManager.Instance.dataRefs.playerData.unlockedZones[i];
		}
	}

    public int GetLockedIndex()
    {
		int lockedIndex = -1;
		for (int i = 0; i < GameManager.Instance.dataRefs.playerData.unlockedZones.Length; i++)
		{
			if (!GameManager.Instance.dataRefs.playerData.unlockedZones[i])
			{
				lockedIndex = i;
				break;
			}
		}
		
        return lockedIndex;
	}

	public void CheckZoneUnlock()
	{
        int lockedIndex = GetLockedIndex();
        if(lockedIndex == -1) return; // all unlocked

		ItemData checkingForThisItem = GetNextRequiredItem(lockedIndex);

        if (CheckItemInHandsVan(checkingForThisItem)) UnlockZone(lockedIndex, 1f);
	}

    public void SetInfoText()
    {
        GameManager.Instance.dataRefs.infoText.text =
            GameManager.Instance.dataRefs.playerData.characterName + "\n" +
            "Kills: " + GameManager.Instance.dataRefs.playerData.kills;
    }

    public bool CheckItemInHandsVan(ItemData item)
    {
        if (GameManager.Instance.dataRefs.handsData.container.collectibleSlots[0].Collectible == item) return true;
        if (GameManager.Instance.dataRefs.handsData.container.collectibleSlots[1].Collectible == item) return true;
        foreach(CollectibleSlot cs in GameManager.Instance.dataRefs.vanFloor.floorContainer.container.collectibleSlots)
        {
            if (cs.Collectible == item) return true;
        }
        return false;
    }

    public void CheckZoneUnlockFromCollectible(CollectibleData collectible)
    {
		int lockedIndex = GetLockedIndex();
		if (lockedIndex == -1) return; // all unlocked

        ItemData checkingForThisItem = GetNextRequiredItem(lockedIndex);
        if (collectible == checkingForThisItem)
        {
            UnlockZone(lockedIndex);
            SetUnlockedZones();
        }
	}


	public void CheckForZBG(CollectibleData collectibleData)
	{
		if (collectibleData == CodeMonkey.Assets.i.ZBGData)
		{
            GameManager.Instance.dataRefs.playerData.zbgUnlocked = true;
            ShowZomBGoneOnSchematic();
			StartCoroutine(ShowZBGAquiredText());
		}
	}

    public void ShowZomBGoneOnSchematic()
    {
        GameManager.Instance.dataRefs.schematicZBG.color = Color.white;
	}

	public void UnlockZone(int index, float textDelay = 0)
    {
		GameManager.Instance.dataRefs.playerData.unlockedZones[index] = true;
        StartCoroutine(ShowItemAquiredText());
	}

    public ItemData GetNextRequiredItem(int lockedIndex)
    {
        return CodeMonkey.Assets.i.zoneUnlockItems[lockedIndex - 1];  // -1 because zoneUnlockItems index 0 is zone 2 unlock
	}

    public IEnumerator ShowItemAquiredText(float initialDelay = 0)
    {
        if(initialDelay > 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }
        crucialAquireText.gameObject.SetActive(true);
        crucialAquireText.Read();
        yield return new WaitForSeconds(3f);
        crucialAquireText.UnRead();
        yield return new WaitForSeconds(3f);
        crucialAquireText.gameObject.SetActive(false);

    }

	public IEnumerator ShowZBGAquiredText(float initialDelay = 0)
	{
		if (initialDelay > 0)
		{
			yield return new WaitForSeconds(initialDelay);
		}
		ZBGAquireText.gameObject.SetActive(true);
		ZBGAquireText.Read();
		yield return new WaitForSeconds(3f);
		ZBGAquireText.UnRead();
		yield return new WaitForSeconds(3f);
		ZBGAquireText.gameObject.SetActive(false);

	}
}
