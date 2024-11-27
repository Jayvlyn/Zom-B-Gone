using CodeMonkey;
using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
	public EffectData effectData;
    public SpriteRenderer spriteRenderer;

	public Enemy effectedEnemy;
	public Health effectedEnemyHealth;

	public LayerMask enemyLm;

	public void Initialize(EffectData data, Enemy enemy)
	{
		effectData = data;
		spriteRenderer.sprite = effectData.effectSprite;
		effectedEnemy = enemy;
		effectedEnemy.activeEffect = this;
		effectedEnemyHealth = enemy.health;

		spriteRenderer.sortingOrder = effectedEnemy.spriteRenderer.sortingOrder;

		if(effectData.passiveDamageTick > 0 && effectData.passiveDamage > 0)
		{
			damageTimer = effectData.passiveDamageTick;
		}

		if(effectData.spreadTick > 0 && effectData.spreadChance > 0)
		{
			spreadTimer = effectData.spreadTick;
		}

		effectedEnemyHealth.TakeDamage(effectData.onSpreadDamage, Vector2.zero, 0, false, new Vector2(10, 10), false, 0, effectData.damageColor);

		if (effectData.initialSpread) StartCoroutine(Spread());

		StartCoroutine(DestroyRoutine());
		StartCoroutine(SpreadTimer());
	}

	float damageTimer = -10;
	float spreadTimer = -10;
	private void Update()
	{
		if(damageTimer != -10)
		{
			if(damageTimer <= 0)
			{
				effectedEnemyHealth.TakeDamage(effectData.passiveDamage, Vector2.zero, 0, false, new Vector2(10, 10), false, 0, effectData.damageColor);
				damageTimer = effectData.passiveDamageTick;
			}
			else
			{
				damageTimer -= Time.deltaTime;
			}
		}

		if (spreadTimer != -10)
		{
			if(spreadTimer <= 0)
			{
				StartCoroutine(Spread());
				spreadTimer = effectData.spreadTick;
			}
			else
			{
				spreadTimer -= Time.deltaTime;
			}
		}
	}


	private IEnumerator DestroyRoutine()
	{
		yield return new WaitForSeconds(effectData.effectDuration);
		effectedEnemy.activeEffect = null;
		Destroy(gameObject);
	}

	private IEnumerator SpreadTimer()
	{
		yield return new WaitForSeconds(effectData.spreadDuration);
		spreadTimer = -10;
	}

	private IEnumerator Spread()
	{
		Collider2D[] spreadingToEnemies = Physics2D.OverlapCircleAll(transform.position, effectData.spreadDistance, enemyLm);
		foreach (Collider2D c in spreadingToEnemies)
		{
			yield return new WaitForSeconds(0.02f);
			int roll = Random.Range(0, 100);
			if (roll <= effectData.spreadChance)
			{
				if (c.TryGetComponent(out Enemy e))
				{
					Utils.ApplyEffect(effectData, e);
				}
			}
		}
	}
}
