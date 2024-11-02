using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemData : CollectibleData
{
	[Header("Item Attributes")]
	// Value that determines the effect it has on the players movement when held, also determines throw damage and speed
	[Range(1, 20000), Tooltip("In grams")] public float weight; // grams
	public AudioClip throwSound;

	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}

