using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour, IInteractable
{
    public enum ItemState
    {
        GROUNDED, AIRBORNE, HELD
    }
    public ItemState currentState;


	[Header("Item attributes")]
    [SerializeField] public ItemData itemData;
    // Component Refs
    [SerializeField] protected Transform pivotPoint;
    [SerializeField] protected Collider2D fullCollider;
    protected SpriteRenderer itemRenderer;
    // Drag
    [SerializeField, Range(0.0f, 10.0f)] protected float airborneAngularDrag = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] protected float airborneLinearDrag = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] protected float groundedAngularDrag = 3.0f;
    [SerializeField, Range(0.0f, 10.0f)] protected float groundedLinearDrag = 3.0f;
    // Customization
	[SerializeField] Vector2 holdOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] protected bool spinThrow = true;
    [SerializeField] protected bool aimAtMouse = true;
    [SerializeField] protected float gripRotation = 130;
    [SerializeField] protected float minimumAirborneSpeed = 10;

    protected Rigidbody2D rb;
    protected PlayerController playerController;
    protected Hands playerHands;
    protected Head playerHead;
    protected PlayerData playerData;
    [HideInInspector] public VanBack vanBack;

    // Functional vars
    [HideInInspector] public bool inRightHand;
    [HideInInspector] public bool useHeld;
    protected bool moveToHand;
    private Vector3 pickupTarget;
    private Quaternion rotationTarget;
    private float pickupSpeed = 10;


    public void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerHands = playerController.GetComponentInParent<Hands>();
        playerHead = playerController.GetComponentInParent<Head>();
        playerData = playerController.playerData;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (fullCollider == null) fullCollider = GetComponent<Collider2D>();
        if (itemRenderer == null) itemRenderer = GetComponent<SpriteRenderer>();

        // Set sorting order based on awake state
        switch (currentState)
        {
            case ItemState.GROUNDED:
                itemRenderer.sortingLayerName = "GroundedItem";
                rb.drag = groundedLinearDrag;
                rb.angularDrag = groundedAngularDrag;
                break;
            case ItemState.AIRBORNE: 
                itemRenderer.sortingLayerName = "ActiveItem";
                rb.drag = airborneLinearDrag;
                rb.angularDrag = airborneAngularDrag;
                break;
            case ItemState.HELD:
                itemRenderer.sortingLayerName = "ActiveItem";
                break;
        }
    }

    protected virtual void Update()
    {
        if(currentState == ItemState.AIRBORNE && rb.velocity.magnitude < minimumAirborneSpeed)
        {
            ChangeState(ItemState.GROUNDED);
        }

        if(moveToHand)
        {
            ReturnToGrip();
		}
    }

    private void FixedUpdate()
    {
        if(currentState == ItemState.HELD && aimAtMouse)
        {
            RotateToMouse();
        }
    }

    private void ChangeState(ItemState newState)
    {
        switch (newState)
        {
            case ItemState.GROUNDED:
                rb.drag = groundedLinearDrag;
                rb.angularDrag = groundedAngularDrag;

                gameObject.layer = LayerMask.NameToLayer("InteractableItem");

                itemRenderer.sortingLayerName = "GroundedItem";

                if (currentState == ItemState.HELD) rb.bodyType = RigidbodyType2D.Dynamic;

				fullCollider.enabled = true;

				// fullCollider.isTrigger = true;

				break;
            case ItemState.AIRBORNE:
                rb.drag = airborneLinearDrag;
                rb.angularDrag = airborneAngularDrag;

                gameObject.layer = LayerMask.NameToLayer("AirborneItem");

                itemRenderer.sortingLayerName = "ActiveItem";

                if (currentState == ItemState.HELD) rb.bodyType = RigidbodyType2D.Dynamic;

				fullCollider.enabled = true;

				fullCollider.isTrigger = false;

                break;

            case ItemState.HELD:
				gameObject.layer = LayerMask.NameToLayer("AirborneItem");

                itemRenderer.sortingLayerName = "ActiveItem";

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
        if(invertDrop) { dropForce = -dropForce; }
        rb.AddForce(-transform.right * dropForce, ForceMode2D.Impulse);
    }

    public virtual void Drop()
    {
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
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(ItemState.AIRBORNE);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        //if (Utils.WallInFront(playerT)) direction = -direction;

        float throwForce = Utils.MapWeightToRange(itemData.weight, 10, 20, true);

        // Velocity change instead because throw is fastest the second it the object leaves contact with the propelling force
        rb.velocity = direction * throwForce;
		//rb.AddForce(direction * throwForce, ForceMode2D.Impulse);

        if(spinThrow)
        {
            float spinForce = Utils.MapWeightToRange(itemData.weight, 100, 700, true);
            if (!inRightHand) spinForce *= -1;
            rb.angularVelocity = spinForce;
        }
    }

    public virtual void PickUp(Transform parent, bool rightHand)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        ChangeState(ItemState.HELD);
        
        if (rightHand) inRightHand = true;
        else inRightHand = false;
        CheckFlip();
        
        
        transform.SetParent(parent);
        PositionInHand();
    }

    protected void CheckFlip()
    {
        if((inRightHand && transform.localScale.x < 0) || (!inRightHand && transform.localScale.x > 0))
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

    public void Interact(bool rightHand)
    {
        PickUp(playerController.transform, rightHand);
    }

    public void PositionInHand()
    {
        if (inRightHand) rotationTarget = Quaternion.Euler(0, 0, -gripRotation);
        else rotationTarget = Quaternion.Euler(0, 0, gripRotation);

        CheckFlip();
		
        moveToHand = true; // see update()
    }

    protected void ReturnToGrip()
    {
		if (inRightHand)
		{
			pickupTarget = transform.parent.position + (transform.parent.right * holdOffset.x + transform.parent.up * holdOffset.y);
		}
		else
		{
			pickupTarget = transform.parent.position + (-transform.parent.right * holdOffset.x + transform.parent.up * holdOffset.y);
		}
		if (Vector3.Distance(transform.position, pickupTarget) < 0.02f)
		{
			transform.position = pickupTarget;
			moveToHand = false;
			return;
		}
		transform.position = Vector2.Lerp(transform.position, pickupTarget, Time.deltaTime * pickupSpeed);
		if (!aimAtMouse) transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, pickupSpeed * Time.deltaTime);
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

        if(vanBack)
        {
            if(fullCollider.IsTouching(vanBack.backCollider))
            {
                vanBack.AddToBack(this);
            }
        }

    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(currentState == ItemState.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            int damage = Mathf.RoundToInt(Utils.MapWeightToRange(itemData.weight, 5, 100, false) * (rb.velocity.magnitude / 2));


            Vector3 popupVector = (collisionHealth.transform.position - playerHead.transform.position).normalized * 20f;
            bool invertRotate = popupVector.x < 0; // invert when enemy is on left of player

            collisionHealth.TakeDamage(damage, 0, 1, false, popupVector, invertRotate);

            // do knockback if there is rigidbody
            if (collisionHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb))
            {
                hitRb.AddForce(rb.velocity.normalized * 0.5f * (Utils.MapWeightToRange(itemData.weight, 10, 70, true)), ForceMode2D.Impulse);
            }
        }
    }

    public void AddToVan()
    {
        fullCollider.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }


    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}
