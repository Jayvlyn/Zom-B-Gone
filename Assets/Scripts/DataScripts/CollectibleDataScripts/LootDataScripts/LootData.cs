using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot", menuName = "Collectibles/New Loot")]
public class LootData : CollectibleData
{
    [Header("Loot Attributes")]
    [SerializeField, Min(1)] private int maxStack = 64;

    public int MaxStack => maxStack;

    public override string GetInfoDisplayText()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();
        sb.Append("<color=green> Gold Value: ").Append(BaseValue).Append("</color>").AppendLine();

        return sb.ToString();
    }
}
