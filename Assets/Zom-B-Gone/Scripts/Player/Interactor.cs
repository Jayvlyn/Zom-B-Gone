using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IInteractable
{
    public void Interact(bool rightHand, PlayerController playerController);
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

    [SerializeField] private float swapLootableCooldown = 0.2f;
    private bool canSwapLootable = true;

    [SerializeField] Material defaultSpriteMaterial;
    [SerializeField] Material outlineSpriteMaterial;
    [SerializeField] Material lootOutlineSpriteMaterial;

    [SerializeField] VoidEvent[] closeContainerEvents;
    [SerializeField] VoidEvent closeLootableEvent;
    [SerializeField] VoidEvent closeCraftingEvent;

    [SerializeField] private PlayerController playerController;

    // when a container is interacted with, it will use this to track the distance, and close the container when you get out of interact range
    [HideInInspector] public static GameObject interactedContainer;
	[HideInInspector] public static GameObject interactedCrafting;
	[HideInInspector] public static Lootable openedLootable;
    private bool lootableMenuOpen = false;

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
        if(PlayerController.currentState != PlayerController.PlayerState.DRIVING && PlayerController.currentState != PlayerController.PlayerState.HIDING)
        {
            // scan must happen even with hands full for lootable scanning

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


        if ((interactedContainer != null || interactedCrafting != null || openedLootable != null) && SceneManager.GetActiveScene().name != "Unit")
        {
            // Distance Check
            if (distanceCheckTimer <= 0)
            {
                distanceCheckTimer = openContainerDistanceCheckInterval;

                if (interactedContainer)
                {
                    float containerToInteractorDist = ((Vector2)transform.position - (Vector2)interactedContainer.transform.position).magnitude;
                    if (containerToInteractorDist > _interactRange + .5f) { Debug.Log("Close open 2"); CloseOpenedContainer(); }
                    
                }

                if (interactedCrafting)
                {
                    float craftingToInteractorDist = ((Vector2)transform.position - (Vector2)interactedCrafting.transform.position).magnitude;
                    if (craftingToInteractorDist > _interactRange + .5f) CloseOpenedCrafting();
                }

                if (openedLootable)
                {
                    Debug.Log("Debug 1: there is opened lootable");
                    float lootableToInteractorDist = ((Vector2)transform.position - (Vector2)openedLootable.transform.position).magnitude;
                    if (lootableToInteractorDist > _interactRange + .5f) {
                        CloseOpenedLootable();
                        Debug.Log("Debug 2: far away, Close opened lootable");
                    }
                }
            }
            else
            {
                distanceCheckTimer -= Time.deltaTime;
            }
        }
        else if (openedLootable == null && lootableMenuOpen)
        {
            CloseOpenedLootable();
        }


    }


    public void CloseOpenedContainer()
    {
        if (PlayerController.mouseHeldIcon != null)
        {
            PlayerController.mouseHeldIcon.sendBackToSlot();
            PlayerController.mouseHeldIcon = null;
        }

        GameEvents.TransformListener tl = interactedContainer.GetComponent<GameEvents.TransformListener>();
		string containersCloseEventName = tl.GameEvent.name; 
        
        foreach(VoidEvent e in closeContainerEvents)
        {
            if(e.name.Equals(containersCloseEventName))
            {
                e.Raise();
                break;
            }
        }
        interactedContainer = null;
    }

    public void CloseOpenedLootable()
    {
        lootableMenuOpen = false;

        if (PlayerController.mouseHeldIcon != null)
        {
            PlayerController.mouseHeldIcon.sendBackToSlot();
            PlayerController.mouseHeldIcon = null;
        }

        closeLootableEvent.Raise();

        openedLootable = null;
    }


    private void CloseOpenedCrafting()
    {
        if (PlayerController.mouseHeldIcon != null)
        {
            PlayerController.mouseHeldIcon.sendBackToSlot();
            PlayerController.mouseHeldIcon = null;
        }

        closeCraftingEvent.Raise();

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
            //Debug.Log(collider.gameObject.name);
            Vector2 direction = collider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float dist = direction.magnitude;
            Debug.DrawLine(transform.position, transform.position + (Vector3)direction.normalized * dist, Color.red);  // Ray from the center
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dist, InteractionBlockersLm);
            if (!hit.collider)
            {
                validColliders.Add(collider);
            }
        }
        if (validColliders.Count == 0) return null;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseToInteractorDist = (mousePosition - new Vector2(transform.position.x, transform.position.y)).magnitude;

        Collider2D closestCollider;
        IInteractable closestInteractable = null;
        float closestColliderDist = float.PositiveInfinity;
        
        Lootable closestLootable = null;
        float clostestLootableDist = float.PositiveInfinity;

        // loop through hits
        foreach (Collider2D collider in validColliders)
        {
            IInteractable thisInteractable = collider.transform.gameObject.GetComponent<IInteractable>();

            float dist;
            if (mouseToInteractorDist <= _interactRange + 0.5f) dist = (mousePosition - collider.ClosestPoint(mousePosition)).magnitude;
            else                                         dist = ((Vector2)transform.position - collider.ClosestPoint(transform.position)).magnitude;

            if (thisInteractable is Obstacle obstacle)
            {
                if (obstacle.lootable != null)
                {
                    if (dist < clostestLootableDist)
                    {
                        clostestLootableDist = dist;
                        closestLootable = obstacle.lootable;
                    }
                }

                if (playerController.hands.LeftObstacle && playerController.hands.LeftObstacle == obstacle){}
                else if (playerController.hands.RightObstacle && playerController.hands.RightObstacle == obstacle){}
                else if (((Vector2)transform.position - collider.ClosestPoint(transform.position)).magnitude > obstacle.grabRange)
                {
                    continue;
                }
            }

            if (dist < closestColliderDist)
            {
                if (!playerController.hands.UsingLeft || !playerController.hands.UsingRight)
                {
                    closestColliderDist = dist;
                    closestInteractable = thisInteractable;
                    closestCollider = collider;
                }
            }
        }


        if(closestLootable != null)
        {
            if ((openedLootable == null || (openedLootable != closestLootable && !PlayerController.holdingSneak && !PlayerController.holdingLeft)) && canSwapLootable)
            {
                if (openedLootable)
                {
                    CloseOpenedLootable();
                }
                openedLootable = closestLootable;

                if (openLootableRoutine != null) StopCoroutine(openLootableRoutine); 
                openLootableRoutine = StartCoroutine(OpenNewLootable());

                if (swapCooldownRoutine != null) StopCoroutine(swapCooldownRoutine);
                swapCooldownRoutine = StartCoroutine(SwapLootableCooldownTimer());
            }
        }

        return closestInteractable;
    }

    private Coroutine swapCooldownRoutine;
    private IEnumerator SwapLootableCooldownTimer()
    {
        canSwapLootable = false;
        yield return new WaitForSeconds(swapLootableCooldown);
        canSwapLootable = true;
        swapCooldownRoutine = null;
    }

    private Coroutine openLootableRoutine;
    private IEnumerator OpenNewLootable()
    {
        yield return new WaitForSeconds(0.05f);
        if (openedLootable)
        {
            openedLootable.OpenLootable();
            lootableMenuOpen = true;
        }
    }

    public void Interact(bool rightHand)
    {
        if (AvailableInteractable != null)
        {
            if (AvailableInteractable is Locker)
            {
                if (interactedContainer != null)
                {
                    CloseOpenedContainer();
                }

                AvailableInteractable.Interact(rightHand, playerController);
                if (AvailableInteractable is MonoBehaviour mono) interactedContainer = mono.gameObject;
            }

            else if (AvailableInteractable is Workbench)
            {
                if (interactedCrafting != null)
                {
                    CloseOpenedCrafting();
                }

                if (AvailableInteractable is MonoBehaviour mono) interactedCrafting = mono.gameObject;
                AvailableInteractable.Interact(rightHand, playerController);
            }

            // INTERACT WITH ITEM
            else if (AvailableInteractable is Item)
            {
                Item thisItem = (Item)AvailableInteractable;

                // use right hand to add dupe items to left hand
                if(rightHand && playerController.hands.leftItem != null && playerController.hands.leftItem.itemData == thisItem.itemData && playerController.hands.leftItem.Quantity + thisItem.Quantity <= thisItem.itemData.MaxStack)
                {
                    thisItem.AddToHand(false, playerController); // add to left hand
                }
				// use left hand to add dupe items to right hand
				else if (!rightHand && playerController.hands.rightItem != null && playerController.hands.rightItem.itemData == thisItem.itemData && playerController.hands.rightItem.Quantity + thisItem.Quantity <= thisItem.itemData.MaxStack)
				{
					thisItem.AddToHand(true, playerController); // add to right hand
				}
                else
                {
				    AvailableInteractable.Interact(rightHand, playerController);

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
                    playerController.head.HatObject.layer = LayerMask.NameToLayer("Interactable");
                    h.ChangeSortingLayer(h.wornSortingLayerID);
                    h.StartCoroutine(h.TransferPosition(newHat.transform.position, newHat.transform.rotation));
                    h.head = null;

                    //h.DropHat(newHat.transform.position); makes duplicate on head for no reason

                }

                // Wear new hat
                AvailableInteractable.Interact(false, playerController);
            }


            // INTERACT WITH LOOT
            else if (AvailableInteractable is Loot)
            {
                AvailableInteractable.Interact(false, playerController);
            }

            // INTERACT WITH OBSTACLE
            else if (AvailableInteractable is Obstacle o)
            {
                AvailableInteractable.Interact(rightHand, playerController);
                if (rightHand)
                {
                    playerController.hands.UsingRight = true;
                    playerController.hands.RightObstacle = o;
                }
                else
                {
                    playerController.hands.UsingLeft = true;
                    playerController.hands.LeftObstacle = o;
                }
                o.joint.connectedBody = playerController.rb;
                if (playerController.hands.LeftObstacle && playerController.hands.RightObstacle && playerController.hands.LeftObstacle == playerController.hands.RightObstacle)
                {
                    o.OnTwoHandsOn();
                }
            }

            else if (AvailableInteractable is HidingSpot h)
            {
                playerController.currentHidingSpot = h;
                playerController.StartCoroutine(playerController.EnterHidingSpot(h.hidingT.position, h.hidingT.rotation));
                AvailableInteractable.Interact(rightHand, playerController);
            }

            // INTERACT WITH VEHICLE
            else if (AvailableInteractable is Vehicle v)
            {
                playerController.vehicleDriver.vehicle = v;
                playerController.vehicleDriver.Enter(playerController.playerCollider, playerController);
                AvailableInteractable.Interact(false, playerController);
            }

            else
            {
                AvailableInteractable.Interact(rightHand, playerController);
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
