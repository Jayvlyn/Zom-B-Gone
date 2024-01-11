using UnityEditor;
using UnityEngine;

public abstract class Item : MonoBehaviour, IInteractable
{
    [SerializeField] protected string _name;

    // Items will be found with different levels of "quality" affecting the effectiveness of the item 
    // Items will break after the quality reaches 0
    [SerializeField] protected int _quality;

    // Value that determines the effect it has on the players movement when held, also determines throw damage and speed
    [SerializeField] protected float _weight; // kg

    protected PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
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
    }

    public void Throw()
    {
        // Eject item in direction aiming, do damage based on weight
        // Should happen when player presses drop button and moving forward
    }

    public void PickUp(Transform parent, bool rightHand)
    {
        if(rightHand)
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

}
