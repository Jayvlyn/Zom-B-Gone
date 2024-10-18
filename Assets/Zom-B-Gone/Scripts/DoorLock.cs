using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Door"))
        {
            collision.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            collision.gameObject.layer = LayerMask.NameToLayer("Door");
        }
    }
}
