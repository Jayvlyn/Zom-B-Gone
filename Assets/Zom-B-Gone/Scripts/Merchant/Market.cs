using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    [SerializeField] MarketData marketData;
    [SerializeField] GameObject buyOptionPrefab;
    private MerchantBuyOptionRefs buyOptionRefs;

    [Header("UI Refs")]
    [SerializeField] Image merchantImage;
    [SerializeField] TMP_Text merchantNameText;
    [SerializeField] SuperTextMesh merchantDialogue;
    [SerializeField] RectTransform sellingItemHolder;
    [SerializeField] TMP_Text goldCount;

    [HideInInspector] public MerchantData loadedMerchant;

	public void LoadMerchant(string name)
    {
        foreach (var merchant in marketData.merchants)
        {
            if (merchant.name == name)
            {
                loadedMerchant = merchant;
            }
        }

        merchantImage.sprite = loadedMerchant.merchantSprite;
        merchantNameText.text = loadedMerchant.merchantName;
        merchantNameText.color = loadedMerchant.nameColor;
        UpdatePlayerGoldText();

        MerchantSaySomething();

	}

    public void UpdatePlayerGoldText()
    {
		goldCount.text = SaveManager.currentSave.gold.ToString();
	}

    public void MerchantSaySomething()
    {
        int dialogueIndex = UnityEngine.Random.Range(0, loadedMerchant.dialogueOptions.Length);
        merchantDialogue.text = "<readDelay="+loadedMerchant.talkSpeed+">" + loadedMerchant.dialogueOptions[dialogueIndex];
        //merchantDialogue.Read();
    }

    public void OnOpenBuy()
    {
        for (int i = sellingItemHolder.childCount - 1; i >= 0; i--) Destroy(sellingItemHolder.GetChild(i).gameObject);

        foreach(CollectibleData collectible in loadedMerchant.vals.inventory.Keys)
        { 
            GameObject option = Instantiate(buyOptionPrefab, sellingItemHolder);
            MerchantBuyOptionRefs refs = option.GetComponent<MerchantBuyOptionRefs>();
            refs.loadedMerchant = loadedMerchant;

            refs.hoverableCollectible.CollectibleData = collectible;

			refs.unitPrice.text = loadedMerchant.vals.prices[collectible].ToString();

            refs.buyTotal.text = refs.unitPrice.text; // starts at one selected

            int maxAmt = loadedMerchant.vals.inventory[collectible];

			refs.maxAmount.text = maxAmt.ToString();
            if (maxAmt == 1) refs.plusButton.interactable = false;

            refs.minusButton.interactable = false;

        }
    }
}
