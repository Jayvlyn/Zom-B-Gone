using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootrunnerSave
{
    public PlayerData playerData;

    public CollectibleContainer hands;
    public CollectibleContainer head;

    public CollectibleContainer backpack;
    public CollectibleContainer hatLocker;
    public CollectibleContainer itemLocker;
    public CollectibleContainer lootLocker;

    public CollectibleContainer workbenchInput;
    public CollectibleContainer workbenchOutput;

    public SaveableFloor vanFloor;
    public SaveableFloor unitFloor;

    public List<MerchantVals> merchantVals;
    public int marketDay;
    public int marketCycles;
}
