using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFloor : Floor
{
    [SerializeField] VanFloor vanBack;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeSinceAwake < .1) return; // dont re-add initialized collectibles
        if (collision.bounds.Intersects(vanBack.floorCollider.bounds)) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("AirborneItem") || collision.gameObject.layer == LayerMask.NameToLayer("InteractableItem") || collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            Collectible c = collision.gameObject.GetComponent<Collectible>();
			if (c.addContainer != null)
			{
				c.StopCoroutine(c.addContainer);
                c.addContainer = null;
			}

			if (collision.gameObject.layer == LayerMask.NameToLayer("AirborneItem") || collision.gameObject.layer == LayerMask.NameToLayer("InteractableItem"))
            {
                Item item = c as Item;
                if (item.currentState == Item.ItemState.HELD)
                {
                    item.unitFloor = this;
                }
                else
                {
				    item.addContainer = item.StartCoroutine(item.AddToFloorContainer(this));
                }
            }
			else
            {
				c.addContainer = c.StartCoroutine(c.AddToFloorContainer(this));
			    
            }
        }

    }
}
