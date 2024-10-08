using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] Image merchantImage;
    [SerializeField] TMP_Text merchantNameText;
    [SerializeField] SuperTextMesh merchantDialogue;

    [HideInInspector] public MerchantData loadedMerchant;

    public void LoadMerchant(string name)
    {
        loadedMerchant = Resources.Load<MerchantData>(name);
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
}
