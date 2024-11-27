using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Collectibles/New Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
	[Header("Melee Properties")]
	public float staminaCost = 1;
	[Tooltip("Time in seconds to prepare swing")] public float prepSpeed = 0.2f;
	public AnimationCurve swingCurve;
	public AnimationCurve rotationCurve;
	public AnimationCurve prepSwingCurve;
	public AnimationCurve prepRotationCurve;
	public List<AudioClip> swingSounds;
	public List<AudioClip> hitSounds;
	public EffectData effect;


	public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
