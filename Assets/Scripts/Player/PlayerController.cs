using GameEvents;
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
    private Rigidbody2D rb;
    private Camera gameCamera;
    private Interactor interactor;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Hands hands;
    [SerializeField] private Head head;

    [Header("Properties")]
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 9;
    [SerializeField] private float sneakSpeed = 3;
    public float speedModifier = 1;
    [SerializeField] public float maxStamina = 15;
    [SerializeField] public float staminaRecoverySpeed = 2;
    [SerializeField] private float staminaRecoveryDelay = 2f;
    [SerializeField] private float velocityChangeSpeed = 0.17f;
    [SerializeField] public float reloadSpeedReduction = 1f;
    
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
        gameCamera = FindObjectOfType<Camera>();
        interactor = GetComponent<Interactor>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        // Set currents
        currentStamina = maxStamina;
        currentMoveSpeed = walkSpeed;
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
                if (currentStamina < maxStamina) currentStamina += Time.deltaTime * staminaRecoverySpeed;
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
        staminaSlider.value = currentStamina / maxStamina;
    }

    public void RecoverStamina()
    {
        recoverTimer = staminaRecoveryDelay;
    }

    private void SetPlayerVelocity()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, velocityChangeSpeed * Time.deltaTime * 100);
        Vector3 newVelocity = (smoothedMovementInput) * currentMoveSpeed * speedModifier * leftLumbering * rightLumbering;
        #region hat buff
        if(head.wornHat != null)
        {
            newVelocity *= head.wornHat.hatData.moveSpeedMod;
        }
        #endregion
        rb.velocity = newVelocity;
    }

    private void RotateToMouse()
    {
        Vector2 mousePosition = gameCamera.ScreenToWorldPoint(Input.mousePosition);

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
		    if (!hands.UsingLeft)
            {
                interactor.Interact(false);
                if(hands.UsingLeft)
                {
                    leftLumbering = Utils.MapWeightToRange(hands.leftItem.itemData.weight, lumberingLowerBound, 1.0f, true);
                }
            }
            
			else if (hands.leftItem != null) hands.leftItem.Use();
		}
        else
        {
            holdingLeft = false;
            if (hands.leftItem != null) hands.leftItem.useHeld = false;
        }
    }

    private void OnRightHand(InputValue inputValue)
    {
		if (inputValue.isPressed)
		{
			if (!hands.UsingRight)
            {
                interactor.Interact(true);
				if (hands.UsingRight)
				{
					rightLumbering = Utils.MapWeightToRange(hands.rightItem.itemData.weight, lumberingLowerBound, 1.0f, true);
				}
			}
            
			else if (hands.rightItem != null) hands.rightItem.Use();
		}
		else
		{
			holdingRight = false;
            if (hands.rightItem != null) hands.rightItem.useHeld = false;
        }
	}

    private void OnLeftHold(InputValue inputValue)
    {
		holdingLeft = true;
        if (hands.leftItem != null) hands.leftItem.useHeld = true;

    }

    private void OnRightHold(InputValue inputValue)
    {
		holdingRight = true;
        if (hands.rightItem != null) hands.rightItem.useHeld = true;
    }

    private void OnDropLeft(InputValue inputValue)
    {
        if (hands.UsingLeft)
        {
            if (movementInput.magnitude > 0) hands.leftItem.Throw();
            
            else hands.leftItem.Drop();

            leftLumbering = 1;
        }
    }

    private void OnDropRight(InputValue inputValue)
    {
        if (hands.UsingRight)
        {
            if (movementInput.magnitude > 0) hands.rightItem.Throw();
            
            else hands.rightItem.Drop();

            rightLumbering = 1;
        }
    }

    // Input for reloading, wont do anything without projectileWeapon
    private void OnReload(InputValue inputValue)
    {
		if (hands.UsingLeft && hands.LeftObject.TryGetComponent(out ProjectileWeapon leftProjectileWeapon))
        {
			if (hands.UsingRight && hands.RightObject.TryGetComponent(out ProjectileWeapon rightProjectileWeapon))
            {
				if (!leftProjectileWeapon.reloading && !rightProjectileWeapon.reloading)
                { // Neither gun reloading yet
                    float leftRatio = leftProjectileWeapon.CurrentAmmo / (float)leftProjectileWeapon.projectileWeaponData.maxAmmo;
                    float rightRatio = rightProjectileWeapon.CurrentAmmo / (float)rightProjectileWeapon.projectileWeaponData.maxAmmo;
                    if (leftRatio <= rightRatio)
                    {
                        leftProjectileWeapon.StartReload(reloadSpeedReduction);
                    }
                    else
                    {
                        rightProjectileWeapon.StartReload(reloadSpeedReduction);
                    }
                }
                else if (leftProjectileWeapon.reloading && !rightProjectileWeapon.reloading) rightProjectileWeapon.StartReload(reloadSpeedReduction);
                else if (!leftProjectileWeapon.reloading && rightProjectileWeapon.reloading) leftProjectileWeapon.StartReload(reloadSpeedReduction);
            }
            else if(!leftProjectileWeapon.reloading) leftProjectileWeapon.StartReload(reloadSpeedReduction);
            
		}
        else if(hands.UsingRight && hands.RightObject.TryGetComponent(out ProjectileWeapon rightprojectileWeapon) && !rightprojectileWeapon.reloading)
        {
            rightprojectileWeapon.StartReload(reloadSpeedReduction);
        }
	}

    private void OnRun(InputValue inputValue)
    {
        if (inputValue.isPressed) // Run Key Pressed
        {
            holdingRun = true;

            if(rb.velocity.magnitude > 0 && movementInput != Vector2.zero && currentStamina > 0)
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

    [SerializeField] protected VoidEvent onBackpackToggle = null;
    private void OnToggleBackpack(InputValue inputValue)
    {
        onBackpackToggle.Raise();
    }

    #endregion

    private void ChangeState(State newState)
    {
		if (currentState == newState) return;

		switch (newState)
        {
            case State.WALKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = walkSpeed;
                break;
            case State.RUNNING:
                currentMoveSpeed = runSpeed;
                recoverStamina = false;
                break;
            case State.SNEAKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = sneakSpeed;
                break;
            default: // IDLE:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = walkSpeed; 
                break;
        }

        currentState = newState;
    }
}


