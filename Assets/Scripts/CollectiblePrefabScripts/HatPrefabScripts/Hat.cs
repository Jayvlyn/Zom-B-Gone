using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour, IInteractable
{
    [SerializeField] public HatData hatData;
    protected Head head;
    private bool transferring = false;

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
        if(transferring)
        {
            if(transferTarget == null)
            {
                TransferPosition(transferPos, transferRot);
            }
            else
            {
                TransferPosition(transferTarget.position, transferTarget.localRotation);
            }
        }
    }

    public void Interact(Head head)
    {
		head.HatObject = gameObject;
		spriteRenderer.sortingOrder = 1;
		gameObject.transform.parent = head.gameObject.transform;
		this.head = head;
        gameObject.layer = LayerMask.NameToLayer("Default");
        StartTransferPosition(head.hatTransform);
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

	private void TransferPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = Vector3.Lerp(transform.position, position, transferSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, transferSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, position) < 0.05f)
        {
            transferring = false;
            transform.position = position;
            transform.localRotation = rotation;
        }
    }

    public void DropHat()
    {
        if (head != null)
        {
            head.HatObject = null;
            gameObject.transform.parent = null;
			gameObject.layer = LayerMask.NameToLayer("Interactable");
			spriteRenderer.sortingOrder = -1;

            StartTransferPosition(head.gameObject.transform.position + head.gameObject.transform.up, transform.rotation);
			//head.gameObject.transform.forward
		}
    }

	public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }
}
