using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantBuyOptionRefs : MonoBehaviour
{
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

    public CollectibleContainerData lootLocker;
    public CollectibleContainerData itemLocker;
    public CollectibleContainerData hatLocker;

	private void Awake()
	{
        UpdatePriceTotalTextColor();
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
        UpdatePriceTotalTextColor(total);

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
                Destroy(this);
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
}
