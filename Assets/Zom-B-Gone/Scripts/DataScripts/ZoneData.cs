using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zone", menuName = "New Zone")]
public class ZoneData : ScriptableObject
{
    public LootTable lootTable;

    public AnimationCurve spawnRate;

}
