using UnityEngine;

public class GunMelee : MonoBehaviour
{
	public int damage = 30;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Enemy"))
		{
			if(collision.gameObject.TryGetComponent(out Health h))
			{
				Vector2 knockbackVec = (collision.gameObject.transform.position - gameObject.transform.parent.position).normalized * 10;
				bool invert = false;
				if (knockbackVec.x < 0) invert = true;
				h.TakeDamage(damage, knockbackVec, 20, false, knockbackVec, invert, 0);
			}
		}
	}
}
