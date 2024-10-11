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

	private void Awake()
	{
        if (!sellOption) UpdatePriceTotalTextColor();
	}

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

        if(total > SaveManager.currentSave.gold)
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
		if (total > SaveManager.currentSave.gold)
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
        if(SaveManager.currentSave.gold >= totalCost)
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

                loadedMerchant.vals.inventory[hoverableCollectible.CollectibleData] = maxAmt;
			}
            else
            { // none left to buy
                loadedMerchant.vals.inventory.Remove(hoverableCollectible.CollectibleData);
                Destroy(gameObject);
            }


            SaveManager.currentSave.gold -= totalCost;
            updatePlayerGold.Raise();


            if(hoverableCollectible.CollectibleData is LootData)
            {
                lootLocker.AddToContainer(hoverableCollectible.CollectibleData, currentSelectedAmt);
            }
            else if (hoverableCollectible.CollectibleData is HatData)
            {
				hatLocker.AddToContainer(hoverableCollectible.CollectibleData, currentSelectedAmt);
			}
			else if (hoverableCollectible.CollectibleData is ItemData)
			{
				itemLocker.AddToContainer(hoverableCollectible.CollectibleData, currentSelectedAmt);
			}

		}
	}

    public void OnSell()
    {
        int totalCost = int.Parse(buyTotal.text);
        int currentSelectedAmt = int.Parse(selectedAmount.text);
        int maxAmt = int.Parse(maxAmount.text);

        int amountLeftToFind = currentSelectedAmt;

        if(hoverableCollectible.CollectibleData is LootData)
        {
            for(int i = 0; i < backpack.container.collectibleSlots.Length; i++) // search backpack
            {
                if(backpack.container.collectibleSlots[i].quantity > amountLeftToFind)
                {
                    backpack.container.collectibleSlots[i].quantity -= amountLeftToFind;
                    amountLeftToFind = 0;
                    break;
                }
                else if (backpack.container.collectibleSlots[i].quantity == amountLeftToFind)
                {
                    backpack.container.collectibleSlots[i].quantity = 0;
                    amountLeftToFind = 0;
                    backpack.container.collectibleSlots[i].CollectibleName = null;
                    break;
                }
                else if (backpack.container.collectibleSlots[i].quantity < amountLeftToFind)
                {
                    amountLeftToFind -= backpack.container.collectibleSlots[i].quantity;
                    backpack.container.collectibleSlots[i].quantity = 0;
                    backpack.container.collectibleSlots[i].CollectibleName = null;
                }
            }
            if (amountLeftToFind > 0)
            {
                for (int i = 0; i < lootLocker.container.collectibleSlots.Length; i++) // when more left to find, search locker
                {
                    if (lootLocker.container.collectibleSlots[i].quantity > amountLeftToFind)
                    {
                        lootLocker.container.collectibleSlots[i].quantity -= amountLeftToFind;
                        amountLeftToFind = 0;
                        break;
                    }
                    else if (lootLocker.container.collectibleSlots[i].quantity == amountLeftToFind)
                    {
                        lootLocker.container.collectibleSlots[i].quantity = 0;
                        amountLeftToFind = 0;
                        lootLocker.container.collectibleSlots[i].CollectibleName = null;
                        break;
                    }
                    else if (lootLocker.container.collectibleSlots[i].quantity < amountLeftToFind)
                    {
                        amountLeftToFind -= lootLocker.container.collectibleSlots[i].quantity;
                        lootLocker.container.collectibleSlots[i].quantity = 0;
                        lootLocker.container.collectibleSlots[i].CollectibleName = null;
                    }
                }
            }
        }
        else if(hoverableCollectible.CollectibleData is HatData)
        {
            for (int i = 0; i < hatLocker.container.collectibleSlots.Length; i++)
            {
                if (hatLocker.container.collectibleSlots[i].quantity > amountLeftToFind)
                {
                    hatLocker.container.collectibleSlots[i].quantity -= amountLeftToFind;
                    amountLeftToFind = 0;
                    break;
                }
                else if (hatLocker.container.collectibleSlots[i].quantity == amountLeftToFind)
                {
                    hatLocker.container.collectibleSlots[i].quantity = 0;
                    amountLeftToFind = 0;
                    hatLocker.container.collectibleSlots[i].CollectibleName = null;
                    break;
                }
                else if (hatLocker.container.collectibleSlots[i].quantity < amountLeftToFind)
                {
                    amountLeftToFind -= hatLocker.container.collectibleSlots[i].quantity;
                    hatLocker.container.collectibleSlots[i].quantity = 0;
                    hatLocker.container.collectibleSlots[i].CollectibleName = null;
                }
            }
        }
        else if (hoverableCollectible.CollectibleData is ItemData)
        {
            for (int i = 0; i < itemLocker.container.collectibleSlots.Length; i++)
            {
                if (itemLocker.container.collectibleSlots[i].quantity > amountLeftToFind)
                {
                    itemLocker.container.collectibleSlots[i].quantity -= amountLeftToFind;
                    amountLeftToFind = 0;
                    break;
                }
                else if (itemLocker.container.collectibleSlots[i].quantity == amountLeftToFind)
                {
                    itemLocker.container.collectibleSlots[i].quantity = 0;
                    amountLeftToFind = 0;
                    itemLocker.container.collectibleSlots[i].CollectibleName = null;
                    break;
                }
                else if (itemLocker.container.collectibleSlots[i].quantity < amountLeftToFind)
                {
                    amountLeftToFind -= itemLocker.container.collectibleSlots[i].quantity;
                    itemLocker.container.collectibleSlots[i].quantity = 0;
                    itemLocker.container.collectibleSlots[i].CollectibleName = null;
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


        SaveManager.currentSave.gold += totalCost;
        updatePlayerGold.Raise();

        // Add collectible to merchant inventory
        if(loadedMerchant.vals.inventory.ContainsKey(hoverableCollectible.CollectibleData))
        {
            loadedMerchant.vals.inventory[hoverableCollectible.CollectibleData] += currentSelectedAmt;
        }
        else // when not already in merchant inventory, add to inventory and upcharge from bought price
        {
            int unitCost = int.Parse(unitPrice.text);
            int merchantsPrice = Mathf.RoundToInt(unitCost * Random.Range(1.1f, 1.7f));

            loadedMerchant.vals.inventory.Add(hoverableCollectible.CollectibleData, currentSelectedAmt);
            loadedMerchant.vals.prices.Add(hoverableCollectible.CollectibleData, merchantsPrice);
        }

    }
}
