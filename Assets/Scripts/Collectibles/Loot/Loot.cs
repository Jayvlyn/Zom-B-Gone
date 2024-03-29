using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot", menuName = "Loot/NewLoot")]
public class Loot : Collectible
{
    [Header("Loot Attributes")]
    [SerializeField, Min(0)] private int baseValue;
    [SerializeField, Min(1)] private int maxStack = 64;
    [SerializeField] private int rarity;

    public override string ColoredName
    {
        get
        {
            return Name;
        }
    }
    public int BaseValue => baseValue;
    public int MaxStack => maxStack;

    public override string GetInfoDisplayText()
    {
        StringBuilder sb = new StringBuilder();

        //sb.Append(Name).AppendLine();
        sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();
        sb.Append("<color=green> Gold Value: ").Append(BaseValue).Append("</color>").AppendLine();

        return sb.ToString();
    }
}
