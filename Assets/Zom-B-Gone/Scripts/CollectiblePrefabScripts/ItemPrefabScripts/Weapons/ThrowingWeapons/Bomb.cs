using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : ThrowingWeapon
{
    [SerializeField] private float fuseTime = 3.5f;
    [SerializeField] private float explosionRadius = 10f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField,Tooltip("If this is checked, will change bomb to sprite below when lit")] private bool hasLitSprite;
    [SerializeField] private Sprite litSprite;
    [SerializeField] bool bigExplosion = true;

    public override void Use()
    {
        base.Use();
        if(lastThrownItem is Bomb b)
        {
            ArmBomb(b);
        }
    }

    private void ArmBomb(Bomb bomb)
    {
        if (bomb.hasLitSprite) bomb.itemRenderer.sprite = bomb.litSprite;
        bomb.StartCoroutine(bomb.CountdownExplosion(bomb));

    }

    private IEnumerator CountdownExplosion(Bomb bomb)
    {
        yield return new WaitForSeconds(bomb.fuseTime);
        Utils.CreateExplosion(bomb.transform.position, bomb.explosionRadius, bomb.explosionForce, bomb.throwingWeaponData.damage * 10, bigExplosion);
        Destroy(bomb.gameObject);
    }


}
