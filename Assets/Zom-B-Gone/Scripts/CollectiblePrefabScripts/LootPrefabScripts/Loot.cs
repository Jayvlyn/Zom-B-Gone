using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Loot : MonoBehaviour, IInteractable
{
	[SerializeField] public LootData lootData;
	[SerializeField] public int lootCount;
	[SerializeField] private CollectibleContainerData containerToFill;
	private bool transferring = false;
	private bool pickupTransfer = false;

	private Transform transferTarget;
	private Vector3 transferPos;
	private Quaternion transferRot;
	private SpriteRenderer spriteRenderer;
	[SerializeField] private float transferSpeed = 8.0f;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if (pickupTransfer)
		{
			if (transferTarget == null)
			{
				PickupTransferPosition(transferPos, transferRot);
			}
			else
			{
				PickupTransferPosition(transferTarget.position, transferTarget.localRotation);
			}
		}

		else if (transferring)
		{
			if (transferTarget == null)
			{
				TransferPosition(transferPos, transferRot);
			}
			else
			{
				TransferPosition(transferTarget.position, transferTarget.localRotation);
			}
		}
	}

	public void StartTransferPosition(Transform target)
	{
		transferTarget = target;
		transferring = true;
	}

	public void StartTransferPosition(Vector3 position, Quaternion rotation)
	{
		transferTarget = null;
		transferRot = rotation;
		transferPos = position;
		transferring = true;
	}

	public void StartPickupTransferPosition(Transform target)
	{
		transferTarget = target;
		pickupTransfer = true;
	}

	public void StartPickupTransferPosition(Vector3 position, Quaternion rotation)
	{
		transferTarget = null;
		transferRot = rotation;
		transferPos = position;
		pickupTransfer = true;
	}

	private void PickupTransferPosition(Vector3 position, Quaternion rotation)
	{
		transform.position = Vector3.Lerp(transform.position, position, transferSpeed * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, transferSpeed * Time.deltaTime);

		if (Vector3.Distance(transform.position, position) < 0.05f)
		{
			pickupTransfer = false;
			containerToFill.AddToContainer(lootData, lootCount);
			Destroy(gameObject);
		}
	}

	private void TransferPosition(Vector3 position, Quaternion rotation)
	{
		transform.position = Vector3.Lerp(transform.position, position, transferSpeed * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, transferSpeed * Time.deltaTime);

		if (Vector3.Distance(transform.position, position) < 0.005f)
		{
			transferring = false;
			transform.position = position;
			transform.localRotation = rotation;
		}
	}

	public void DropLoot()
	{
		//Spawn loot in and then move forward in front of player

		//Transform playerT = gameObject.transform.parent;
		//gameObject.transform.parent = null;
		//gameObject.layer = LayerMask.NameToLayer("Interactable");
		//spriteRenderer.sortingOrder = -3;

		//StartTransferPosition(playerT.position + playerT.transform.up, transform.rotation);
	}

	public void Interact(Head head)
	{
		spriteRenderer.sortingOrder = -50;
		gameObject.transform.parent = head.gameObject.transform;
		gameObject.layer = LayerMask.NameToLayer("Default");
		StartPickupTransferPosition(head.hatTransform);
	}

	public void Interact(bool rightHand)
	{
		throw new System.NotImplementedException();
	}

}