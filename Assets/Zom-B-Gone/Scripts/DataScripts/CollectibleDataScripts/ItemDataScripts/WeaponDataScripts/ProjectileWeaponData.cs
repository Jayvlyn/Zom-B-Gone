using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Weapon", menuName = "Collectibles/New Projectile Weapon")]
public class ProjectileWeaponData : WeaponData
{
	[Header("Projectile Weapon attributes")]
	public float range = 10; // m / km
	public int maxAmmo = 10;
	public int ammoConsumption = 1;
	public float reloadTime = 1; // Seconds
	public float fireForce = 25; // Basically bullet speed
    public bool isAutomatic; // Semi-Automatic or Automatic Gun?
    public GameObject bulletPrefab;
	public List<AudioClip> shootSounds;
	public AudioClip reloadStart;

    public override string GetInfoDisplayText()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<color=grey>").Append(Description).Append("</color>").AppendLine();

		return sb.ToString();
	}
}
