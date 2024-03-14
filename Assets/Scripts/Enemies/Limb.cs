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

    private bool fallOut = false;
    [SerializeField] float fallOutSpeed = 5.0f;

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
        if(fallOut)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, fallOutSpeed * Time.deltaTime);
            if(transform.localScale == Vector3.zero)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DetachFromOwner()
    {
        owner.limbs.Remove(this);
        GameObject detachBleeding = Instantiate(bleedParticles, detachPoint);
        detachBleeding.transform.position = detachPoint.position;
        GameObject attachBleeding = Instantiate(bleedParticles, attachPoint);
        attachBleeding.transform.position = attachPoint.position;
        RemoveAttacksFromOwner();
        if(transform.parent) transform.parent = null;
        fallOut = true;
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
