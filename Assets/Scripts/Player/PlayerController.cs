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
    private State currentState;

    [Header("References")]
    private Rigidbody2D _rigidBody;
    private Camera _gameCamera;
    private Interactor _interactor;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Hands _hands;
    [SerializeField] private Head head;

    [Header("Properties")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 9;
    [SerializeField] private float _sneakSpeed = 3;
    public float _speedModifier = 1;
    [SerializeField] public float _maxStamina = 15;
    [SerializeField] public float _staminaRecoverySpeed = 2;
    [SerializeField] private float staminaRecoveryDelay = 2f;
    [SerializeField] private float _velocityChangeSpeed = 0.17f;
    [SerializeField] public float _reloadSpeedReduction = 1f;
    
    public float currentStamina;
    private float currentMoveSpeed;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    public bool recoverStamina;
    private bool holdingRun;

    public bool holdingLeft;
	public bool holdingRight;

    public float leftLumbering = 1;
    public float rightLumbering = 1;
    [SerializeField] private float lumberingLowerBound = 0.8f;

    private void Awake()
    {
        //Cursor.visible = false;
        // Find references
        _gameCamera = FindObjectOfType<Camera>();
        _interactor = GetComponent<Interactor>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.freezeRotation = true;

        // Set currents
        currentStamina = _maxStamina;
        currentMoveSpeed = _walkSpeed;
    }

    private float recoverTimer;
    private void Update()
    {
        UpdateStaminaBar();

        if (currentState == State.RUNNING) 
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                ChangeState(State.WALKING);
                RecoverStamina();
            }
        }
        else
        {
            if(recoverStamina)
            {
                if (currentStamina < _maxStamina) currentStamina += Time.deltaTime * _staminaRecoverySpeed;
                else recoverStamina = false;
            }
            else if(recoverTimer > 0)
            {
                recoverTimer -= Time.deltaTime;
                if (recoverTimer <= 0)
                {
                    recoverStamina = true;
                }
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
        _staminaSlider.value = currentStamina / _maxStamina;
    }

    public void RecoverStamina()
    {
        recoverTimer = staminaRecoveryDelay;
    }

    private void SetPlayerVelocity()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, _velocityChangeSpeed * Time.deltaTime * 100);
        Vector3 newVelocity = (smoothedMovementInput) * currentMoveSpeed * _speedModifier * leftLumbering * rightLumbering;
        #region hat buff
        if(head.wornHat != null)
        {
            newVelocity *= head.wornHat.moveSpeedMod;
        }
        #endregion
        _rigidBody.velocity = newVelocity;
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
        movementInput = inputValue.Get<Vector2>();
        if(currentState != State.SNEAKING)
        {
            if(movementInput != Vector2.zero && currentState == State.IDLE) 
            {
                if (holdingRun) ChangeState(State.RUNNING);
                else ChangeState(State.WALKING);
            }
            else if(movementInput == Vector2.zero) { ChangeState(State.IDLE); }
        }
    }

    private void OnLeftHand(InputValue inputValue)
    {
        if(inputValue.isPressed)
        {
		    if (!_hands.UsingLeft)
            {
                _interactor.Interact(false);
                if(_hands.UsingLeft)
                {
                    leftLumbering = Utils.MapWeightToRange(_hands._leftItem._weight, lumberingLowerBound, 1.0f, true);
                }
            }
            
			else if (_hands._leftItem != null) _hands._leftItem.Use();
		}
        else
        {
            holdingLeft = false;
            if (_hands._leftItem != null) _hands._leftItem._useHeld = false;
        }
    }

    private void OnRightHand(InputValue inputValue)
    {
		if (inputValue.isPressed)
		{
			if (!_hands.UsingRight)
            {
                _interactor.Interact(true);
				if (_hands.UsingRight)
				{
					rightLumbering = Utils.MapWeightToRange(_hands._rightItem._weight, lumberingLowerBound, 1.0f, true);
				}
			}
            
			else if (_hands._rightItem != null) _hands._rightItem.Use();
		}
		else
		{
			holdingRight = false;
            if (_hands._rightItem != null) _hands._rightItem._useHeld = false;
        }
	}

    private void OnLeftHold(InputValue inputValue)
    {
		holdingLeft = true;
        if (_hands._leftItem != null) _hands._leftItem._useHeld = true;

    }

    private void OnRightHold(InputValue inputValue)
    {
		holdingRight = true;
        if (_hands._rightItem != null) _hands._rightItem._useHeld = true;
    }

    private void OnDropLeft(InputValue inputValue)
    {
        if (_hands.UsingLeft)
        {
            if (movementInput.magnitude > 0) _hands._leftItem.Throw();
            
            else _hands._leftItem.Drop();

            leftLumbering = 1;
        }
    }

    private void OnDropRight(InputValue inputValue)
    {
        if (_hands.UsingRight)
        {
            if (movementInput.magnitude > 0) _hands._rightItem.Throw();
            
            else _hands._rightItem.Drop();

            rightLumbering = 1;
        }
    }

    // Input for reloading, wont do anything without firearm
    private void OnReload(InputValue inputValue)
    {
		if (_hands.UsingLeft && _hands.LeftObject.TryGetComponent(out Firearm leftFirearm))
        {
			if (_hands.UsingRight && _hands.RightObject.TryGetComponent(out Firearm rightFirearm))
            {
				if (!leftFirearm._reloading && !rightFirearm._reloading)
                { // Neither gun reloading yet
                    float leftRatio = leftFirearm.CurrentAmmo / (float)leftFirearm._maxAmmo;
                    float rightRatio = rightFirearm.CurrentAmmo / (float)rightFirearm._maxAmmo;
                    if (leftRatio <= rightRatio)
                    {
                        leftFirearm.StartReload(_reloadSpeedReduction);
                    }
                    else
                    {
                        rightFirearm.StartReload(_reloadSpeedReduction);
                    }
                }
                else if (leftFirearm._reloading && !rightFirearm._reloading) rightFirearm.StartReload(_reloadSpeedReduction);
                else if (!leftFirearm._reloading && rightFirearm._reloading) leftFirearm.StartReload(_reloadSpeedReduction);
            }
            else if(!leftFirearm._reloading) leftFirearm.StartReload(_reloadSpeedReduction);
            
		}
        else if(_hands.UsingRight && _hands.RightObject.TryGetComponent(out Firearm rightFirearm) && !rightFirearm._reloading)
        {
            rightFirearm.StartReload(_reloadSpeedReduction);
        }
	}

    private void OnRun(InputValue inputValue)
    {
        if (inputValue.isPressed) // Run Key Pressed
        {
            holdingRun = true;

            if(_rigidBody.velocity.magnitude > 0 && movementInput != Vector2.zero && currentStamina > 0)
            {
                ChangeState(State.RUNNING);
            }
        }

        else // Run Key Released
        {
            
            holdingRun = false;

            if(currentState == State.RUNNING) // Letting go of run key only matters if running
            {
                if (movementInput != Vector2.zero) { ChangeState(State.WALKING); }
                else { ChangeState(State.IDLE); }
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
            if(currentState == State.SNEAKING) // Letting go of sneak key only matters if sneaking
            {
                if (holdingRun) { ChangeState(State.RUNNING);}
                else if (smoothedMovementInput != Vector2.zero) { ChangeState(State.WALKING); }
                else { ChangeState(State.IDLE); }
            }
        }
    }

    #endregion

    private void ChangeState(State newState)
    {
		if (currentState == newState) return;

		switch (newState)
        {
            case State.WALKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = _walkSpeed;
                break;
            case State.RUNNING:
                currentMoveSpeed = _runSpeed;
                recoverStamina = false;
                break;
            case State.SNEAKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = _sneakSpeed;
                break;
            default: // IDLE:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = _walkSpeed; 
                break;
        }

        currentState = newState;
    }
}


