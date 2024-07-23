using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WeaponData : ItemData
{
	[Header("Weapon attributes")]
	[SerializeField] public int damage;
	[SerializeField] public float attackSpeed; // time between attacks
	[SerializeField] public float knockbackPower;
	[SerializeField] public bool dismembering = false;

	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}

