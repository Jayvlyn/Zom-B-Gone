using UnityEngine;

public class Glass : MonoBehaviour
{
    public Window window;
    public float breakRequirement = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            if (collision.relativeVelocity.magnitude > breakRequirement) window.ShatterGlass();
        }
    }
}
