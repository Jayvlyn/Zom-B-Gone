using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    private enum State
    {
        IDLE, WALKING, RUNNING, SNEAKING
    }
    private State _currentState;

    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 9;
    [SerializeField] private float _sneakSpeed = 3;
    [SerializeField] private float _maxStamina = 100;
    [SerializeField] private float _velocityChangeSpeed = 0.17f;
    
    private float _currentStamina;
    private float _currentMoveSpeed;
    private Rigidbody2D _rigidBody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;

    private Camera _gameCamera;
    private Interactor _interactor;

    private void Awake()
    {
        _gameCamera = FindObjectOfType<Camera>();
        _interactor = GetComponent<Interactor>();
        _currentStamina = _maxStamina;
        _currentMoveSpeed = _walkSpeed;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateToMouse();
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _movementInput, ref _movementInputSmoothVelocity, _velocityChangeSpeed);
        _rigidBody.velocity = _smoothedMovementInput * _currentMoveSpeed;
    }

    private void RotateToMouse()
    {
        Vector2 mousePosition = _gameCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.up = mousePosition - new Vector2(transform.position.x, transform.position.y);
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    // Player will be able to be empty handed, or pickup ONE item from the world
    // There will be no inventory, so the player must throw away/use up their current item to get a new one
    // Items include but are not limited to weapons
    private void OnUseItem(InputValue inputValue)
    {
        // if has item, item.use
    }

    private void OnInteract(InputValue inputValue)
    {
        _interactor.Interact();
    }

    private void OnReload(InputValue inputValue)
    {
        // if has firearm:weapon:item and ammo not full, reload
    }

    private void OnRun(InputValue inputValue)
    {
        bool runPressed = Convert.ToBoolean(inputValue.Get<float>());

        if (runPressed)
        {
            if (_rigidBody.velocity.magnitude > 0 && _currentStamina > 0)
            {
                ChangeState(State.RUNNING);
            }
        }
        else ChangeState(State.WALKING);
        
    }
    private void OnSneak(InputValue inputValue)
    {
        bool sneakPressed = Convert.ToBoolean(inputValue.Get<float>());

        if (sneakPressed)
        {
            ChangeState(State.SNEAKING);
        }
        else ChangeState(State.WALKING);
    }

    private void ChangeState(State newState)
    {
        _currentState = newState;

        switch (_currentState)
        {
            case State.WALKING:
                _currentMoveSpeed = _walkSpeed;
                break;
            case State.RUNNING:
                _currentMoveSpeed = _runSpeed;
                break;
            case State.SNEAKING:
                _currentMoveSpeed = _sneakSpeed;
                break;
            default:
                _currentMoveSpeed = _walkSpeed; 
                break;
                
        }
    }
}


