using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour, IInteractable
{
    private enum State
    {
        GROUNDED, AIRBORNE, HELD
    }
    private State _currentState;

    [SerializeField] protected string _name;

    // Items will be found with different levels of "quality" affecting the effectiveness of the item 
    // Items will break after the quality reaches 0
    [SerializeField] protected int _quality;

    // Value that determines the effect it has on the players movement when held, also determines throw damage and speed
    [SerializeField, Range(1, 20000), Tooltip("In grams")] protected float _weight; // grams

    // Values that will be multiplied with velocity and angularVelocity to create friction
    [SerializeField, Range(0.9f, 1.0f)] protected float _rotationalFriction = 0.9f;
    [SerializeField, Range(0.9f, 1.0f), Tooltip("Higher = less friction")] protected float _friction = 0.99f;

    [SerializeField] protected bool spinThrow = true;

    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    protected PlayerController _playerController;
    protected Hands _playerHands;
    protected bool inRightHand;


    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerHands = _playerController.GetComponentInParent<Hands>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        _collider = GetComponent<Collider2D>();
    }

    protected void Update()
    {
        if(_currentState == State.AIRBORNE && _rb.velocity.magnitude < 3)
        {
            ChangeState(State.GROUNDED);
        }
        if(_currentState == State.GROUNDED)
        {
            Friction();
        }
    }

    protected Item()
    {
        _name = "Item";
        _quality = 100;
        _weight = 1f;
    }

    protected Item(string name, int quality, float weight)
    {
        _name = name;
        _quality = quality;
        _weight = weight;
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
                gameObject.layer = LayerMask.NameToLayer("Default");
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

    public void Drop()
    {
        transform.SetParent(null);
        ChangeState(State.GROUNDED);
        _rb.AddForce(transform.up * Utils.MapScalarToRange(_friction, 3, 500, true), ForceMode2D.Impulse);

    }

    public void Throw()
    {
        transform.SetParent(null);
        ChangeState(State.AIRBORNE);

        float throwForce = Utils.MapWeightToRange(_weight, 5, 25, true);
        _rb.AddForce(transform.up * throwForce, ForceMode2D.Impulse);

        if(spinThrow)
        {
            float spinForce = Utils.MapWeightToRange(_weight, 200, 2000, true);
            _rb.angularVelocity = spinForce;
        }

        StartCoroutine(Fall());
    }

    public void PickUp(Transform parent, bool rightHand)
    {
        ChangeState(State.HELD);
        
        if (rightHand)
        {
            inRightHand = true;
            transform.SetParent(parent);
            transform.position = parent.position + (parent.right + parent.up) * 0.5f;
            transform.rotation = parent.rotation;
        }
        else
        {
            inRightHand = false;
            transform.SetParent(parent);
            transform.position = parent.position + (-parent.right + parent.up) * 0.5f;
            transform.rotation = parent.rotation;
        }
    }

    public void Interact(bool rightHand)
    {
        PickUp(_playerController.transform, rightHand);
    }

    private void Friction()
    {
        _rb.velocity *= _friction;
        _rb.angularVelocity *= _rotationalFriction;
    }

    private IEnumerator TriggerToSolid()
    {
        yield return new WaitForSeconds(0.05f);
        _collider.isTrigger = false;
    }

    private IEnumerator Fall()
    {
        float fallTime = Utils.MapWeightToRange(_weight, 1, 3, true);

        yield return new WaitForSeconds(fallTime);
        ChangeState(State.GROUNDED);
    }

}
