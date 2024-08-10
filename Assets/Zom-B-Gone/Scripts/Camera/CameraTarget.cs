using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform crosshair;
    [SerializeField] private Transform player;

    void Update()
    {
        transform.position =  (crosshair.position + player.position) * 0.3f;
    }
}
