using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanBack : MonoBehaviour
{
    public Vehicle vehicle;
    public SpriteRenderer vanRoofSprite;
    public Collider2D backCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (vehicle.Active) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            if (pc.currentState != PlayerController.PlayerState.DRIVING) StartCoroutine(HideRoof(2));
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
                StopAllCoroutines();
                StartCoroutine(AddToBack(item));
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            collision.gameObject.transform.parent = transform;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
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
}
