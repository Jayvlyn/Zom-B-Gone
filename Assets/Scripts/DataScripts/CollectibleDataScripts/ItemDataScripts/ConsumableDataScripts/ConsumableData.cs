using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Collectibles/New Consumable")]
public class ConsumableData : ItemData
{
	[SerializeField, Tooltip("Amount of time the consumable effect lasts (0/negative for perminance)")]
	public float effectTime;

	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
