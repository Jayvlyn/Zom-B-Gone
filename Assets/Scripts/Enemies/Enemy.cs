using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D _rigidBody;

    [Header("Properties")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 7;
    [SerializeField] private float _crawlSpeed = 2;
    [SerializeField] private float _attackDamage = 10;
    [SerializeField] private float _attacksPerSecond = 1;


    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }
}
