using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Limb : MonoBehaviour
{
    [SerializeField] private List<GameObject> attacks;
    [SerializeField] private Transform detachPoint;
    [SerializeField] private Transform attachPoint;
    [SerializeField] private GameObject bleedParticles;

    [NonSerialized] public Enemy owner;
    [NonSerialized] public Rigidbody2D rb;

    private float detachedLifetime = 1.0f;
    private float disappearSpeed = 8.0f;
    private bool detached = false;
    private float detachedTimer;
    private GameObject detachBleeding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(transform.parent.gameObject.TryGetComponent(out Enemy owner))
        {
            this.owner = owner;
        }   
    }

    private void Update()
    {
        if(detached)
        {
            if(detachedTimer > 0) detachedTimer -= Time.deltaTime;
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, disappearSpeed * Time.deltaTime);
                detachBleeding.transform.localScale = Vector3.Lerp(detachBleeding.transform.localScale, Vector3.zero, disappearSpeed * Time.deltaTime);
                if (transform.localScale.x < 0.3)
                {
                    Debug.Log("destroyed");
                    Destroy(gameObject);
                }
            }
        }
    }

    public void DetachFromOwner()
    {
        owner.limbs.Remove(this);
        detachBleeding = Instantiate(bleedParticles, detachPoint);
        detachBleeding.transform.position = detachPoint.position;
        GameObject attachBleeding = Instantiate(bleedParticles, attachPoint);
        attachBleeding.transform.position = attachPoint.position;
        owner.bleedingParticles.Add(attachBleeding);
        RemoveAttacksFromOwner();
        if(transform.parent) transform.parent = null;

        detachedTimer = detachedLifetime;
        detached = true;
    }

    public void AddAttacksToOwner()
    {
        foreach (var attack in attacks)
        {
            owner.attacks.Add(attack);
        }
    }

    public void RemoveAttacksFromOwner()
    {
        foreach (var attack in attacks)
        {
            owner.attacks.Remove(attack);
        }
    }

}
