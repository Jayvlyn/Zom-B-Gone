using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Collectibles/New Consumable")]
public class ConsumableData : ItemData
{
	[Header("Consumable Attributes")]
	[Tooltip("Amount of time the consumable effect lasts (0 or negative for permanence)")] 
	public float effectTime;

    public float instantStaminaRecovery = 0;
    public float staminaRecoverySpeed = 0;
    public float moveSpeedMod = 0;

    public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
