using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour, IInteractable
{
    [SerializeField] protected string _name;

    // Items will be found with different levels of "quality" affecting the effectiveness of the item 
    // Items will break after the quality reaches 0
    [SerializeField] protected int _quality;

    // Value that determines the effect it has on the players movement when held, also determines throw damage and speed
    [SerializeField] protected float _weight; // kg

    // Values that will be multiplied with velocity and angularVelocity to create friction
    [SerializeField,Range(0.9f,1.0f)] protected float _rotationalFriction = 0.9f;
    [SerializeField, Range(0.9f, 1.0f)] protected float _friction = 0.99f;

    protected Rigidbody2D _rb;
    protected Collider2D _collider;

    protected PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _rb = GetComponent<Rigidbody2D>(); 
        _collider = GetComponent<Collider2D>();
    }

    protected void Update()
    {
        if(_rb.bodyType == RigidbodyType2D.Dynamic)
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

    public abstract void Use();

    public void Drop()
    {
        // Remove item from player and place it at their feet
        transform.SetParent(null);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.AddForce(transform.up * 100);
        StartCoroutine(TriggerToSolid());
    }

    public void Throw()
    {
        // Eject item in direction aiming, do damage based on weight
        // Should happen when player presses drop button and moving forward

        // make items have a rigidbody that is kinematic when held, and dynamic when dropped/thrown
    }

    public void PickUp(Transform parent, bool rightHand)
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _collider.isTrigger = true;
        
        if (rightHand)
        {
            transform.SetParent(parent);
            transform.position = parent.position + (parent.right + parent.up) * 0.5f;
            transform.rotation = parent.rotation;
        }
        else
        {
            transform.SetParent(parent);
            transform.position = parent.position + (-parent.right + parent.up) * 0.5f;
            transform.rotation = parent.rotation;
        }
    }

    public void Interact(bool rightHand)
    {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        PickUp(_playerController.transform, rightHand);
    }

    private void Friction()
    {
        _rb.velocity *= _friction;
        _rb.angularVelocity *= _rotationalFriction;
        
    }

    private IEnumerator TriggerToSolid()
    {
        yield return new WaitForSeconds(0.1f);
        _collider.isTrigger = false;
    }

}
