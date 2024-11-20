using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public BulletData bulletData;
    private int currentPiercingPower;

    public int ProjectileWeaponDamage { get; set; }
    public float LifeSpan { get; set; }

    public Rigidbody2D rigidBody;
    public Collider2D bulletCollider;
    [HideInInspector] public Weapon shooter;
    
    private PlayerController playerController;
    private Head playerHead;

    void Start()
    {
        StartCoroutine(lifeStart());

        playerController = FindFirstObjectByType<PlayerController>();
        playerHead = playerController.gameObject.GetComponent<Head>();

        if(bulletData.spin > 0)
        {
            rigidBody.angularVelocity = bulletData.spin;
        }
    }

    private IEnumerator lifeStart()
    {
        currentPiercingPower = bulletData.piercingPower;
        yield return new WaitForSeconds(LifeSpan);
        Destroy(gameObject);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(rigidBody.linearVelocity.magnitude > 3)
        {
			if (collision.CompareTag("Enemy"))
            {
                bulletData.enterEvent.Raise(transform);
                if (currentPiercingPower > 0) bulletData.exitEvent.Raise(transform);

                if (collision.gameObject.TryGetComponent(out Enemy enemy))
                {
                    currentPiercingPower--;
                    DealDamage(enemy.health);

                    if(bulletData.effectData)
                    {
                        Utils.ApplyEffect(bulletData.effectData, enemy);
					}
                }
                
            }
            else if (collision.gameObject.TryGetComponent(out Health targetHealth))
            {
                currentPiercingPower--;
                DealDamage(targetHealth);
				
			}
            else // hit surface
            {
                if (bulletData.wallPiercing) currentPiercingPower--;
                else if(!bulletData.residual)
                {
                    Destroy(gameObject);
                }
            }


            if (currentPiercingPower < 0 && !bulletData.residual) Destroy(gameObject);
        }
    }

    protected void DealDamage(Health targetHealth)
    {
        float damage = ProjectileWeaponDamage * bulletData.damageMultiplier;
        shooter.DealDamage(targetHealth, damage);
    }
}
