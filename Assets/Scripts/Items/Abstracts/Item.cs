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
    [SerializeField] protected string _name;

    // Items will be found with different levels of "quality" affecting the effectiveness of the item 
    // Items will break after the quality reaches 0
    [SerializeField] protected int _quality;

    // Value that determines the effect it has on the players movement when held, also determines throw damage and speed
    [SerializeField, Range(1, 20000), Tooltip("In grams")] protected float _weight; // grams

    // Values that will be multiplied with velocity and angularVelocity to create friction
    [SerializeField, Range(0.9f, 1.0f)] protected float _rotationalFriction = 0.9f;
    [SerializeField, Range(0.9f, 1.0f), Tooltip("Higher = less friction")] protected float friction = 0.99f;

    [SerializeField] Vector2 _holdOffset = new Vector2(0.5f, 0.5f);

    [SerializeField] protected bool _spinThrow = true;

    [SerializeField] protected bool _aimAtMouse = true;
    [SerializeField] protected float gripRotation = 130;
    [SerializeField] private Transform? pivotPoint;

    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    protected PlayerController _playerController;
    protected Hands _playerHands;
    protected bool _inRightHand;
    public bool _useHeld;

    private bool moveToHand;
    private Vector3 pickupTarget;
    private float pickupSpeed = 10;


    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerHands = _playerController.GetComponentInParent<Hands>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        _collider = GetComponent<Collider2D>();
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
            if (_inRightHand)
            {
                pickupTarget = transform.parent.position + (transform.parent.right * _holdOffset.x + transform.parent.up * _holdOffset.y);
            }
            else
            {
                pickupTarget = transform.parent.position + (-transform.parent.right * _holdOffset.x + transform.parent.up * _holdOffset.y);
            }
            if (Vector3.Distance(transform.position,pickupTarget) < 0.2f)
            {
                transform.position = pickupTarget;
                moveToHand = false; 
                return;
            }
            transform.position = Vector2.Lerp(transform.position, pickupTarget, Time.deltaTime * pickupSpeed);
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
                gameObject.layer = LayerMask.NameToLayer("Interactable");
                if(_currentState == State.HELD)_rb.bodyType = RigidbodyType2D.Dynamic;
                StartCoroutine(TriggerToSolid());
				break;
            case State.AIRBORNE:
                gameObject.layer = LayerMask.NameToLayer("Interactable");
                if (_currentState == State.HELD) _rb.bodyType = RigidbodyType2D.Dynamic;
                StartCoroutine(TriggerToSolid());
                break;
            case State.HELD:
                gameObject.layer = LayerMask.NameToLayer("Default");
                _rb.bodyType = RigidbodyType2D.Kinematic;
                _collider.isTrigger = true;
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
        _rb.AddForce(playerT.up * Utils.MapScalarToRange(friction, 3, 500, true), ForceMode2D.Impulse);
    }

    public virtual void Throw()
    {
        Transform playerT = transform.parent;
        RemoveFromHand();
        ChangeState(State.AIRBORNE);

        float throwForce = Utils.MapWeightToRange(_weight, 2, 15, true);
        _rb.AddForce(playerT.up * throwForce, ForceMode2D.Impulse);

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
        
        if (rightHand)
        {
            _inRightHand = true;
        }
        else
        {
            _inRightHand = false;
        }
        transform.SetParent(parent);
        PositionInHand();
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
        PickUp(_playerController.transform, rightHand);
    }

    private void Friction()
    {
        _rb.velocity *= friction;
        _rb.angularVelocity *= _rotationalFriction;
    }

    private IEnumerator TriggerToSolid()
    {
        yield return new WaitForSeconds(0.1f);
        if(_collider.isTrigger)_collider.isTrigger = false;
    }

    private IEnumerator Fall()
    {
        float fallTime = Utils.MapWeightToRange(_weight, 1, 2, true);

        yield return new WaitForSeconds(fallTime);
        if(_currentState != State.HELD)ChangeState(State.GROUNDED);
    }

    protected void PositionInHand()
    {
        moveToHand = true; // see update()


        if (!_aimAtMouse)
        {
            transform.localRotation = Quaternion.identity;
        }

        ReturnToGrip();
    }

    protected void ReturnToGrip()
    {
        if(pivotPoint != null)
        {
            if (_inRightHand) transform.RotateAround(pivotPoint.position, Vector3.forward, -gripRotation);
            else transform.RotateAround(pivotPoint.position, Vector3.forward, gripRotation);
        }
    }

    protected virtual void RemoveFromHand()
    {
        if (_inRightHand)
        {
            _playerHands.RightObject = null; 
            _playerHands.UsingRight = false;
        }
        else
        {
            _playerHands.LeftObject = null; 
            _playerHands.UsingLeft = false;
        }

        transform.SetParent(null);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(_currentState == State.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            collisionHealth.TakeDamage(Utils.MapWeightToRange(_weight, 5, 100, false));
        }
    }

}
