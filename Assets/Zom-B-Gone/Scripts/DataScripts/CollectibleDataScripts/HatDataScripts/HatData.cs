using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hat", menuName = "Collectibles/New Hat")]
public class HatData : CollectibleData
{
	[Header("Visual")]
	public bool showHair = true;
	[Header("Buffs / Effects")]
	public float swingTimeMultiplier = 1;
	public float damageMultiplier = 1;
	public float damageIncrease = 0;
	public float moveSpeedMod = 1;
	public int defense = 0;
	public bool camo = false;


	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
