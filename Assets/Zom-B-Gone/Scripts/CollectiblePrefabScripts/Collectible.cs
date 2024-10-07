using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour, IInteractable
{
	public Collider2D fullCollider;

	public virtual void Interact(bool rightHand)
    {
		if (addContainer != null)
		{
			StopCoroutine(addContainer);
			addContainer = null;
		}
		if (floorContainer)
        {
            PosRot posRot = new PosRot(transform.localPosition, transform.localRotation);
            floorContainer.RemoveFromContainer(posRot);
            floorContainer = null;
        }
    }
    public virtual void Interact(Head head)
    {
        if (addContainer != null)
        {
            StopCoroutine(addContainer);
            addContainer = null;
        }
        if (floorContainer)
        {
            PosRot posRot = new PosRot(transform.localPosition, transform.localRotation);
            floorContainer.RemoveFromContainer(posRot);
            floorContainer = null;
        }
    }

	public virtual IEnumerator AddToFloorContainer(Floor floor)
	{
		yield return new WaitForSeconds(2f);
		if (fullCollider.bounds.Intersects(floor.floorCollider.bounds))
		{
			fullCollider.gameObject.transform.parent = floor.floorCollider.transform;
			floor.floorContainer.AddCollectibleToContainer(this);
		}
	}

    public Coroutine addContainer = null;
	[HideInInspector] public FloorContainer floorContainer = null;
}
