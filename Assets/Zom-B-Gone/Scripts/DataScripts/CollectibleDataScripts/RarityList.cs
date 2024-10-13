using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity List", menuName = "Collectibles/New Rarity List")]
public class RarityList : ScriptableObject
{
    public Rarity[] rarities = new Rarity[5];
}
