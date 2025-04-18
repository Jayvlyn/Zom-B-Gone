using UnityEngine;

public class GunMelee : MonoBehaviour
{
	public Weapon mainWeapon;
	public int damage = 30;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(mainWeapon.currentState == Item.ItemState.HELD && collision.CompareTag("Enemy"))
		{
			if(collision.gameObject.TryGetComponent(out Health h))
			{
				Vector2 popupVec = (collision.gameObject.transform.position - gameObject.transform.parent.position).normalized * 10;
				bool invert = false;
				if (popupVec.x < 0) invert = true;
				h.TakeDamage(damage, popupVec * 50, 20, false, popupVec, invert, 0);
			}
		}
	}
}
