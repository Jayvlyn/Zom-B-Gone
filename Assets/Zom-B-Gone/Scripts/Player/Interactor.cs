using UnityEngine;

public interface IInteractable
{
    public void Interact(bool rightHand);
    public void Interact(Head head);
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 1.5f;

    [SerializeField] private LayerMask InteractionLm;

    private Hands hands;
    private Head head;
    private RaycastHit2D hit;
    //private Camera gameCamera;

    private void Awake()
    {
        //gameCamera = FindObjectOfType<Camera>();
        hands = GetComponent<Hands>();
        head = GetComponent<Head>();
    }

    public void Interact(bool rightHand)
    {
        hit = Physics2D.CircleCast(transform.position, _interactRange, Vector2.zero, 0, InteractionLm);

        if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactedObject))
        {
            if(interactedObject != null)
            {
                if(interactedObject is Item)
                {
                    interactedObject.Interact(rightHand);
                    if (rightHand) {
                        hands.RightObject = ((Component)interactedObject).gameObject; hands.UsingRight = true;
                    }
                    else {
                        hands.LeftObject = ((Component)interactedObject).gameObject; hands.UsingLeft = true;
                    }
                }
                if(interactedObject is Hat)
                {
                    GameObject newHat = ((Component)interactedObject).gameObject;

                    if(head.HatObject != null)
                    { // Remove current hat
                        head.HatObject.transform.parent = null;
                        head.HatObject.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -20;
                        head.HatObject.gameObject.layer = LayerMask.NameToLayer("Interactable");
                        head.HatObject.GetComponent<Hat>().StartTransferPosition(newHat.transform.position, newHat.transform.localRotation);
                        //head.hatObject.transform.position = newHat.transform.position;
                    }
                    // Wear new hat
                    interactedObject.Interact(head);
                }

                if(interactedObject is Loot)
                {
                    interactedObject.Interact(head);
                }
            }
        }
    }
}
