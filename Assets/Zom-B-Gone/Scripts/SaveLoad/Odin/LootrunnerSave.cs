using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootrunnerSave
{
    public string name;

    public int gold;
    public int kills;

    public CollectibleContainer hands;
    public CollectibleContainer head;

    public CollectibleContainer backpack;
    public CollectibleContainer hatLocker;
    public CollectibleContainer itemLocker;
    public CollectibleContainer lootLocker;

    public FloorContainer vanFloor;
    //public FloorContainer unitFloor;
}
