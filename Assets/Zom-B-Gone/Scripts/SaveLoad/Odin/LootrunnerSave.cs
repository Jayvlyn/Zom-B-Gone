using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootrunnerSave
{
    public string name;

    public int gold = 1000;
    public int kills;

    public CollectibleContainer hands;
    public CollectibleContainer head;

    public CollectibleContainer backpack;
    public CollectibleContainer hatLocker;
    public CollectibleContainer itemLocker;
    public CollectibleContainer lootLocker;

    public SaveableFloor vanFloor;
    public SaveableFloor unitFloor;

    public List<MerchantVals> merchantVals;
}
