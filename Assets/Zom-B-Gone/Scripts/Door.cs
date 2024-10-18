using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public HingeJoint2D hinge;
    public Rigidbody2D rb;
    public bool locked = false;

    private Vector3 basePos;
    private Quaternion baseRot;

    private void Start()
    {
        basePos = transform.position;
        baseRot = transform.rotation;
    }

    public void Interact(bool rightHand)
    {
        ToggleLock();
    }

    public void ToggleLock()
    {
        if (locked)
        { // unlock
            locked = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        { // lock
            locked = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.rotation = baseRot;
            transform.position = basePos;

        }

    }

    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}
