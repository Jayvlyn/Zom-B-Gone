using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Floor Container", menuName = "New Floor Container")]
public class FloorContainerData : CollectibleContainerData
{
    // int is index in the container, PosRot is the local position and rotation of the collectibles on the floor
    public Dictionary<string, int> collectibleDict = new Dictionary<string, int>();

    public int collectibleCount = 0;

    public void ClearFloorSpecificVals()
    {
        collectibleCount = 0;
        collectibleDict = new Dictionary<string, int>();
    }
}
