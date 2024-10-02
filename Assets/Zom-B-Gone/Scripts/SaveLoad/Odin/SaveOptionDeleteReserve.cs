using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOptionDeleteReserve : MonoBehaviour
{
    public static GameObject reserve;

    public void ReserveThis()
    {
        reserve = this.gameObject;
    }
}
