using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanBack : MonoBehaviour
{
    public FloorContainer vanContainer;
    public Vehicle vehicle;
    public SpriteRenderer vanRoofSprite;
    public Collider2D backCollider;

    // make sure collectibles with multiple colliders only have 1 that interacts with floor container's collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if player is driving vehicle, or vehicle is moving, dont do anything, dont want to pick stuff up by moving over it
        if (vehicle.Active) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            StopCoroutine("ShowRoof");
            StartCoroutine(HideRoof(2));
        }
        else if (vehicle.rb.velocity.magnitude > 0.2)
        {
            return;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("AirborneItem") || collision.gameObject.layer == LayerMask.NameToLayer("InteractableItem"))
        {
            GameObject itemObject = collision.gameObject;
            Item item = itemObject.GetComponent<Item>();
            if (item.currentState == Item.ItemState.HELD)
            {
                item.vanBack = this;
            }
            else
            {
                StartCoroutine(AddToBack(item));
                StartCoroutine(AddToFloorContainer(collision));
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            StartCoroutine(AddToFloorContainer(collision));
        }

    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine("HideRoof");
            StartCoroutine(ShowRoof(2));
        }
    }

    private IEnumerator HideRoof(float speed)
    {
        while(vanRoofSprite.color.a > 0.1)
        {
            vanRoofSprite.color = new Color(vanRoofSprite.color.r, vanRoofSprite.color.g, vanRoofSprite.color.b, vanRoofSprite.color.a - Time.deltaTime * speed);
            yield return null;
        }
    }

    private IEnumerator ShowRoof(float speed)
    {
        while (vanRoofSprite.color.a < 1)
        {
            vanRoofSprite.color = new Color(vanRoofSprite.color.r, vanRoofSprite.color.g, vanRoofSprite.color.b, vanRoofSprite.color.a + Time.deltaTime * speed);
            yield return null;
        }
    }

    public IEnumerator AddToBack(Item item)
    {
        yield return new WaitForSeconds(2);
        item.transform.parent = transform;
        item.AddToVan();
    }

    public IEnumerator AddToFloorContainer(Collider2D collision)
    {
        yield return new WaitForSeconds(2.2f);
        if(collision.bounds.Intersects(backCollider.bounds))
        {
            if(collision.TryGetComponent(out Collectible c))
            {
                collision.gameObject.transform.parent = transform;
                vanContainer.AddCollectibleToContainer(c);
            }
        }
    }
}
