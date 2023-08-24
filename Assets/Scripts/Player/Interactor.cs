using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 1.5f;

    [SerializeField] private LayerMask InteractionLm;

    private RaycastHit2D _hit;

    private Camera _gameCamera;

    private void Awake()
    {
        _gameCamera = FindObjectOfType<Camera>();
    }

    public void Interact()
    {
        _hit = Physics2D.CircleCast(transform.position, _interactRange, Vector2.zero, 0, InteractionLm);
        if (_hit.collider != null && _hit.collider.TryGetComponent(out IInteractable interactedObject))
        {
            if(interactedObject != null) interactedObject.Interact();
        }
        Debug.Log(_hit.distance);
    }
}
