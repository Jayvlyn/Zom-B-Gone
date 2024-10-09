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

    [Header("Changing Vals")]
    [HideInInspector] public int reputationLevel;
    [HideInInspector] public int reputationExp;
    [HideInInspector] public CollectibleData[] buyOffers;
    public Dictionary<CollectibleData, int> inventory; // collectible and amount
    public Dictionary<CollectibleData, int> prices; // collectible and price
}
