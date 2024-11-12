using UnityEngine;

public class Glass : MonoBehaviour
{
    public Window window;
    public float breakRequirement = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > breakRequirement)
        {
            Vector2 velDir = collision.relativeVelocity.normalized;
            Vector2 upDir = transform.parent.up.normalized;
            float dot = Vector2.Dot(velDir, upDir);
            if(dot>0) window.ShatterGlass(false);
            else window.ShatterGlass(true);
            
        }
    }
}
