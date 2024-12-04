using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zone", menuName = "New Zone")]
public class ZoneData : ScriptableObject
{
    public LootTable lootTable;
    public ZoneTrack track;
    public Color globalLightColor = new Color (1,1,1,1);
    public float middayIntensity = 0.73f;
    public float midnightIntensity = 0.13f;

    public AnimationCurve spawnRate;

}
