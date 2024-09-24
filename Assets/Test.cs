using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    private void OnLeftHand(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Debug.Log("yes");
        }
    }
}
