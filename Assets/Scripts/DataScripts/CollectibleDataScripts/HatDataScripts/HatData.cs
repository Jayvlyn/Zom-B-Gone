using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hat", menuName = "Collectibles/New Hat")]
public class HatData : CollectibleData
{
	[Header("Bufss / Effects")]
	public float swingTimeMultiplier = 1;
	public float damageMultiplier = 1;
	public float damageIncrease = 0;
	public float moveSpeedMod = 1;
	public float defense = 0;


	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();
		sb.Append("<color=green> Gold Value: ").Append(BaseValue).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
