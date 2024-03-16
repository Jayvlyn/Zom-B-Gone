using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public Hat wornHat;
    public GameObject hatObject;
    public GameObject HatObject
    {
        get { return hatObject; }
        set { 
            hatObject = value;
            if (hatObject.TryGetComponent(out Hat hat)) wornHat = hat;
        }
    }
    public Transform hatPosition;
}
