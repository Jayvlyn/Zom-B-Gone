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
        merchantDialogue.readDelay = loadedMerchant.talkSpeed;

        MerchantSaySomething();
    }

    public void MerchantSaySomething()
    {
        int dialogueIndex = Random.Range(0, loadedMerchant.dialogueOptions.Length);
        merchantDialogue.text = loadedMerchant.dialogueOptions[dialogueIndex];
        merchantDialogue.Read();
    }

    public void OnOpenBuy()
    {
        for (int i = sellingItemHolder.childCount - 1; i >= 0; i--) Destroy(sellingItemHolder.GetChild(i).gameObject);

        int optionsCount = loadedMerchant.vals.buyOffers.Length;
        for (int i = 0; i < optionsCount; i++)
        {
            GameObject option = Instantiate(buyOptionPrefab, sellingItemHolder);
            MerchantBuyOptionRefs refs = option.GetComponent<MerchantBuyOptionRefs>();
            CollectibleData thisCollectible = loadedMerchant.vals.buyOffers[i];
            refs.collectibleImage.sprite = thisCollectible.Icon;

            refs.unitPrice.text = loadedMerchant.vals.prices[thisCollectible].ToString();
            refs.buyTotal.text = refs.unitPrice.text; // starts at one selected

            refs.maxAmount.text = loadedMerchant.vals.inventory[thisCollectible].ToString();

            refs.minusButton.interactable = false;
        }
    }
}
