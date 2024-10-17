using GameEvents;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public interface IInteractable
{
    public void Interact(bool rightHand);
    public void Interact(Head head);
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 1.5f;

    [SerializeField] private LayerMask InteractionLm;
    [SerializeField] private LayerMask InteractionBlockersLm;

    [SerializeField] private float scanInterval = .1f;
    private float scanTimer;

    [SerializeField] private float openContainerDistanceCheckInterval = 0.1f;
    private float distanceCheckTimer;

    [SerializeField] Material defaultSpriteMaterial;
    [SerializeField] Material outlineSpriteMaterial;
    [SerializeField] Material lootOutlineSpriteMaterial;

    [SerializeField] VoidEvent[] closeContainerEvents;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private VehicleDriver vehicleDriver;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer playerSprite;

    // when a container is interacted with, it will use this to track the distance, and close the container when you get out of interact range
    [HideInInspector] public static GameObject interactedContainer;
	[HideInInspector] public static GameObject interactedCrafting;
	[HideInInspector] public static Lootable openedLootable;

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
                if (interactableSpriteRenderer != null)
                {
                    interactableSpriteRenderer.material = defaultSpriteMaterial;
                }
            }

            availableInteractable = value;

            if (availableInteractable != null)
            { // new available interactable is not null
                interactableSpriteRenderer = ((Component)availableInteractable).gameObject.GetComponent<SpriteRenderer>();
                //set highlight
                if (availableInteractable is Loot) interactableSpriteRenderer.material = lootOutlineSpriteMaterial;
                else interactableSpriteRenderer.material = outlineSpriteMaterial;
            }
        }
    }

    private void Update()
    {
        //if(!hands.UsingLeft || !hands.UsingRight)
        //{ // at least one hand available for interact, do scan

        if(playerController.currentState != PlayerController.PlayerState.DRIVING)
        {
            if (!playerController.hands.UsingLeft || !playerController.hands.UsingRight)
            {
                if (scanTimer <= 0)
                {
                    scanTimer = scanInterval;

                    IInteractable selectedInteractable = InteractableScan();

                    if(selectedInteractable != null)
                    {
                        AvailableInteractable = selectedInteractable; // will set available to null when no valid interactable found
                    }
                    else if (AvailableInteractable != null) AvailableInteractable = null;
            
                }
                else
                {
                    scanTimer -= Time.deltaTime;
                }
            }
        }


        if ((interactedContainer != null || interactedCrafting != null) && SceneManager.GetActiveScene().name != "Unit")
        {
            if (distanceCheckTimer <= 0)
            {
                distanceCheckTimer = openContainerDistanceCheckInterval;

                if (interactedContainer)
                {
                    float containerToInteractorDist = (transform.position - interactedContainer.transform.position).magnitude;
                    if(containerToInteractorDist > _interactRange + .5f) CloseOpenedContainer();
                }

                if (interactedCrafting)
                {
                    float craftingToInteractorDist = (transform.position - interactedCrafting.transform.position).magnitude;
                    if (craftingToInteractorDist > _interactRange + .5f) CloseOpenedCrafting();

                }
            }
            else
            {
                distanceCheckTimer -= Time.deltaTime;
            }
        }


    }


    public void CloseOpenedContainer()
    {
        if (PlayerController.mouseHeldIcon != null)
        {
            PlayerController.mouseHeldIcon.sendBackToSlot();
            PlayerController.mouseHeldIcon = null;
        }
        string containersCloseEventName = interactedContainer.GetComponent<VoidListener>().GameEvent.name;
        foreach(VoidEvent e in closeContainerEvents)
        {
            if(e.name.Equals(containersCloseEventName))
            {
                e.Raise();
                break;
            }
        }
        interactedContainer = null;
        openedLootable = null;
    }

    private void CloseOpenedCrafting()
    {
        if (PlayerController.mouseHeldIcon != null)
        {
            PlayerController.mouseHeldIcon.sendBackToSlot();
            PlayerController.mouseHeldIcon = null;
        }

        string craftingCloseEventName = interactedCrafting.GetComponent<VoidListener>().GameEvent.name;
        foreach (VoidEvent e in closeContainerEvents)
        {
            if (e.name.Equals(craftingCloseEventName))
            {
                e.Raise();
                break;
            }
        }
        interactedCrafting = null;
    }

    /// <summary>
    /// Finds interactables around interactor. 
    /// <para>First checks if mouse is within interact range, if so it will find the interactable closest to the mouse.</para>
    /// <para>Otherwise it will find the interactable closest to the interactor.</para>
    /// </summary>
    /// <returns>Closest IInteractable, if there are no valid Interactables, will return null</returns>
    public IInteractable InteractableScan()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _interactRange, InteractionLm);

        if (colliders.Length == 0) return null;

        List<Collider2D> validColliders = new List<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            Vector2 direction = collider.transform.position - transform.position;
            float dist = direction.magnitude;
            Debug.DrawLine(transform.position, transform.position + (Vector3)direction.normalized * dist, Color.red);  // Ray from the center
            RaycastHit2D hit = Physics2D.Raycast(transform.position, collider.transform.position - transform.position, dist, InteractionBlockersLm);
            if(!hit.collider) validColliders.Add(collider);

        }

        if (validColliders.Count == 0) return null;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseToInteractorDist = (mousePosition - new Vector2(transform.position.x, transform.position.y)).magnitude;

        Collider2D closestCollider;
        IInteractable closestInteractable = null;
        float closestColliderDist = float.PositiveInfinity;
        
        Lootable closestLootable = null;
        IInteractable closestLootableInteractable = null;
        float clostestLootableDist = float.PositiveInfinity;
        
        // loop through hits
        foreach (Collider2D collider in validColliders)
        {
            IInteractable thisInteractable = collider.transform.gameObject.GetComponent<IInteractable>();

            if (thisInteractable is Obstacle obstacle)
            {
                if(playerController.hands.leftObstacle && playerController.hands.leftObstacle == obstacle){}
                else if (playerController.hands.rightObstacle && playerController.hands.rightObstacle == obstacle){}
                else if (((Vector2)transform.position - collider.ClosestPoint(transform.position)).magnitude > obstacle.grabRange)
                {
                    continue;
                }
            }

            float dist;
            if (mouseToInteractorDist <= _interactRange + 0.5f) dist = (mousePosition - collider.ClosestPoint(mousePosition)).magnitude;
            else                                         dist = ((Vector2)transform.position - collider.ClosestPoint(transform.position)).magnitude;

            if (thisInteractable is Lootable lootable)
            {
                if (dist < clostestLootableDist)
                {
                    clostestLootableDist = dist;
                    closestLootableInteractable = thisInteractable;
                    closestLootable = lootable;
                }
            }
            else if (dist < closestColliderDist)
            {
                closestColliderDist = dist;
                closestInteractable = thisInteractable;
                closestCollider = collider;
            }


        }

        if(closestLootable != null && closestLootableInteractable is Lootable lootableContainer)
        {
            if(openedLootable == null || (openedLootable != null && lootableContainer != openedLootable) && !PlayerController.holdingLeft && !PlayerController.holdingSneak)
            {
                // Manually auto interacts with lootable, but leaves availableInteractable how it was before
                IInteractable lastAvailable = availableInteractable;
                availableInteractable = closestLootableInteractable;
                Interact(false);
                availableInteractable = lastAvailable;
                openedLootable = lootableContainer;
            }
        }

        return closestInteractable;
    }

    public void Interact(bool rightHand)
    {
        if (AvailableInteractable != null)
        {
            if (AvailableInteractable is Locker || AvailableInteractable is Lootable)
            {
                if (interactedContainer != null)
                {
                    Lootable l;
                    if(!interactedContainer.TryGetComponent(out l)) // dont close other container if it is lootable, lootable slider handles this
                    {
                        CloseOpenedContainer();
                    }
                }

                if (AvailableInteractable is MonoBehaviour mono) interactedContainer = mono.gameObject;
                AvailableInteractable.Interact(rightHand);
            }

            else if (AvailableInteractable is Workbench)
            {
                if (interactedCrafting != null)
                {
                    CloseOpenedCrafting();
                }

                if (AvailableInteractable is MonoBehaviour mono) interactedCrafting = mono.gameObject;
                AvailableInteractable.Interact(rightHand);
            }

            // INTERACT WITH ITEM
            else if (AvailableInteractable is Item)
            {
                AvailableInteractable.Interact(rightHand);

                if (rightHand)
                {
                    playerController.hands.RightObject = ((Component)AvailableInteractable).gameObject; 
                    playerController.hands.UsingRight = true;
                }
                else
                {
                    playerController.hands.LeftObject = ((Component)AvailableInteractable).gameObject; 
                    playerController.hands.UsingLeft = true;
                }
            }


            // INTERACT WITH HAT
            else if (AvailableInteractable is Hat)
            {
                GameObject newHat = ((Component)AvailableInteractable).gameObject;

                if (playerController.head.HatObject != null)
                { // Remove current hat
                    Hat h = playerController.head.wornHat;
                    playerController.head.hairRenderer.enabled = true;
                    playerController.head.HatObject.transform.parent = null;
                    playerController.head.HatObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundedHat";
                    playerController.head.HatObject.layer = LayerMask.NameToLayer("Interactable");
                    if (playerController.head.HatObject.transform.childCount > 0)
                    {
                        SpriteRenderer[] childRenderers = playerController.head.HatObject.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer childRenderer in childRenderers) childRenderer.sortingLayerName = "GroundedHat";
                    }
                    h.StartCoroutine(h.TransferPosition(newHat.transform.position, newHat.transform.rotation));
                    h.head = null;

                    //h.DropHat(newHat.transform.position); makes duplicate on head for no reason

                }

                // Wear new hat
                AvailableInteractable.Interact(playerController.head);
            }


            // INTERACT WITH LOOT
            else if (AvailableInteractable is Loot)
            {
                AvailableInteractable.Interact(playerController.head);
            }

            // INTERACT WITH VEHICLE
            else if (AvailableInteractable is Vehicle v)
            {
                vehicleDriver.vehicle = v;
                vehicleDriver.Enter(playerCollider, playerController);
                AvailableInteractable.Interact(playerController.head);
            }

            else if (AvailableInteractable is Obstacle o)
            {
                AvailableInteractable.Interact(rightHand);
                if (rightHand)
                {
                    playerController.hands.UsingRight = true;
                    playerController.hands.rightObstacle = o;
                }
                else
                {
                    playerController.hands.UsingLeft = true;
					playerController.hands.leftObstacle = o;
				}
                o.joint.connectedBody = playerController.rb;
                if(playerController.hands.leftObstacle && playerController.hands.rightObstacle && playerController.hands.leftObstacle == playerController.hands.rightObstacle)
                {
                    o.OnTwoHandsOn();
                }
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
