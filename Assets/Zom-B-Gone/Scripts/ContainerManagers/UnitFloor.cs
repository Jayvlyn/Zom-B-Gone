using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class UnitFloor : MonoBehaviour
{
    [SerializeField] VanBack vanBack;
    public FloorContainer floorContainer;

    private List<Collider2D> dealtWith = new List<Collider2D>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dealtWith.Contains(collision)) return; 

        dealtWith.Add(collision);

        if (collision.bounds.Intersects(vanBack.backCollider.bounds)) return;

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
                floorContainer.AddCollectibleToContainer(item);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            if (collision.TryGetComponent(out Collectible c))
            {
                floorContainer.AddCollectibleToContainer(c);
            }
        }
    }
}
