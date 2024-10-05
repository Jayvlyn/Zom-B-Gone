using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanFloor : Floor
{
	public Vehicle vehicle;
	public SpriteRenderer vanRoofSprite;

	private Coroutine hideRoofCoroutine;
	private Coroutine showRoofCoroutine;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (vehicle.Active) return;

		if (collision.CompareTag("Player"))
		{
			PlayerController pc = collision.GetComponent<PlayerController>();

			if (showRoofCoroutine != null)
			{
				StopCoroutine(showRoofCoroutine);
				showRoofCoroutine = null;
			}

			if (hideRoofCoroutine == null && gameObject.activeInHierarchy)
			{
				hideRoofCoroutine = StartCoroutine(HideRoof(2));
			}
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
			if (hideRoofCoroutine != null)
			{
				StopCoroutine(hideRoofCoroutine);
				hideRoofCoroutine = null;
			}

			if (showRoofCoroutine == null && gameObject.activeInHierarchy)
			{
				showRoofCoroutine = StartCoroutine(ShowRoof(2));
			}
		}
	}

	private IEnumerator HideRoof(float speed)
	{
		while (vanRoofSprite.color.a > 0.1f)
		{
			vanRoofSprite.color = new Color(vanRoofSprite.color.r, vanRoofSprite.color.g, vanRoofSprite.color.b, vanRoofSprite.color.a - Time.deltaTime * speed);
			yield return null;
		}

		hideRoofCoroutine = null;
	}

	private IEnumerator ShowRoof(float speed)
	{
		while (vanRoofSprite.color.a < 1f)
		{
			vanRoofSprite.color = new Color(vanRoofSprite.color.r, vanRoofSprite.color.g, vanRoofSprite.color.b, vanRoofSprite.color.a + Time.deltaTime * speed);
			yield return null;
		}

		showRoofCoroutine = null;
	}

	public IEnumerator AddToBack(Item item)
	{
		yield return new WaitForSeconds(2);
		item.AddToFloor();
	}
}
