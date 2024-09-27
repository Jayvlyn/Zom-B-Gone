using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour, IInteractable
{
    public virtual void Interact(bool rightHand)
    {
        if(floorContainer)
        {
            PosRot posRot = new PosRot(transform.localPosition, transform.localRotation);
            floorContainer.RemoveFromContainer(posRot);
            floorContainer = null;
        }
    }
    public virtual void Interact(Head head)
    {
        if (floorContainer)
        {
            PosRot posRot = new PosRot(transform.localPosition, transform.localRotation);
            floorContainer.RemoveFromContainer(posRot);
            floorContainer = null;
        }
    }
    [HideInInspector] public FloorContainer floorContainer = null;
}
