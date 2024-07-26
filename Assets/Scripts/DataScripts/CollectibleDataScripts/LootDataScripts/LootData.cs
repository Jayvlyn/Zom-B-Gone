using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot", menuName = "Collectibles/New Loot")]
public class LootData : CollectibleData
{
    //[Header("Loot Attributes")]


    public override string GetInfoDisplayText()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

        return sb.ToString();
    }
}
