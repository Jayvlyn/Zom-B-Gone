using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackOpener : MonoBehaviour
{
    [SerializeField] GameObject backpackUI;

    public void OnBackpackToggled()
    {
        if(backpackUI.activeSelf)
        {
            backpackUI.SetActive(false);
        }
        else
        {
            backpackUI.SetActive(true);
        }
    }
}
