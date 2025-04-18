using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : Collectible
{
    public enum ItemState
    {
        GROUNDED, AIRBORNE, HELD
    }
    public ItemState currentState;

    [HideInInspector] public ItemData itemData;

    [Header("Item attributes")]
    // Component Refs
    [SerializeField] protected Transform pivotPoint;
    [HideInInspector] public SpriteRenderer itemRenderer;
    public SpriteRenderer[] childrenRenderers;
    public Sticky optionalSticky;
    // Drag
    [SerializeField, Range(0.0f, 10.0f)] protected float airborneAngularDrag = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] protected float airborneLinearDrag = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] protected float groundedAngularDrag = 3.0f;
    [SerializeField, Range(0.0f, 10.0f)] protected float groundedLinearDrag = 3.0f;
    // Customization
    [SerializeField] protected Vector2 holdOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] protected bool spinThrow = true;
    [SerializeField] protected bool aimAtMouse = true;
    [SerializeField] protected float gripRotation = 130;
    [SerializeField] protected float minimumAirborneSpeed = 10;

    public Rigidbody2D rb;

    [HideInInspector] public VanFloor vanBack;
    [HideInInspector] public UnitFloor unitFloor;

    protected LayerMask useBlockersLm;

	// Functional vars
	[HideInInspector] public bool inRightHand;
    [HideInInspector] public bool useHeld;
    protected bool moveToHand;
    private Vector3 pickupTarget;
    private Quaternion rotationTarget;
    private float pickupSpeed = 10;
    [SerializeField] private int quantity = 1;
    public int Quantity
    {
        get { return quantity; }
        set
        {
            quantity = value;
            if (currentState == ItemState.HELD)
            {
                if (inRightHand) playerHands.handContainerData.Container.collectibleSlots[1].quantity = quantity;
                else playerHands.handContainerData.Container.collectibleSlots[0].quantity = quantity;

                playerHands.handContainerData.onContainerCollectibleUpdated.Raise();
            }
        }
    }


    public void Awake()
    {
		useBlockersLm = LayerMask.GetMask("World");
		itemData = data as ItemData;
        if (rb == null )          rb = GetComponent<Rigidbody2D>();
        if (fullCollider == null) fullCollider = GetComponent<Collider2D>();
        if (itemRenderer == null) itemRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;

        // Set sorting order based on awake state
        switch (currentState)
        {
            case ItemState.GROUNDED:
                ChangeSorting("GroundedItem");
                rb.linearDamping = groundedLinearDrag;
                rb.angularDamping = groundedAngularDrag;
                break;
            case ItemState.AIRBORNE:
                ChangeSorting("ActiveItem");
                rb.linearDamping = airborneLinearDrag;
                rb.angularDamping = airborneAngularDrag;
                break;
            case ItemState.HELD:
                ChangeSorting("ActiveItem");
                break;
        }
    }

    private void ChangeSorting(string sortingLayerName)
    {
        itemRenderer.sortingLayerName = sortingLayerName;
        foreach (SpriteRenderer sr in childrenRenderers)
        {
            sr.sortingLayerName = sortingLayerName;
        }
    }


    protected virtual void Update()
    {
        if (currentState == ItemState.AIRBORNE && rb.linearVelocity.magnitude < minimumAirborneSpeed)
        {
            ChangeState(ItemState.GROUNDED);
        }
    }

    private void FixedUpdate()
    {
        if (currentState == ItemState.HELD && aimAtMouse)
        {
            RotateToMouse();
        }
    }

    public void ChangeState(ItemState newState)
    {
        switch (newState)
        {
            case ItemState.GROUNDED:
                rb.linearDamping = groundedLinearDrag;
                rb.angularDamping = groundedAngularDrag;

                gameObject.layer = LayerMask.NameToLayer("InteractableItem");

                ChangeSorting("GroundedItem");

                if (currentState == ItemState.HELD) rb.bodyType = RigidbodyType2D.Dynamic;

                fullCollider.enabled = true;

                // fullCollider.isTrigger = true;

                break;
            case ItemState.AIRBORNE:
                rb.linearDamping = airborneLinearDrag;
                rb.angularDamping = airborneAngularDrag;

                gameObject.layer = LayerMask.NameToLayer("AirborneItem");

                ChangeSorting("ActiveItem");

                if (currentState == ItemState.HELD) rb.bodyType = RigidbodyType2D.Dynamic;

                fullCollider.enabled = true;

                fullCollider.isTrigger = false;

                break;

            case ItemState.HELD:
                if (optionalSticky) optionalSticky.stick = true;
                gameObject.layer = LayerMask.NameToLayer("AirborneItem");

                ChangeSorting("ActiveItem");

                rb.bodyType = RigidbodyType2D.Kinematic;

                fullCollider.enabled = false;
                fullCollider.isTrigger = true;
                break;

            default:
                break;

        }

        currentState = newState;
    }

    public abstract void Use();

    public virtual void InventoryDrop(bool invertDrop)
    {
        float dropForce = Utils.MapWeightToRange(itemData.weight, 1.5f, 3, true);
        if (invertDrop) { dropForce = -dropForce; }
        rb.AddForce(-transform.right * dropForce, ForceMode2D.Impulse);
    }

    public virtual void Drop()
    {
        PlayDropSound();
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(ItemState.AIRBORNE);
        float dropForwardForce = Utils.MapWeightToRange(itemData.weight, 1.5f, 3, true);
        Vector2 direction = playerT.up;
        //if (Utils.WallInFront(playerT)) direction = -direction;
        rb.AddForce(direction * dropForwardForce, ForceMode2D.Impulse);
    }

    
    public virtual void Throw()
    {
        PlayThrowSound();
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(ItemState.AIRBORNE);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        //if (Utils.WallInFront(playerT)) direction = -direction;

        float throwForce = Utils.MapWeightToRange(itemData.weight, 10, 20, true) * itemData.throwForceMult;

        // Velocity change instead because throw is fastest the second it the object leaves contact with the propelling force
        rb.linearVelocity = direction * throwForce;
        //rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
        if (spinThrow)
        {
            float spinForce = Utils.MapWeightToRange(itemData.weight, 60, 500, true);
            if (!inRightHand) spinForce *= -1;
            rb.angularVelocity = spinForce;
        }
    }

    public virtual void PickUp(Transform parent, bool rightHand, bool adding = false)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        ChangeState(ItemState.HELD);

        if (rightHand) inRightHand = true;
        else inRightHand = false;
        CheckFlip();


        transform.SetParent(parent);
        PositionInHand(adding);
    }

    protected void CheckFlip()
    {
        if ((inRightHand && transform.localScale.x < 0) || (!inRightHand && transform.localScale.x > 0))
        {
            FlipX();
        }
    }

    protected void FlipX()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void RotateToMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public override void Interact(bool rightHand, PlayerController playerController)
    {
        base.Interact(rightHand, playerController);
        PickUp(playerController.transform, rightHand);
    }

    public void AddToHand(bool rightHand, PlayerController playerController)
    {
        base.Interact(rightHand, playerController);
        PickUp(playerController.transform, rightHand, true);
    }

    public void PositionInHand(bool destroy = false)
    {
        if (inRightHand) rotationTarget = Quaternion.Euler(0, 0, -gripRotation);
        else rotationTarget = Quaternion.Euler(0, 0, gripRotation);

        CheckFlip();

        StartCoroutine(ReturnToGrip(destroy));
    }

    protected IEnumerator ReturnToGrip(bool destroy = false)
    {

        moveToHand = true;
		while (moveToHand)
        {
		    if (inRightHand)
		    {
			    pickupTarget = transform.parent.position + (transform.parent.right * holdOffset.x + transform.parent.up * holdOffset.y);
		    }
		    else
		    {
			    pickupTarget = transform.parent.position + (-transform.parent.right * holdOffset.x + transform.parent.up * holdOffset.y);
		    }
            transform.position = Vector2.Lerp(transform.position, pickupTarget, Time.deltaTime * pickupSpeed);
            if (!aimAtMouse) transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, pickupSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, pickupTarget) < 0.02f)
            {
                transform.position = pickupTarget;
                moveToHand = false;
            }
            yield return null;
        }

        if(destroy)
        {
            if(inRightHand)
            {
                playerHands.rightItem.Quantity += Quantity;
            }
            else
            {
                playerHands.leftItem.Quantity += Quantity;
            }
            playerHands.handContainerData.UpdateUI();
            Destroy(gameObject);
        }
    }

    protected virtual void RemoveFromHand()
    {
        fullCollider.enabled = true;
        useHeld = false;
        moveToHand = false;

        if (inRightHand)
        {
            playerHands.RightObject = null;
            playerHands.UsingRight = false;
        }
        else
        {
            playerHands.LeftObject = null;
            playerHands.UsingLeft = false;
        }

        transform.SetParent(null);

        if (vanBack)
        {
            StartCoroutine(AddToFloorContainer(vanBack));
        }
        else if (unitFloor)
        {
            StartCoroutine(AddToFloorContainer(unitFloor));
        }

    }
    

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == ItemState.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            int damage = Mathf.RoundToInt(Utils.MapWeightToRange(itemData.weight, 3, 30, false) * (rb.linearVelocity.magnitude / 2));


            Vector3 popupVector = (collisionHealth.transform.position - playerHead.transform.position).normalized * 20f;
            bool invertRotate = popupVector.x < 0; // invert when enemy is on left of player

            Vector2 knockbackVector = rb.linearVelocity.normalized * 0.5f * (Utils.MapWeightToRange(itemData.weight, 3, 30, true));
            collisionHealth.TakeDamage(damage, knockbackVector, 0, false, popupVector, invertRotate);
        }
    }

    public void AddToFloor()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0;
        fullCollider.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public override IEnumerator AddToFloorContainer(Floor floor)
    {
        yield return new WaitForSeconds(2.2f);
		if (fullCollider.bounds.Intersects(floor.floorCollider.bounds))
        {
		    AddToFloor();
            fullCollider.gameObject.transform.parent = floor.floorCollider.transform;
            floor.floorContainer.AddCollectibleToContainer(this);

        }
    }

    public void PlayThrowSound()
    {
        if(itemData.throwSound)
        {
            audioSource.PlayOneShot(itemData.throwSound);
        }
        Utils.MakeSoundWave(playerController.transform.position, 1.2f, PlayerController.isSneaking);
    }

}