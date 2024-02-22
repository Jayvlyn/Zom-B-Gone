using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] protected float _damageMod = 1;
    [SerializeField] bool _destroyOnHit = true;

    public int FirearmDamage { get; set; }
    public float LifeSpan { get; set; }

    public Rigidbody2D _rb;

    void Update()
    {
        
    }

    void Start()
    {
        StartCoroutine(lifeStart());
        TryGetComponent(out Rigidbody2D _rb);
    }

    private IEnumerator lifeStart()
    {
        yield return new WaitForSeconds(LifeSpan);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Health targetHealth))
        {
            targetHealth.TakeDamage(FirearmDamage * _damageMod);
        }
        if(_destroyOnHit) Destroy(gameObject);
    }
}
