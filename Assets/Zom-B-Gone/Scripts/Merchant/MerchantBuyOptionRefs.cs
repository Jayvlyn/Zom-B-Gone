using GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantBuyOptionRefs : MonoBehaviour
{
    public bool sellOption = false;
	[HideInInspector] public MerchantData loadedMerchant;
	public HoverableCollectible hoverableCollectible;
    public VoidEvent updatePlayerGold;
    public VoidEvent updateReputation;

    public Color normalTotalPriceColor;
    public Color cannotAffordTotalPriceColor;

    public TMP_Text unitPrice;
    public TMP_Text selectedAmount;
    public TMP_Text maxAmount;
    public TMP_Text buyTotal;
    public Button minusButton;
    public Button plusButton;

    public CollectibleContainerData backpack;
    public CollectibleContainerData lootLocker;
    public CollectibleContainerData itemLocker;
    public CollectibleContainerData hatLocker;
    public CollectibleContainerData hands;
    public CollectibleContainerData head;
    public PlayerData currentSave;

    public VoidEvent sellLeftHand;
    public VoidEvent sellRightHand;

	public void OnMinusClick()
    {
        // button should be disabled if val is 1 so it will only be clicked if greater than 1

        int currentSelectedAmt = int.Parse(selectedAmount.text);
		int maxAmt = int.Parse(maxAmount.text);

		currentSelectedAmt--;
        if (currentSelectedAmt == 1) minusButton.interactable = false;
        if (maxAmt > currentSelectedAmt) plusButton.interactable = true;
        selectedAmount.text = currentSelectedAmt.ToString();
        UpdateTotal(currentSelectedAmt);
    }

    public void OnPlusClick()
    {
		int currentSelectedAmt = int.Parse(selectedAmount.text);
		int maxAmt = int.Parse(maxAmount.text);
		currentSelectedAmt++;
		if (currentSelectedAmt == 2) minusButton.interactable = true;
        if (currentSelectedAmt == maxAmt) plusButton.interactable = false;
		selectedAmount.text = currentSelectedAmt.ToString();
        UpdateTotal(currentSelectedAmt);
	}

    public void UpdateTotal(int selectedAmt)
    {
        int unitCost = int.Parse(unitPrice.text);
        int total = selectedAmt * unitCost;

		buyTotal.text = total.ToString();
        if(!sellOption) UpdatePriceTotalTextColor(total);

	}

    public void UpdatePriceTotalTextColor()
    {
		int currentSelectedAmt = int.Parse(selectedAmount.text);
		int unitCost = int.Parse(unitPrice.text);
		int total = currentSelectedAmt * unitCost;

        if(total > currentSave.gold)
        {
            buyTotal.color = cannotAffordTotalPriceColor;
        }
        else
        {
            buyTotal.color = normalTotalPriceColor;
        }
	}

	public void UpdatePriceTotalTextColor(int total)
	{
		if (total > currentSave.gold)
		{
			buyTotal.color = cannotAffordTotalPriceColor;
		}
		else
		{
			buyTotal.color = normalTotalPriceColor;
		}
	}

	public void OnBuy()
    {
        int totalCost = int.Parse(buyTotal.text);
        if(currentSave.gold >= totalCost)
        { // CAN BUY
			int currentSelectedAmt = int.Parse(selectedAmount.text);
			int maxAmt = int.Parse(maxAmount.text);
            maxAmt -= currentSelectedAmt;
            if(maxAmt > 0)
            { // still more left to buy
				selectedAmount.text = 1.ToString();
                maxAmount.text = maxAmt.ToString();
                buyTotal.text = unitPrice.text;
                minusButton.interactable = false;
                if(maxAmt > 1) plusButton.interactable = true; 
                else plusButton.interactable = false;

                loadedMerchant.vals.inventory[hoverableCollectible.CollectibleData.Name] = maxAmt;
			}
            else
            { // none left to buy
                loadedMerchant.vals.inventory.Remove(hoverableCollectible.CollectibleData.Name);
                Destroy(gameObject);
            }


            currentSave.gold -= totalCost;
            updatePlayerGold.Raise();

            loadedMerchant.vals.GainExp(Mathf.RoundToInt(totalCost * 1.5f));
            updateReputation.Raise();


            if(hoverableCollectible.CollectibleData is LootData)
            {
                lootLocker.AddToContainer(hoverableCollectible.CollectibleData, currentSelectedAmt);
                lootLocker.onContainerCollectibleUpdated.Raise();
            }
            else if (hoverableCollectible.CollectibleData is HatData)
            {
				hatLocker.AddToContainer(hoverableCollectible.CollectibleData, currentSelectedAmt);
                hatLocker.onContainerCollectibleUpdated.Raise();
            }
			else if (hoverableCollectible.CollectibleData is ItemData)
			{
				itemLocker.AddToContainer(hoverableCollectible.CollectibleData, currentSelectedAmt);
                itemLocker.onContainerCollectibleUpdated.Raise();
            }


            foreach(MerchantBuyOptionRefs refs in Market.buyOptions)
            {
                refs.UpdatePriceTotalTextColor();
            }
		}
	}

    public void OnSell()
    {
        int totalCost = int.Parse(buyTotal.text);
        int currentSelectedAmt = int.Parse(selectedAmount.text);
        int maxAmt = int.Parse(maxAmount.text);

        int amountLeftToFind = currentSelectedAmt;

        if (hoverableCollectible.CollectibleData is LootData || hoverableCollectible.CollectibleData is HatData)
        {
            for (int i = 0; i < backpack.container.collectibleSlots.Length; i++) // search backpack
            {
                if (backpack.container.collectibleSlots[i].Collectible == hoverableCollectible.CollectibleData)
                {
                    if (backpack.container.collectibleSlots[i].quantity > amountLeftToFind)
                    {
                        backpack.container.collectibleSlots[i].quantity -= amountLeftToFind;
                        amountLeftToFind = 0;
                        backpack.onContainerCollectibleUpdated.Raise();
                        break;
                    }
                    else if (backpack.container.collectibleSlots[i].quantity == amountLeftToFind)
                    {
                        backpack.container.collectibleSlots[i].quantity = 0;
                        amountLeftToFind = 0;
                        backpack.container.collectibleSlots[i].CollectibleName = null;
                        backpack.onContainerCollectibleUpdated.Raise();
                        break;
                    }
                    else if (backpack.container.collectibleSlots[i].quantity < amountLeftToFind)
                    {
                        amountLeftToFind -= backpack.container.collectibleSlots[i].quantity;
                        backpack.container.collectibleSlots[i].quantity = 0;
                        backpack.container.collectibleSlots[i].CollectibleName = null;
                        backpack.onContainerCollectibleUpdated.Raise();
                    }
                }
            }
        }

        if (amountLeftToFind > 0)
        {
            if (hoverableCollectible.CollectibleData is LootData)
            {
                for (int i = 0; i < lootLocker.container.collectibleSlots.Length; i++) // when more left to find, search locker
                {
                    if (lootLocker.container.collectibleSlots[i].Collectible == hoverableCollectible.CollectibleData)
                    {
                        if (lootLocker.container.collectibleSlots[i].quantity > amountLeftToFind)
                        {
                            lootLocker.container.collectibleSlots[i].quantity -= amountLeftToFind;
                            amountLeftToFind = 0;
                            lootLocker.onContainerCollectibleUpdated.Raise();
                            break;
                        }
                        else if (lootLocker.container.collectibleSlots[i].quantity == amountLeftToFind)
                        {
                            lootLocker.container.collectibleSlots[i].quantity = 0;
                            amountLeftToFind = 0;
                            lootLocker.container.collectibleSlots[i].CollectibleName = null;
                            lootLocker.onContainerCollectibleUpdated.Raise();
                            break;
                        }
                        else if (lootLocker.container.collectibleSlots[i].quantity < amountLeftToFind)
                        {
                            amountLeftToFind -= lootLocker.container.collectibleSlots[i].quantity;
                            lootLocker.container.collectibleSlots[i].quantity = 0;
                            lootLocker.container.collectibleSlots[i].CollectibleName = null;
                            lootLocker.onContainerCollectibleUpdated.Raise();
                        }
                    }
                }
            }
            else if (hoverableCollectible.CollectibleData is HatData)
            {
                for (int i = 0; i < hatLocker.container.collectibleSlots.Length; i++)
                {
                    if (hatLocker.container.collectibleSlots[i].Collectible == hoverableCollectible.CollectibleData)
                    {
                        if (hatLocker.container.collectibleSlots[i].quantity > amountLeftToFind)
                        {
                            hatLocker.container.collectibleSlots[i].quantity -= amountLeftToFind;
                            amountLeftToFind = 0;
                            hatLocker.onContainerCollectibleUpdated.Raise();
                            break;
                        }
                        else if (hatLocker.container.collectibleSlots[i].quantity == amountLeftToFind)
                        {
                            hatLocker.container.collectibleSlots[i].quantity = 0;
                            amountLeftToFind = 0;
                            hatLocker.container.collectibleSlots[i].CollectibleName = null;
                            hatLocker.onContainerCollectibleUpdated.Raise();
                            break;
                        }
                        else if (hatLocker.container.collectibleSlots[i].quantity < amountLeftToFind)
                        {
                            amountLeftToFind -= hatLocker.container.collectibleSlots[i].quantity;
                            hatLocker.container.collectibleSlots[i].quantity = 0;
                            hatLocker.container.collectibleSlots[i].CollectibleName = null;
                            hatLocker.onContainerCollectibleUpdated.Raise();
                        }
                    }
                }
                if(amountLeftToFind > 0)
                {
                    if (head.container.collectibleSlots[0].Collectible == hoverableCollectible.collectibleData)
                    {
                        head.container.collectibleSlots[0].quantity = 0;
                        amountLeftToFind = 0;
                        head.container.collectibleSlots[0].CollectibleName = null;
                        head.onContainerCollectibleUpdated.Raise();
                    }
                }
            }
            else if (hoverableCollectible.CollectibleData is ItemData)
            {
                for (int i = 0; i < itemLocker.container.collectibleSlots.Length; i++)
                {
                    if (itemLocker.container.collectibleSlots[i].Collectible == hoverableCollectible.CollectibleData)
                    {
                        if (itemLocker.container.collectibleSlots[i].quantity > amountLeftToFind)
                        {
                            itemLocker.container.collectibleSlots[i].quantity -= amountLeftToFind;
                            amountLeftToFind = 0;
                            itemLocker.onContainerCollectibleUpdated.Raise();
                            break;
                        }
                        else if (itemLocker.container.collectibleSlots[i].quantity == amountLeftToFind)
                        {
                            itemLocker.container.collectibleSlots[i].quantity = 0;
                            amountLeftToFind = 0;
                            itemLocker.container.collectibleSlots[i].CollectibleName = null;
                            itemLocker.onContainerCollectibleUpdated.Raise();
                            break;
                        }
                        else if (itemLocker.container.collectibleSlots[i].quantity < amountLeftToFind)
                        {
                            amountLeftToFind -= itemLocker.container.collectibleSlots[i].quantity;
                            itemLocker.container.collectibleSlots[i].quantity = 0;
                            itemLocker.container.collectibleSlots[i].CollectibleName = null;
                            itemLocker.onContainerCollectibleUpdated.Raise();
                        }
                    }
                }
				if (amountLeftToFind > 0)
				{
					if (hands.container.collectibleSlots[0].Collectible == hoverableCollectible.collectibleData)
					{
						hands.container.collectibleSlots[0].quantity = 0;
						amountLeftToFind = 0;
						hands.container.collectibleSlots[0].CollectibleName = null;
						hands.onContainerCollectibleUpdated.Raise();
                        sellLeftHand.Raise();
					}
				}
				if (amountLeftToFind > 0)
				{
					if (hands.container.collectibleSlots[1].Collectible == hoverableCollectible.collectibleData)
					{
						hands.container.collectibleSlots[1].quantity = 0;
						amountLeftToFind = 0;
						hands.container.collectibleSlots[1].CollectibleName = null;
						hands.onContainerCollectibleUpdated.Raise();
                        sellRightHand.Raise();
					}
				}
			}
        }

        maxAmt -= currentSelectedAmt;
        if (maxAmt > 0)
        { // still more left to sell
            selectedAmount.text = 1.ToString();
            maxAmount.text = maxAmt.ToString();
            buyTotal.text = unitPrice.text;
            minusButton.interactable = false;
            if (maxAmt > 1) plusButton.interactable = true;
            else plusButton.interactable = false;

        }
        else
        { // none left to sell
            Destroy(gameObject);
        }

        currentSave.gold += totalCost;
        updatePlayerGold.Raise();

        loadedMerchant.vals.GainExp(Mathf.RoundToInt(totalCost * 1.5f));
        updateReputation.Raise();

        // Add collectible to merchant inventory
        if (loadedMerchant.vals.inventory.ContainsKey(hoverableCollectible.CollectibleData.Name))
        {
            loadedMerchant.vals.inventory[hoverableCollectible.CollectibleData.Name] += currentSelectedAmt;
        }
        else // when not already in merchant inventory, add to inventory and upcharge from bought price
        {
            int unitCost = int.Parse(unitPrice.text);
            int merchantsPrice = Mathf.RoundToInt(unitCost * Random.Range(1.1f, 1.7f));

            loadedMerchant.vals.inventory.Add(hoverableCollectible.CollectibleData.Name, currentSelectedAmt);
            loadedMerchant.vals.prices.Add(hoverableCollectible.CollectibleData.Name, merchantsPrice);
        }

    }
}
