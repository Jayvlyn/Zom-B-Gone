using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Collectibles/New Tool")]
public class ToolData : ItemData
{
	[Header("Tool Attributes")]
	public AudioClip toolSound;
	public bool playToolSoundOnUse = true;
	public float activateDelay = 3;
	public float timeActivated = 10;


	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
