using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Firearm", menuName = "Collectibles/Items/New Firearm")]
public class FirearmData : WeaponData
{
	[Header("Firearm attributes")]
	public float range = 10; // m / km
	public int maxAmmo = 10;
	public int ammoConsumption = 1;
	public float reloadTime = 1; // Seconds
	public float fireForce = 25;

	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
