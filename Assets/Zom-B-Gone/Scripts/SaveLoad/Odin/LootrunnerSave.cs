using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootrunnerSave
{
    public int gold;
    public int kills;

    public ItemData leftHandItem;
    public ItemData rightHandItem;
    public HatData wornHat;

    public CollectibleContainer backpack;
    public CollectibleContainer hatLocker;
    public CollectibleContainer itemLocker;
    public CollectibleContainer lootLocker;

    public FloorContainer vanFloor;
    //public FloorContainer unitFloor;
}
