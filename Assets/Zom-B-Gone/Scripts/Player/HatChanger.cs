using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatChanger : MonoBehaviour
{
    public CollectibleContainerSlot headSlot;

    public void CheckHatChange()
    {
        if(headSlot == null)
        {
            if(headSlot.SlotCollectible.name != headSlot.containerData.Container.collectibleSlots[0].collectible.name)
            {

            }
        }
    }
}
