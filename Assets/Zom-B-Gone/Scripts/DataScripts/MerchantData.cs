using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Merchant", menuName = "New Merchant")]
public class MerchantData : ScriptableObject
{
    public string merchantName;
    public Color nameColor;
    public Sprite merchantSprite;
    public float talkSpeed = 0.05f;
    public string[] dialogueOptions = new string[0];

    public bool dealsItems;
    public bool dealsLoot;
    public bool dealsHats;

    public LootTable lootTable;

    public MerchantVals vals;
}
