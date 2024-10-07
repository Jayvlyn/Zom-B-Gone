using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableFloor
{
    public Dictionary<string, int> collectibleDict;
    public CollectibleContainer floorContainer;
}
