using GameEvents;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] public PlayerData playerData;
    [SerializeField] private Hands hands;
    [SerializeField] private Head head;
    [SerializeField] private Rigidbody2D rb;
    private Slider staminaSlider;
    private Camera gameCamera;
    private Interactor interactor;

    [HideInInspector] public bool holdingLeft;
    [HideInInspector] public bool holdingRight;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public bool recoverStamina;
    private float currentMoveSpeed;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    private bool holdingRun;

    private void Awake()
    {
        //Cursor.visible = false;
        // Find references
        gameCamera = FindObjectOfType<Camera>();
        interactor = GetComponent<Interactor>();
        rb.freezeRotation = true;
        staminaSlider = GameObject.FindGameObjectWithTag("Stamina").GetComponent<Slider>();

        // Set currents
        currentStamina = playerData.maxStamina;
        currentMoveSpeed = playerData.walkSpeed;
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
                if (currentStamina < playerData.maxStamina) currentStamina += Time.deltaTime * playerData.staminaRecoverySpeed;
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
        staminaSlider.value = currentStamina / playerData.maxStamina;
    }

    public void RecoverStamina()
    {
        recoverTimer = playerData.staminaRecoveryDelay;
    }

    private void SetPlayerVelocity()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, playerData.velocityChangeSpeed * Time.deltaTime * 100);
        Vector3 newVelocity = (smoothedMovementInput) * currentMoveSpeed * playerData.speedModifier * hands.leftLumbering * hands.rightLumbering;
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

    #region INPUT ACTIONS -----------------------------------------------------

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
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (inputValue.isPressed)
        {
		    if (!hands.UsingLeft)
            {
                interactor.Interact(false);
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
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (inputValue.isPressed)
		{
			if (!hands.UsingRight)
            {
                interactor.Interact(true);
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
        if (EventSystem.current.IsPointerOverGameObject()) return;

		holdingLeft = true;
        if (hands.leftItem != null) hands.leftItem.useHeld = true;

    }

    private void OnRightHold(InputValue inputValue)
    {
		if (EventSystem.current.IsPointerOverGameObject()) return;
		holdingRight = true;
        if (hands.rightItem != null) hands.rightItem.useHeld = true;
    }

    private void OnThrowLeft(InputValue inputValue)
    {
        if (hands.UsingLeft)
        {
            hands.leftItem.Throw();
			hands.leftLumbering = 1;
		}

	}

	private void OnThrowRight(InputValue inputValue)
	{
		if (hands.UsingRight)
        {
			hands.rightItem.Throw();
			hands.rightLumbering = 1;
		}
	}

    private void OnDropLeft(InputValue inputValue)
    {
        DropLeft();
    }

    private void OnDropRight(InputValue inputValue)
    {
        DropRight();
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
                        leftProjectileWeapon.StartReload(playerData.reloadSpeedReduction);
                    }
                    else
                    {
                        rightProjectileWeapon.StartReload(playerData.reloadSpeedReduction);
                    }
                }
                else if (leftProjectileWeapon.reloading && !rightProjectileWeapon.reloading) rightProjectileWeapon.StartReload(playerData.reloadSpeedReduction);
                else if (!leftProjectileWeapon.reloading && rightProjectileWeapon.reloading) leftProjectileWeapon.StartReload(playerData.reloadSpeedReduction);
            }
            else if(!leftProjectileWeapon.reloading) leftProjectileWeapon.StartReload(playerData.reloadSpeedReduction);
            
		}
        else if(hands.UsingRight && hands.RightObject.TryGetComponent(out ProjectileWeapon rightprojectileWeapon) && !rightprojectileWeapon.reloading)
        {
            rightprojectileWeapon.StartReload(playerData.reloadSpeedReduction);
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

	#endregion -----------------------------------------

	public void DropLeft()
	{
		if (hands.UsingLeft)
		{
			hands.leftItem.Drop();
			hands.leftLumbering = 1;
		}
	}

	public void DropRight()
	{
		if (hands.UsingRight)
		{
			hands.rightItem.Drop();
			hands.rightLumbering = 1;
		}
	}

	public void DropHat()
    {
        head.wornHat.DropHat();
    }

    private void ChangeState(State newState)
    {
		if (currentState == newState) return;

		switch (newState)
        {
            case State.WALKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = playerData.walkSpeed;
                break;
            case State.RUNNING:
                currentMoveSpeed = playerData.runSpeed;
                recoverStamina = false;
                break;
            case State.SNEAKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = playerData.sneakSpeed;
                break;
            default: // IDLE:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = playerData.walkSpeed; 
                break;
        }

        currentState = newState;
    }
}


