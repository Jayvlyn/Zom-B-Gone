using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour, IInteractable
{
    protected enum State
    {
        GROUNDED, AIRBORNE, HELD
    }
    protected State currentState;


	[Header("Item attributes")]
    [SerializeField] public ItemData itemData;
    // Component Refs
    [SerializeField] protected Transform pivotPoint;
    [SerializeField] protected Collider2D fullCollider;
    [SerializeField] protected SpriteRenderer itemRenderer;
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
    // Sort Ordering
    [SerializeField] protected int groundSortOrder = -30;
    [SerializeField] protected int heldSortOrder = 5;
    [SerializeField] protected int airborneSortOrder = 20;

    // Private Refs
    protected Rigidbody2D rb;
    protected PlayerController playerController;
    protected Hands playerHands;
    protected Head playerHead;
    protected PlayerData playerData;

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
            case State.GROUNDED: itemRenderer.sortingOrder = groundSortOrder; break;
            case State.AIRBORNE: itemRenderer.sortingOrder = airborneSortOrder; break;
            case State.HELD: itemRenderer.sortingOrder = heldSortOrder; break;
        }
    }

    protected virtual void Update()
    {
        if(currentState == State.AIRBORNE && rb.velocity.magnitude < minimumAirborneSpeed)
        {
            ChangeState(State.GROUNDED);
        }

        if(moveToHand)
        {
            ReturnToGrip();
		}
    }

    private void FixedUpdate()
    {
        if(currentState == State.HELD && aimAtMouse)
        {
            RotateToMouse();
        }
    }

    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.GROUNDED:
                // Change to grounded drag
                rb.drag = groundedLinearDrag;
                rb.angularDrag = groundedAngularDrag;

                // Change layer
                gameObject.layer = LayerMask.NameToLayer("InteractableItem");

                // Change sorting order
                itemRenderer.sortingOrder = groundSortOrder;

                // If was held, set back to dynamic body
                if (currentState == State.HELD) rb.bodyType = RigidbodyType2D.Dynamic;

				// Reactivate full body collider
				fullCollider.enabled = true;

				// Active objects should pass over it
				// fullCollider.isTrigger = true;

				break;
            case State.AIRBORNE:
                // Change to airborne drag
                rb.drag = airborneLinearDrag;
                rb.angularDrag = airborneAngularDrag;

                // Change layer
                gameObject.layer = LayerMask.NameToLayer("AirborneItem");

                // Change soring order
                itemRenderer.sortingOrder = airborneSortOrder;

                // If was held, set back to dynamic body
                if (currentState == State.HELD) rb.bodyType = RigidbodyType2D.Dynamic;

				// Reactivate full body collider
				fullCollider.enabled = true;

				// Yes full collider
				fullCollider.isTrigger = false;

                break;

            case State.HELD:
				// Change layer
				gameObject.layer = LayerMask.NameToLayer("AirborneItem");

				// Change sorting order
				itemRenderer.sortingOrder = heldSortOrder;

                // Set to kinematic body for manual movement
                rb.bodyType = RigidbodyType2D.Kinematic;

                // No full collider
                fullCollider.enabled = false;
                fullCollider.isTrigger = true;
                break;

            default:
                break;

        }

        currentState = newState;
    }

    public abstract void Use();

    public virtual void Drop()
    {
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(State.AIRBORNE);
        float dropForwardForce = Utils.MapWeightToRange(itemData.weight, 1.5f, 3, true);
        rb.AddForce(playerT.up * dropForwardForce, ForceMode2D.Impulse);
    }

    public virtual void Throw()
    {
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(State.AIRBORNE);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;

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

        ChangeState(State.HELD);
        
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
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(currentState == State.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            Debug.Log(rb.velocity.magnitude);
            collisionHealth.TakeDamage(Utils.MapWeightToRange(itemData.weight, 5, 100, false) * rb.velocity.magnitude);
            if (collisionHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb))
            {
                hitRb.AddForce(rb.velocity.normalized * 0.5f * (Utils.MapWeightToRange(itemData.weight, 10, 70, true)), ForceMode2D.Impulse);
            }
        }
    }


    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}
