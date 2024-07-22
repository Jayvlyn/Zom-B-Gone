using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleData : ScriptableObject
{
    [Header("Collectible Attributes")]
    [SerializeField] private new string name = "Unnamed";
    [SerializeField] private string description = "";
	[SerializeField, Min(0)] private int baseValue;
	[SerializeField] private Rarity rarity = null;
    [SerializeField] private Sprite icon = null;

	public string Name => name;
    public string Description => description;
	public int BaseValue => baseValue;
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
