using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Loot : MonoBehaviour, IInteractable
{
	[SerializeField] public LootData lootData;
	[SerializeField] public int lootCount;
	[SerializeField] private CollectibleContainerData containerToFill;

	private SpriteRenderer spriteRenderer;
	private float transferTime = .2f;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = lootData.icon;
	}

	public IEnumerator TransferPosition(Vector3 position, Quaternion rotation)
	{
        float elapsedTime = 0f;
		Vector3 startPosition = transform.position;
		Quaternion startRotation = transform.localRotation;

        while (elapsedTime < transferTime)
        {
            transform.position = Vector3.Lerp(startPosition, position, elapsedTime / transferTime);
            transform.localRotation = Quaternion.Lerp(startRotation, rotation, elapsedTime / transferTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
        transform.localRotation = rotation;
    }

    private IEnumerator PickupTransferPosition(Transform target)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < transferTime)
        {
            transform.position = Vector3.Lerp(startPosition, target.position, elapsedTime / transferTime);
            transform.localRotation = Quaternion.Lerp(startRotation, target.rotation, elapsedTime / transferTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        containerToFill.AddToContainer(lootData, lootCount);
        Destroy(gameObject);
    }

    public void Interact(Head head)
	{
		gameObject.transform.parent = head.gameObject.transform;
		gameObject.layer = LayerMask.NameToLayer("Default");
		StartCoroutine(PickupTransferPosition(head.hatTransform));
	}

	public void Interact(bool rightHand)
	{
		throw new System.NotImplementedException();
	}
}