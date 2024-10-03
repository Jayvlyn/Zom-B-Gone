using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;

[CreateAssetMenu(fileName = "New Test", menuName = "New Test")]
public class TestScriptableSerialized : SerializedScriptableObject
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
}
