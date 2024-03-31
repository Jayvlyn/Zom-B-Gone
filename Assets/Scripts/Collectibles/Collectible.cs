using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : ScriptableObject
{
    [Header("Collectible Attributes")]
    [SerializeField] private new string name = "Unnamed";
    [SerializeField] private string description = "";
    [SerializeField] private Rarity rarity = null;
    [SerializeField] private Sprite icon = null;

    public string Name => name;
    public string Description => description;
    public Rarity Rarity => rarity;

    public string ColoredName
    {
        get
        {
            string hexColor = ColorUtility.ToHtmlStringRGB(rarity.TextColor);
            return $"<color=#{hexColor}>{Name}</color>";
        }
    }
    public Sprite Icon => icon;
    public abstract string GetInfoDisplayText();
}
