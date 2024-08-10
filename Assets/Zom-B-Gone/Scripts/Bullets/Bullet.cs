using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] public BulletData bulletData;
    private int currentPiercingPower;

    public int ProjectileWeaponDamage { get; set; }
    public float LifeSpan { get; set; }

    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public Collider2D bulletCollider;
    [HideInInspector] public Weapon shooter;

    private PlayerController playerController;
    private Head playerHead;

    void Start()
    {
        StartCoroutine(lifeStart());
        rigidBody = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();

        playerController = FindObjectOfType<PlayerController>();
        playerHead = playerController.GetComponentInParent<Head>();

        if (bulletData.piercingPower > 0)
        {
            bulletCollider.isTrigger = true;
        }
    }

    private IEnumerator lifeStart()
    {
        currentPiercingPower = bulletData.piercingPower;
        yield return new WaitForSeconds(LifeSpan);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision.collider);
    }

	private void OnTriggerEnter2D(Collider2D collider)
	{
        OnHit(collider);
	}

    private void OnHit(Collider2D collision)
    {
		if (collision.gameObject.TryGetComponent(out Health targetHealth))
		{
			currentPiercingPower--;
			DealDamage(targetHealth);
		}
		else if (collision.gameObject.layer == LayerMask.NameToLayer("World"))
		{
			if (bulletData.wallPiercing) currentPiercingPower--;
			else Destroy(gameObject);
		}
		if (currentPiercingPower < 0) Destroy(gameObject);
	}

    protected void DealDamage(Health targetHealth)
    {
        float damage = ProjectileWeaponDamage * bulletData.damageMultiplier;
        #region hat buff
        if (playerHead.wornHat != null)
        {
            damage += playerHead.wornHat.hatData.damageIncrease;
            damage *= playerHead.wornHat.hatData.damageMultiplier;
        }
        #endregion
        targetHealth.TakeDamage(damage, shooter.weaponData.dismemberChance);
        //shooter.TryDealKnockback(targetHealth); Uses its own knockback logic now
        TryDealKnockback(targetHealth);
    }

    protected void TryDealKnockback(Health targetHealth)
    {
        if (targetHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb)) hitRb.AddForce(rigidBody.velocity.normalized * shooter.weaponData.knockbackPower, ForceMode2D.Impulse);
    }
}
