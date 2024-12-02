using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyClickable : MonoBehaviour, IPointerClickHandler
{
	public Enemy attachedEnemy;
	public Health enemyHealth;

	public void OnPointerClick(PointerEventData eventData)
	{
		if(attachedEnemy.currentState != Enemy.State.DEAD)
		{
			bool crit = false;
			if (Random.Range(0, 10) == 0) crit = true;

			int damage = 50 + Random.Range(-11, 51);

			Vector2 kb = Random.insideUnitCircle * (damage) * 2;
			enemyHealth.TakeDamage(damage, Vector2.zero, 70 + damage, crit, default, false, 88);
		}
	}
}
