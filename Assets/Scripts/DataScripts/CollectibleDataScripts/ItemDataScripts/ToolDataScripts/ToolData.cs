using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwing Weapon", menuName = "Collectibles/Items/New Throwing Weapon")]
public class ToolData : ItemData
{

	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();
		sb.Append("<color=green> Gold Value: ").Append(BaseValue).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
