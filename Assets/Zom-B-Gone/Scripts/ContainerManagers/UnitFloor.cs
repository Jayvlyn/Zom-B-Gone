using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFloor : Floor
{
    [SerializeField] VanFloor vanBack;

    private List<Collider2D> dealtWith = new List<Collider2D>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dealtWith.Contains(collision)) return; 

        dealtWith.Add(collision);

        if (collision.bounds.Intersects(vanBack.floorCollider.bounds)) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("AirborneItem") || collision.gameObject.layer == LayerMask.NameToLayer("InteractableItem"))
        {
            GameObject itemObject = collision.gameObject;
            Item item = itemObject.GetComponent<Item>();
            if (item.currentState == Item.ItemState.HELD)
            {
                item.unitFloor = this;
            }
            else
            {
				StartCoroutine(AddToFloorContainer(collision));
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            if (collision.TryGetComponent(out Collectible c))
            {
				StartCoroutine(AddToFloorContainer(collision));
			}
        }
    }
}
