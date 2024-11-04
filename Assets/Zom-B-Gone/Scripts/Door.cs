using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public HingeJoint2D hinge;
    public SpringJoint2D spring;
    public Rigidbody2D rb;
    public bool locked = false;
    public AudioClip doorLockSound;
    public AudioSource audioSource;

    private Vector3 basePos;
    private Quaternion baseRot;

    private void Start()
    {
        basePos = transform.position;
        baseRot = transform.rotation;
    }

    public void Interact(bool rightHand, PlayerController playerController)
    {
        ToggleLock();
    }

    public void ToggleLock()
    {
        audioSource.PlayOneShot(doorLockSound);

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

	private void OnCollisionStay2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Enemy")
        {
            spring.frequency = 0.01f;
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
        spring.frequency = 1;
	}
}
