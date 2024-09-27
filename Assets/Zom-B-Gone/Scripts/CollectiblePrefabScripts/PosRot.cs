using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PosRot
{
    public Vector2 position;
    public Quaternion rotation;

    public PosRot(Vector2 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
