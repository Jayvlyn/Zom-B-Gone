using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableFloor
{
    public Dictionary<PosRot, int> collectibleDict;
    public CollectibleContainer floorContainer;
}
