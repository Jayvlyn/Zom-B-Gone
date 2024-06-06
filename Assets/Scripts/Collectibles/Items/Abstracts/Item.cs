using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour, IInteractable
{
    protected enum State
    {
        GROUNDED, AIRBORNE, HELD
    }
    protected State _currentState;

    [Header("Item attributes")]
    // Items will be found with different levels of "quality" affecting the effectiveness of the item 
    // Items will break after the quality reaches 0
    [SerializeField] protected int _quality;

    // Value that determines the effect it has on the players movement when held, also determines throw damage and speed
    [SerializeField, Range(1, 20000), Tooltip("In grams")] public float _weight; // grams

    // Values that will be multiplied with velocity and angularVelocity to create friction
    [SerializeField, Range(0.9f, 1.0f)] protected float _rotationalFriction = 0.9f;
    [SerializeField, Range(0.9f, 1.0f), Tooltip("Higher = less friction")] protected float friction = 0.98f;

    [SerializeField] Vector2 _holdOffset = new Vector2(0.5f, 0.5f);

    [SerializeField] protected bool _spinThrow = true;

    [SerializeField] protected bool _aimAtMouse = true;
    [SerializeField] protected float gripRotation = 130;
    [SerializeField] protected Transform pivotPoint;

    [SerializeField] protected float knockbackPower;

    [SerializeField] protected Collider2D fullCollider;

    protected Rigidbody2D _rb;
    protected PlayerController playerController;
    protected Hands playerHands;
    protected Head playerHead;
    protected bool _inRightHand;
    public bool _useHeld;

    protected bool moveToHand;
    private Vector3 pickupTarget;
    private Quaternion rotationTarget;
    private float pickupSpeed = 10;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerHands = playerController.GetComponentInParent<Hands>();
        playerHead = playerController.GetComponentInParent<Head>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
    }

    protected virtual void Update()
    {
        if(_currentState == State.AIRBORNE && _rb.velocity.magnitude < 3)
        {
            ChangeState(State.GROUNDED);
        }
        if(_currentState == State.GROUNDED)
        {
            Friction();
        }

        if(moveToHand)
        {
            ReturnToGrip();
		}
    }

    private void FixedUpdate()
    {
        if(_currentState == State.HELD && _aimAtMouse)
        {
            RotateToMouse();
        }
    }

    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.GROUNDED:
                gameObject.layer = LayerMask.NameToLayer("InteractableItem");
                if(_currentState == State.HELD)_rb.bodyType = RigidbodyType2D.Dynamic;
                StartCoroutine(EnableFullCollider());
                fullCollider.isTrigger = true;
				break;
            case State.AIRBORNE:
                gameObject.layer = LayerMask.NameToLayer("AirborneItem");
                if (_currentState == State.HELD) _rb.bodyType = RigidbodyType2D.Dynamic;
                StartCoroutine(EnableFullCollider());
                fullCollider.isTrigger = false;
                break;
            case State.HELD:
                gameObject.layer = LayerMask.NameToLayer("AirborneItem");
                _rb.bodyType = RigidbodyType2D.Kinematic;
                fullCollider.enabled = false;
                //fullCollider.isTrigger = true;
                break;
            default:
                break;

        }

        _currentState = newState;
    }

    public abstract void Use();

    public virtual void Drop()
    {
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(State.GROUNDED);
        _rb.AddForce(playerT.up * Utils.MapScalarToRange(friction, 0.999f, 3, 300, true), ForceMode2D.Impulse);
    }

    public virtual void Throw()
    {
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(State.AIRBORNE);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;

        float throwForce = Utils.MapWeightToRange(_weight, 10, 20, true);
        _rb.AddForce(direction * throwForce, ForceMode2D.Impulse);

        if(_spinThrow)
        {
            float spinForce = Utils.MapWeightToRange(_weight, 100, 700, true);
            if (!_inRightHand) spinForce *= -1;
            _rb.angularVelocity = spinForce;
        }

        StartCoroutine(Fall());
    }

    public virtual void PickUp(Transform parent, bool rightHand)
    {
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;

        ChangeState(State.HELD);
        
        if (rightHand) _inRightHand = true;
        else _inRightHand = false;
        CheckFlip();
        
        
        transform.SetParent(parent);
        PositionInHand();
    }

    protected void CheckFlip()
    {
        if((_inRightHand && transform.localScale.x < 0) || (!_inRightHand && transform.localScale.x > 0))
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

    private void Friction()
    {
        _rb.velocity *= friction;
        _rb.angularVelocity *= _rotationalFriction;
    }

    protected IEnumerator EnableFullCollider()
    {
        yield return new WaitForSeconds(0.1f);
        if(!fullCollider.enabled) fullCollider.enabled = true;
    }

    private IEnumerator Fall()
    {
        float fallTime = Utils.MapWeightToRange(_weight, 1, 2, true);

        yield return new WaitForSeconds(fallTime);
        
        if(_currentState == State.AIRBORNE) 
            ChangeState(State.GROUNDED);
    }

    protected void PositionInHand()
    {
        if (_inRightHand) rotationTarget = Quaternion.Euler(0, 0, -gripRotation);
        else rotationTarget = Quaternion.Euler(0, 0, gripRotation);

        CheckFlip();
		
        moveToHand = true; // see update()
    }

    protected void ReturnToGrip()
    {
		if (_inRightHand)
		{
			pickupTarget = transform.parent.position + (transform.parent.right * _holdOffset.x + transform.parent.up * _holdOffset.y);
		}
		else
		{
			pickupTarget = transform.parent.position + (-transform.parent.right * _holdOffset.x + transform.parent.up * _holdOffset.y);
		}
		if (Vector3.Distance(transform.position, pickupTarget) < 0.02f)
		{
			transform.position = pickupTarget;
			moveToHand = false;
			return;
		}
		transform.position = Vector2.Lerp(transform.position, pickupTarget, Time.deltaTime * pickupSpeed);
		if (!_aimAtMouse) transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, pickupSpeed * Time.deltaTime);
	}

    protected virtual void RemoveFromHand()
    {
        StartCoroutine(EnableFullCollider());
        moveToHand = false;

        if (_inRightHand)
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
        if(_currentState == State.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            collisionHealth.TakeDamage(Utils.MapWeightToRange(_weight, 5, 100, false));
            if (collisionHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb))
            {
                hitRb.AddForce(_rb.velocity.normalized * knockbackPower * 0.5f, ForceMode2D.Impulse);
            }
        }
    }

    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}
