using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible List", menuName = "Collectibles/New Collectible List")]
public class CollectibleList : ScriptableObject
{
    public CollectibleData[] collectibles = new CollectibleData[0];
}
