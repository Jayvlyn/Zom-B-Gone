using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestingDamage : MonoBehaviour
{
    private void Start()
    {
        //DamagePopup.Create(Vector3.zero, 200);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            bool isCriticalHit = Random.Range(0, 100) < 30;
            DamagePopup.Create(UtilsClass.GetMouseWorldPosition(), 100, default, isCriticalHit);
        }
    }
}
