using UnityEngine;

public interface IInteractable
{
    public void Interact(bool rightHand);
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 1.5f;

    [SerializeField] private LayerMask InteractionLm;

    private Hands _hands;
    private RaycastHit2D _hit;
    private Camera _gameCamera;

    private void Awake()
    {
        _gameCamera = FindObjectOfType<Camera>();
        _hands = GetComponent<Hands>();
    }

    public void Interact(bool rightHand)
    {
        _hit = Physics2D.CircleCast(transform.position, _interactRange, Vector2.zero, 0, InteractionLm);

        if (_hit.collider != null && _hit.collider.TryGetComponent(out IInteractable interactedObject))
        {
            if(interactedObject != null)
            {
                interactedObject.Interact(rightHand);
                if (rightHand) {
                    _hands._rightObject = ((Component)interactedObject).gameObject; _hands._usingRight = true;
                }
                else {
                    _hands._leftObject = ((Component)interactedObject).gameObject; _hands._usingLeft = true;
                }
            }
        }
    }
}
