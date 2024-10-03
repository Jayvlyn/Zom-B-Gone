using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleData : ScriptableObject
{
    [Header("Collectible Attributes")]
    public new string name = "Unnamed";
    public string description = "";
	public Rarity rarity = null;
    public Sprite icon = null;
    [SerializeField, Min(1)] private int maxStack = 64;

    public int MaxStack => maxStack;
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
