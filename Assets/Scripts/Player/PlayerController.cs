using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private enum State
    {
        IDLE, WALKING, RUNNING, SNEAKING
    }
    private State _currentState;

    [Header("References")]
    private Rigidbody2D _rigidBody;
    private Camera _gameCamera;
    private Interactor _interactor;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Hands _hands;

    [Header("Properties")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 9;
    [SerializeField] private float _sneakSpeed = 3;
    public float _speedModifier = 1;
    [SerializeField] public float _maxStamina = 15;
    [SerializeField] public float _staminaRecoverySpeed = 2;
    [SerializeField] private float _staminaRecoveryDelay = 2.5f;
    [SerializeField] private float _velocityChangeSpeed = 0.17f;
    [SerializeField] private float _reloadSpeedReduction = 0.7f;
    
    public float _currentStamina;
    private float _currentMoveSpeed;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private bool _recoverStamina;
    private bool _holdingRun;


    private void Awake()
    {
        //Cursor.visible = false;
        // Find references
        _gameCamera = FindObjectOfType<Camera>();
        _interactor = GetComponent<Interactor>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.freezeRotation = true;

        // Set currents
        _currentStamina = _maxStamina;
        _currentMoveSpeed = _walkSpeed;
    }

    private void Update()
    {
        UpdateStaminaBar();

        if (_currentState == State.RUNNING) 
        {
            _currentStamina -= Time.deltaTime;
            if (_currentStamina <= 0)
            {
                _currentStamina = 0;
                ChangeState(State.WALKING);
                StartCoroutine(RecoverStamina());
            }
        }
        else
        {
            if(_recoverStamina)
            {
                if (_currentStamina < _maxStamina) _currentStamina += Time.deltaTime * _staminaRecoverySpeed;
                else _recoverStamina = false;
            }
        }


    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateToMouse();
    }

    private void UpdateStaminaBar()
    {
        _staminaSlider.value = _currentStamina / _maxStamina;
    }

    private IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(_staminaRecoveryDelay);
        if(!_holdingRun) _recoverStamina = true;
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _movementInput, ref _movementInputSmoothVelocity, _velocityChangeSpeed);
        _rigidBody.velocity = (_smoothedMovementInput) * _currentMoveSpeed * _speedModifier;
    }

    private void RotateToMouse()
    {
        Vector2 mousePosition = _gameCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
    }

    #region Input Actions
    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
        if(_currentState != State.SNEAKING)
        {
            if(_movementInput != Vector2.zero && _currentState == State.IDLE) 
            {
                if (_holdingRun) ChangeState(State.RUNNING);
                else ChangeState(State.WALKING);
            }
            else if(_movementInput == Vector2.zero) { ChangeState(State.IDLE); }

        }
    }

    private void OnLeftHand(InputValue inputValue)
    {
        if(!_hands.UsingLeft) _interactor.Interact(false);
        else
        {
            if(_hands._leftObject.TryGetComponent(out Item item))
            {
                item.Use();
            }
        }
    }

    private void OnRightHand(InputValue inputValue)
    {
        if (!_hands.UsingRight) _interactor.Interact(true);
        else
        {
            if (_hands._rightObject.TryGetComponent(out Item item))
            {
                item.Use();
            }
        }
    }

    private void OnDropLeft(InputValue inputValue)
    {
        if (_hands.UsingLeft)
        {
            _hands._leftObject.TryGetComponent<Item>(out Item leftObject);
            _hands._leftObject = null;
            _hands.UsingLeft = false;
            if (_movementInput.magnitude > 0)
            {
                leftObject.Throw();
            }
            else
            {
                leftObject.Drop();
            }

        }
    }

    private void OnDropRight(InputValue inputValue)
    {
        if (_hands.UsingRight)
        {
            _hands._rightObject.TryGetComponent<Item>(out Item rightObject);
            _hands._rightObject = null;
            _hands.UsingRight = false;
            if (_movementInput.magnitude > 0)
            {
                rightObject.Throw();
            }
            else
            {
                rightObject.Drop();
            }
        }
    }

    // Input for reloading, wont do anything without firearm
    private void OnReload(InputValue inputValue)
    {
        if(_hands.UsingLeft && _hands._leftObject.TryGetComponent(out Firearm leftFirearm))
        {
            leftFirearm.StartReload();
        }
        if (_hands.UsingRight && _hands._rightObject.TryGetComponent(out Firearm rightFirearm))
        {
            rightFirearm.StartReload();
        }
    }

    private void OnRun(InputValue inputValue)
    {
        bool runPressed = Convert.ToBoolean(inputValue.Get<float>());

        if (runPressed) // Run Key Pressed
        {
            _holdingRun = true;

            if(_rigidBody.velocity.magnitude > 0 && _movementInput != Vector2.zero && _currentStamina > 0)
            {
                ChangeState(State.RUNNING);
            }
        }

        else // Run Key Released
        {
            
            _holdingRun = false;

            if(_currentState == State.RUNNING) // Letting go of run key only matters if running
            {
                if (_movementInput != Vector2.zero) { ChangeState(State.WALKING); }
                else { ChangeState(State.IDLE); }
                if(!_recoverStamina)StartCoroutine(RecoverStamina());
            }
        }
    }

    // Called when sneak button changes input value
    private void OnSneak(InputValue inputValue)
    {
        bool sneakPressed = Convert.ToBoolean(inputValue.Get<float>());

        if (sneakPressed) // Sneak Key Pressed
        {
            ChangeState(State.SNEAKING);
        }

        else // Sneak Key Released
        {
            if(_currentState == State.SNEAKING) // Letting go of sneak key only matters if sneaking
            {
                if (_holdingRun) { ChangeState(State.RUNNING);}
                else if (_smoothedMovementInput != Vector2.zero) { ChangeState(State.WALKING); }
                else { ChangeState(State.IDLE); }
            }
        }
    }

    #endregion


    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.WALKING:
                _currentMoveSpeed = _walkSpeed;
                break;
            case State.RUNNING:
                _currentMoveSpeed = _runSpeed;
                _recoverStamina = false;
                break;
            case State.SNEAKING:
                _currentMoveSpeed = _sneakSpeed;
                break;
            default:
                _currentMoveSpeed = _walkSpeed; 
                break;
        }

        _currentState = newState;
    }
}


