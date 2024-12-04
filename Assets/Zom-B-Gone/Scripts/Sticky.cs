using UnityEngine;

public class Sticky : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D fullCollider;

    [HideInInspector] public bool stick = true;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(stick) Stick(collision.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(stick) Stick(collision.transform);
    }

    private void Stick(Transform parent)
    {
        transform.parent = parent;

        if(rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            fullCollider.isTrigger = true;   
        }

        stick = false;
    }
}
