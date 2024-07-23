using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Collectibles/Items/New Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
	[Header("Melee Properties")]
	[SerializeField] public float staminaCost = 1;
	[SerializeField, Tooltip("Time in seconds to prepare swing")] public float prepSpeed = 0.2f;
	[SerializeField] public AnimationCurve swingCurve;
	[SerializeField] public AnimationCurve rotationCurve;
	[SerializeField] public AnimationCurve prepSwingCurve;
	[SerializeField] public AnimationCurve prepRotationCurve;

	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
