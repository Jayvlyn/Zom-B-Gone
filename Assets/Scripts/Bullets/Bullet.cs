using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] protected float _damageMod = 1;
    [SerializeField] bool wallPiercing = false;
    [SerializeField] int piercingPower = 0; // How many enemies the bullet can pierce through
    int currentPiercingPower;

    public int FirearmDamage { get; set; }
    public float LifeSpan { get; set; }

    public Rigidbody2D _rb;
    public Collider2D bulletCollider;
    public Weapon shooter;

    private PlayerController playerController;
    private Head playerHead;

    void Start()
    {
        StartCoroutine(lifeStart());
        TryGetComponent(out Rigidbody2D _rb);
        TryGetComponent(out Collider2D collider);

        playerController = FindObjectOfType<PlayerController>();
        playerHead = playerController.GetComponentInParent<Head>();

        if (piercingPower > 0)
        {
            collider.isTrigger = true;
        }
    }

    private IEnumerator lifeStart()
    {
        currentPiercingPower = piercingPower;
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
			if (wallPiercing) currentPiercingPower--;
			else Destroy(gameObject);
		}
		if (currentPiercingPower < 0) Destroy(gameObject);
	}

    protected void DealDamage(Health targetHealth)
    {
        float damage = FirearmDamage * _damageMod;
        #region hat buff
        if (playerHead.wornHat != null)
        {
            damage += playerHead.wornHat.hatData.damageIncrease;
            damage *= playerHead.wornHat.hatData.damageMultiplier;
        }
        #endregion
        targetHealth.TakeDamage(damage, shooter.weaponData.dismembering);
    }
}
