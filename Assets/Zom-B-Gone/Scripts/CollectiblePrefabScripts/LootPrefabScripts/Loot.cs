using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Loot : Collectible
{
    [HideInInspector] public LootData lootData;

	public int quantity;
	[SerializeField] private CollectibleContainerData containerToFill;

	private SpriteRenderer spriteRenderer;
	private float transferTime = .2f;


	private void Awake()
	{
        lootData = data as LootData;
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

        containerToFill.AddToContainer(lootData, quantity);
        Destroy(gameObject);
    }

    public override void Interact(bool rightHand, PlayerController playerController)
	{
        base.Interact(rightHand, playerController);
		gameObject.transform.parent = playerController.head.gameObject.transform;
		gameObject.layer = LayerMask.NameToLayer("Default");
		StartCoroutine(PickupTransferPosition(playerController.head.hatTransform));
	}
}