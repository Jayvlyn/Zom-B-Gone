using GameEvents;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        IDLE, WALKING, RUNNING, SNEAKING, DRIVING
    }
    public PlayerState currentState;

    [Header("References")]
    public PlayerData playerData;
    public Hands hands;
    public Head head;
    public Rigidbody2D rb;
    public PlayerInput input;
    private Slider staminaSlider;
    private Camera gameCamera;
    private Interactor interactor;
    
    [HideInInspector] public static bool holdingRun;
    [HideInInspector] public static bool holdingSneak;
    [HideInInspector] public static bool holdingLeft;
    [HideInInspector] public static bool holdingRight;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public bool recoverStamina;
    private float currentMoveSpeed;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;

    public static ContainerDragHandler mouseHeldIcon = null;

    private void Awake()
    {
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

        if (currentState == PlayerState.RUNNING) 
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                ChangeState(PlayerState.WALKING);
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
        if(currentState != PlayerState.DRIVING) RotateToMouse();
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
        if(currentState != PlayerState.SNEAKING)
        {
            if(movementInput != Vector2.zero && currentState == PlayerState.IDLE) 
            {
                if (holdingRun) ChangeState(PlayerState.RUNNING);
                else ChangeState(PlayerState.WALKING);
            }
            else if(movementInput == Vector2.zero) { ChangeState(PlayerState.IDLE); }
        }
    }

    private void OnLeftHand(InputValue inputValue)
    {
		if (inputValue.isPressed)
        {
		    if (EventSystem.current.IsPointerOverGameObject()) return;

		    if (!hands.UsingLeft) interactor.Interact(false);
            
			else if (hands.leftItem != null) hands.leftItem.Use();
		}
        else
        {
            holdingLeft = false;
            if (hands.leftItem != null) hands.leftItem.useHeld = false;
            else if (hands.leftObstacle != null)
            {
                hands.leftObstacle.ChangeMotorSpeed();
            }
        }
    }

    private void OnRightHand(InputValue inputValue)
    {
		if (inputValue.isPressed)
		{
		    if (EventSystem.current.IsPointerOverGameObject()) return;

			if (!hands.UsingRight) interactor.Interact(true);
			
			else if (hands.rightItem != null) hands.rightItem.Use();
		}
		else
		{
			holdingRight = false;
            if (hands.rightItem != null) hands.rightItem.useHeld = false;
            else if (hands.rightObstacle != null)
            {
                hands.rightObstacle.ChangeMotorSpeed();
            }
        }
	}

    private void OnLeftHold(InputValue inputValue)
    {
		holdingLeft = true;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (hands.leftItem != null) hands.leftItem.useHeld = true;
        else if (hands.leftObstacle != null)
        {
            if (holdingSneak) hands.leftObstacle.ChangeMotorSpeed(-playerData.obstacleTurningSpeed);
            else hands.leftObstacle.ChangeMotorSpeed(playerData.obstacleTurningSpeed);
        }

    }

    private void OnRightHold(InputValue inputValue)
    {
		holdingRight = true;
		if (EventSystem.current.IsPointerOverGameObject()) return;
        if (hands.rightItem != null) hands.rightItem.useHeld = true;
        else if (hands.rightObstacle != null)
        {
            if (holdingSneak) hands.rightObstacle.ChangeMotorSpeed(-playerData.obstacleTurningSpeed);
            else hands.rightObstacle.ChangeMotorSpeed(playerData.obstacleTurningSpeed);
        }
    }

    private void OnThrowLeft(InputValue inputValue)
    {
        if (hands.UsingLeft)
        {
            if(hands.leftItem != null)
            {
                hands.leftItem.Throw();
                hands.leftLumbering = 1;
            }
            else if(hands.leftObstacle != null)
            {
                LetGoLeftObstacle();
            }
		}

	}

	private void OnThrowRight(InputValue inputValue)
	{
		if (hands.UsingRight)
        {
            if (hands.rightItem != null)
            {
                hands.rightItem.Throw();
                hands.rightLumbering = 1;
            }
            else if (hands.rightObstacle != null)
            {
                LetGoRightObstacle();
            }
        }
	}

    private void OnDropLeft(InputValue inputValue)
    {
        if (hands.leftItem != null) DropLeft();
        else if (hands.leftObstacle != null) LetGoLeftObstacle();
    }

    private void OnDropRight(InputValue inputValue)
    {
        if (hands.rightItem != null) DropRight();
        else if (hands.rightObstacle != null) LetGoRightObstacle();
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
                ChangeState(PlayerState.RUNNING);
            }
        }

        else // Run Key Released
        {
            
            holdingRun = false;

            if(currentState == PlayerState.RUNNING) // Letting go of run key only matters if running
            {
                if (movementInput != Vector2.zero) { ChangeState(PlayerState.WALKING); }
                else { ChangeState(PlayerState.IDLE); }
            }
        }
    }

    // Called when sneak button changes input value
    private void OnSneak(InputValue inputValue)
    {
        bool sneakPressed = Convert.ToBoolean(inputValue.Get<float>());

        if (sneakPressed) // Sneak Key Pressed
        {
            ChangeState(PlayerState.SNEAKING);
            holdingSneak = true;
        }

        else // Sneak Key Released
        {
            holdingSneak = false;
            if(currentState == PlayerState.SNEAKING) // Letting go of sneak key only matters if sneaking
            {
                if (holdingRun) { ChangeState(PlayerState.RUNNING);}
                else if (smoothedMovementInput != Vector2.zero) { ChangeState(PlayerState.WALKING); }
                else { ChangeState(PlayerState.IDLE); }
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
        else if(hands.leftObstacle != null)
        {
            LetGoLeftObstacle();
        }
    }

	public void DropRight()
	{
		if (hands.UsingRight)
		{
			hands.rightItem.Drop();
			hands.rightLumbering = 1;
		}
        else if (hands.rightObstacle != null)
        {
            LetGoRightObstacle();
        }
    }

    public void LetGoLeftObstacle()
    {
        if (hands.leftObstacle && hands.rightObstacle && hands.leftObstacle == hands.rightObstacle)
        {
            hands.leftObstacle.OnOneHandOn();
        }
        else
        {
            hands.leftObstacle.BeFreed();
        }
        hands.leftObstacle = null;
        hands.UsingLeft = false;
    }

    public void LetGoRightObstacle()
    {
        if (hands.rightObstacle && hands.leftObstacle && hands.rightObstacle == hands.leftObstacle)
        {
            hands.rightObstacle.OnOneHandOn();
        }
        else
        {
            hands.rightObstacle.BeFreed();
        }
        hands.rightObstacle = null;
        hands.UsingRight = false;
    }

    public void DropHat()
    {
        head.wornHat.DropHat();
    }

    public void ChangeState(PlayerState newState)
    {
		if (currentState == newState) return;

		switch (newState)
        {
            case PlayerState.WALKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = playerData.walkSpeed;
                break;
            case PlayerState.RUNNING:
                currentMoveSpeed = playerData.runSpeed;
                recoverStamina = false;
                break;
            case PlayerState.SNEAKING:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = playerData.sneakSpeed;
                break;
            case PlayerState.IDLE:
                if (!recoverStamina) RecoverStamina();
                currentMoveSpeed = playerData.walkSpeed;
                if(input.currentActionMap != null && input.currentActionMap.name != "Player") input.SwitchCurrentActionMap("Player");
                if(rb.bodyType != RigidbodyType2D.Dynamic) rb.bodyType = RigidbodyType2D.Dynamic;
                break;
            case PlayerState.DRIVING:
                if (!recoverStamina) RecoverStamina();
                rb.bodyType = RigidbodyType2D.Kinematic;
                input.SwitchCurrentActionMap("Vehicle");
                break;
        }

        currentState = newState;
    }

	void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus) Unhold();
	}

	void OnApplicationPause(bool isPaused)
	{
		if (isPaused) Unhold();
	}

	private void Unhold()
	{
        holdingSneak = false;
        holdingRun = false;
        holdingRight = false;
        holdingLeft = false;
	}
}


