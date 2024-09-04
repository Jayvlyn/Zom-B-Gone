using Unity.VisualScripting;
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

    [SerializeField] private float scanInterval = .1f;
    private float scanTimer;

    [SerializeField] private float openContainerDistanceCheckInterval = 0.1f;
    private float distanceCheckTimer;

    [SerializeField] Material defaultSpriteMaterial;
    [SerializeField] Material outlineSpriteMaterial;


    private Hands hands;
    private Head head;
    private RaycastHit2D hit;
    //private Camera gameCamera;

    // when a container is interacted with, it will use this to track the distance, and close the container when you get out of interact range
    private GameObject interactedContainer;

    private SpriteRenderer interactableSpriteRenderer;
    private IInteractable availableInteractable;
    public IInteractable AvailableInteractable
    {
        get { return availableInteractable; }
        
        set
        {
            if (availableInteractable == value) return; // dont set same value

            // if previous value set to interactable, remove highlight first
            if(availableInteractable != null)
            { // remove highlight from old available
                interactableSpriteRenderer.material = defaultSpriteMaterial;
            }

            availableInteractable = value;

            if (availableInteractable != null)
            { // new available interactable is not null
                interactableSpriteRenderer = ((Component)availableInteractable).gameObject.GetComponent<SpriteRenderer>();
                //set highlight
                interactableSpriteRenderer.material = outlineSpriteMaterial;
            }
        }
    }

    private void Awake()
    {
        //gameCamera = FindObjectOfType<Camera>();
        hands = GetComponent<Hands>();
        head = GetComponent<Head>();
    }

    private void Update()
    {
        //if(!hands.UsingLeft || !hands.UsingRight)
        //{ // at least one hand available for interact, do scan


        if (scanTimer <= 0)
        {
            scanTimer = scanInterval;

            RaycastHit2D clostestHit = InteractableScan();
            if(clostestHit.collider != null)
            {
                AvailableInteractable = clostestHit.collider.GetComponent<IInteractable>();
            }
            else
            {
                AvailableInteractable = null;
            }

        }
        else
        {
            scanTimer -= Time.deltaTime;
        }



    }

    /// <summary>
    /// Finds interactables around interactor. 
    /// <para>First checks if mouse is within interact range, if so it will find the interactable closest to the mouse.</para>
    /// <para>Otherwise it will find the interactable closest to the interactor.</para>
    /// </summary>
    /// <returns>Closest hit, if there are no hits it will return first hit with null collider</returns>
    public RaycastHit2D InteractableScan()
    {
        // NEED TO ADD WALL DETECTION TO THIS
        /**
         * For wall check:
         * 
         * shoot three rays from interactor, one from left, center, and right of interactor, all towards the interactable that was hit in the circle cast
         * if all three rays are blocked by environment, dont add as possible interaction
         * only add as possible interaction when at least one of the rays was able to reach the interactable without obstruction
         * this will stop interactions through walls, and when things are blocked by large obstacles, but wont interfere with nearby interacitons only blocked by small objects or corners
        */

        // get all interactables within interactRange
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, _interactRange, Vector2.zero, 0, InteractionLm);

        // quick exits
        if (hits.Length == 0) return new RaycastHit2D(); 
        else if (hits.Length == 1) return hits[0];

        // Handle more than 1 interactable in interact range
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseToInteractorDist = (mousePosition - new Vector2(transform.position.x, transform.position.y)).magnitude;

        RaycastHit2D closestHit = hits[0];
        float closestHitDist;
        if(mouseToInteractorDist <= _interactRange)
        { // mouse in interact range, find interactable closest to mouse
            closestHitDist = (mousePosition - new Vector2(closestHit.transform.position.x, closestHit.transform.position.y)).magnitude;
            for (int i = 1; i < hits.Length; i++)
            {
                float mouseToHitDist = (mousePosition - new Vector2(hits[i].transform.position.x, hits[i].transform.position.y)).magnitude;
                if(mouseToHitDist < closestHitDist)
                {
                    closestHitDist = mouseToHitDist;
                    closestHit = hits[i];
                }
            }
        }
        else
        { // mouse out of interact range, find interactable closest to interactor
            closestHitDist = (transform.position - closestHit.transform.position).magnitude;
            for (int i = 1; i < hits.Length; i++)
            {
                float interactorToHitDist = (transform.position - hits[i].transform.position).magnitude;
                if (interactorToHitDist < closestHitDist)
                {
                    closestHitDist = interactorToHitDist;
                    closestHit = hits[i];
                }
            }
        }

        return closestHit;
    }

    public void Interact(bool rightHand)
    {
        if (AvailableInteractable != null)
        {

            // INTERACT WITH ITEM
            if (AvailableInteractable is Item)
            {
                AvailableInteractable.Interact(rightHand);

                if (rightHand)
                {
                    hands.RightObject = ((Component)AvailableInteractable).gameObject; 
                    hands.UsingRight = true;
                }
                else
                {
                    hands.LeftObject = ((Component)AvailableInteractable).gameObject; 
                    hands.UsingLeft = true;
                }
            }


            // INTERACT WITH HAT
            else if (AvailableInteractable is Hat)
            {
                GameObject newHat = ((Component)AvailableInteractable).gameObject;

                if (head.HatObject != null)
                { // Remove current hat
                    head.HatObject.transform.parent = null;
                    head.HatObject.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundedHat";
                    head.HatObject.gameObject.layer = LayerMask.NameToLayer("Interactable");
                    head.HatObject.GetComponent<Hat>().StartTransferPosition(newHat.transform.position, newHat.transform.localRotation);
                    //head.hatObject.transform.position = newHat.transform.position;
                }
                // Wear new hat
                AvailableInteractable.Interact(head);
            }


            // INTERACT WITH LOOT
            else if (AvailableInteractable is Loot)
            {
                AvailableInteractable.Interact(head);
            }

            // OTHER INTERACTABLE, CONTAINER?
            else
            {
                AvailableInteractable.Interact(rightHand);
            }


            AvailableInteractable = null;
            
        }
    }

    #region OLD INTERACTION METHOD (Circle cast and get closest)
    //public void Interact(bool rightHand)
    //{
    //    hit = Physics2D.CircleCast(transform.position, _interactRange, Vector2.zero, 0, InteractionLm);

    //    if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactedObject))
    //    {
    //        if(interactedObject != null)
    //        {
    //            if(interactedObject is Item)
    //            {
    //                interactedObject.Interact(rightHand);
    //                if (rightHand) {
    //                    hands.RightObject = ((Component)interactedObject).gameObject; hands.UsingRight = true;
    //                }
    //                else {
    //                    hands.LeftObject = ((Component)interactedObject).gameObject; hands.UsingLeft = true;
    //                }
    //            }
    //            if(interactedObject is Hat)
    //            {
    //                GameObject newHat = ((Component)interactedObject).gameObject;

    //                if(head.HatObject != null)
    //                { // Remove current hat
    //                    head.HatObject.transform.parent = null;
    //                    head.HatObject.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -20;
    //                    head.HatObject.gameObject.layer = LayerMask.NameToLayer("Interactable");
    //                    head.HatObject.GetComponent<Hat>().StartTransferPosition(newHat.transform.position, newHat.transform.localRotation);
    //                    //head.hatObject.transform.position = newHat.transform.position;
    //                }
    //                // Wear new hat
    //                interactedObject.Interact(head);
    //            }

    //            if(interactedObject is Loot)
    //            {
    //                interactedObject.Interact(head);
    //            }
    //        }
    //    }
    //}
    #endregion
}
