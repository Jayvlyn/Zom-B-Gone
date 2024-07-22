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
    [SerializeField] private float transferSpeed = 8.0f;

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
        this.head = head;
        StartTransferPosition(head.hatPosition);
        gameObject.layer = LayerMask.NameToLayer("Default");
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

    public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }
}
