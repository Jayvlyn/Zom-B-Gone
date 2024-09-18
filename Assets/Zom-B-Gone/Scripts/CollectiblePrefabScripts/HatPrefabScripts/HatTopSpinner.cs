using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatTopSpinner : MonoBehaviour
{
    public Hat hat;
    public float spinSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        if(hat.head)
        {
            transform.Rotate(0,0,Time.deltaTime * spinSpeed);
        }
    }
}
