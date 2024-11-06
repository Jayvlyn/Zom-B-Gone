using Cinemachine;
using GameEvents;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        IDLE, WALKING, RUNNING, SNEAKING, DRIVING, HIDING
    }
    public static PlayerState currentState;
    public static bool isSneaking => currentState == PlayerState.SNEAKING;

	[Header("References")]
    public PlayerData playerData;
    public Hands hands;
    public Head head;
    public Rigidbody2D rb;
    public Collider2D bodyCollider;
    public PlayerInput input;
    private Slider staminaSlider;
    private Interactor interactor;

    [Header("Camera Refs")]
    private Camera gameCamera;
    public CinemachineVirtualCamera vc;
    public Transform groupCam;
	public CinemachineFramingTransposer framingTransposer;
    public LookaheadChanger lookaheadChanger;

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

    //public CameraSizer camSizer;

    public static ContainerDragHandler mouseHeldIcon = null;

    private void Awake()
    {
        // Find references
        gameCamera = FindFirstObjectByType<Camera>();
        interactor = GetComponent<Interactor>();
        rb.freezeRotation = true;
        staminaSlider = GameObject.FindGameObjectWithTag("Stamina").GetComponent<Slider>();
        if (vc != null)
        {
            framingTransposer = vc.GetCinemachineComponent<CinemachineFramingTransposer>();
			vc.Follow = groupCam;
			framingTransposer.m_LookaheadTime = 0f;
		}

		// Set currents
		currentStamina = playerData.maxStamina;
        currentMoveSpeed = playerData.walkSpeed;

        ChangeState(PlayerState.IDLE);
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
        if (currentState != PlayerState.HIDING)
        {
            smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, playerData.velocityChangeSpeed * Time.deltaTime * 100);
            Vector3 newVelocity = (smoothedMovementInput) * currentMoveSpeed * playerData.speedModifier * hands.leftLumbering * hands.rightLumbering;
            #region hat buff
            if(head.wornHat != null)
            {
                newVelocity *= head.wornHat.hatData.moveSpeedMod;
            }
            #endregion
            rb.linearVelocity = newVelocity;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
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
        if (currentState != PlayerState.HIDING)
        {
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
    }

    private void OnLeftHand(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
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
                else if (hands.LeftObstacle != null)
                {
                    hands.LeftObstacle.ChangeMotorSpeed();
                }
            }
        }
    }

    private void OnRightHand(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
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
                else if (hands.RightObstacle != null)
                {
                    hands.RightObstacle.ChangeMotorSpeed();
                }
            }

        }
	}

    private void OnLeftHold(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
        {
		    holdingLeft = true;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (hands.leftItem != null) hands.leftItem.useHeld = true;
            else if (hands.LeftObstacle != null)
            {
                if (holdingSneak) hands.LeftObstacle.ChangeMotorSpeed(-playerData.obstacleTurningSpeed);
                else hands.LeftObstacle.ChangeMotorSpeed(playerData.obstacleTurningSpeed);
            }
        }

    }

    private void OnRightHold(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
        {
            holdingRight = true;
		    if (EventSystem.current.IsPointerOverGameObject()) return;
            if (hands.rightItem != null) hands.rightItem.useHeld = true;
            else if (hands.RightObstacle != null)
            {
                if (holdingSneak) hands.RightObstacle.ChangeMotorSpeed(-playerData.obstacleTurningSpeed);
                else hands.RightObstacle.ChangeMotorSpeed(playerData.obstacleTurningSpeed);
            }

        }
    }

    private void OnThrowLeft(InputValue inputValue)
    {
        if (currentState == PlayerState.HIDING)
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up, transform.rotation));
        }
        else
        {
            if (hands.UsingLeft)
            {
                if(hands.leftItem != null)
                {
                    hands.leftItem.Throw();
                    hands.leftLumbering = 1;
                }
                else if(hands.LeftObstacle != null)
                {
                    LetGoLeftObstacle();
                }
		    }
        }
	}

	private void OnThrowRight(InputValue inputValue)
	{
        if(currentState == PlayerState.HIDING)
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up, transform.rotation));
        }
        else
        {
		    if (hands.UsingRight)
            {
                if (hands.rightItem != null)
                {
                    hands.rightItem.Throw();
                    hands.rightLumbering = 1;
                }
                else if (hands.RightObstacle != null)
                {
                    LetGoRightObstacle();
                }
            }
        }
	}

    private void OnDropLeft(InputValue inputValue)
    {
        if (currentState == PlayerState.HIDING)
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up, transform.rotation));
        }
        else
        {
            if (hands.leftItem != null) DropLeft();
            else if (hands.LeftObstacle != null) LetGoLeftObstacle();
        }
    }

    private void OnDropRight(InputValue inputValue)
    {
        if (currentState == PlayerState.HIDING)
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up, transform.rotation));
        }
        else
        {
            if (hands.rightItem != null) DropRight();
            else if (hands.RightObstacle != null) LetGoRightObstacle();
        }
    }

    // Input for reloading, wont do anything without projectileWeapon
    private void OnReload(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
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
	}

    private void OnRun(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
        {
            if (inputValue.isPressed) // Run Key Pressed
            {
                holdingRun = true;

                if(rb.linearVelocity.magnitude > 0 && movementInput != Vector2.zero && currentStamina > 0)
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
    }

    // Called when sneak button changes input value
    private void OnSneak(InputValue inputValue)
    {
        if(currentState != PlayerState.HIDING)
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
    }

    [SerializeField] protected VoidEvent onBackpackToggle = null;
    private void OnToggleBackpack(InputValue inputValue)
    {
        onBackpackToggle.Raise();
    }

    private void OnLookahead(InputValue inputValue)
    {
        if(lookaheadChanger)
        {
		    if (inputValue.isPressed)
		    {
			    lookaheadChanger.ActivateLookahead();
		    }
		    else
		    {
                lookaheadChanger.DeactivateLookahead();
		    }
        }
	}

	#endregion -----------------------------------------

	public void DropLeft()
	{
		if (hands.UsingLeft)
		{
			hands.leftItem.Drop();
			hands.leftLumbering = 1;
		}
        else if(hands.LeftObstacle != null)
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
        else if (hands.RightObstacle != null)
        {
            LetGoRightObstacle();
        }
    }

    public void LetGoLeftObstacle()
    {
        if (hands.LeftObstacle && hands.RightObstacle && hands.LeftObstacle == hands.RightObstacle)
        {
            hands.LeftObstacle.OnOneHandOn();
        }
        else
        {
            hands.LeftObstacle.BeFreed();
        }
        hands.LeftObstacle = null;
        hands.UsingLeft = false;
    }

    public void LetGoRightObstacle()
    {
        if (hands.RightObstacle && hands.LeftObstacle && hands.RightObstacle == hands.LeftObstacle)
        {
            hands.RightObstacle.OnOneHandOn();
        }
        else
        {
            hands.RightObstacle.BeFreed();
        }
        hands.RightObstacle = null;
        hands.UsingRight = false;
    }

    public void DropHat()
    {
        head.wornHat.DropHat();
    }

    public void ChangeState(PlayerState newState)
    {
		if (currentState == newState) return;

        // previous state check
        switch(currentState)
        {
            case PlayerState.DRIVING: // exiting driving state
				if (input.currentActionMap != null && input.currentActionMap.name != "Player") input.SwitchCurrentActionMap("Player");
                if(vc)
                {
				    framingTransposer.m_LookaheadTime = 0f;
                    if (orthoSizeChangeCoroutine != null) StopCoroutine(orthoSizeChangeCoroutine);
                    orthoSizeChangeCoroutine = StartCoroutine(SmoothOrthographicSizeChange(4f, 1f));
                }
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(true);
                rb.bodyType = RigidbodyType2D.Dynamic;
                break;
            case PlayerState.HIDING: // exiting hiding state
                rb.bodyType = RigidbodyType2D.Dynamic;
                gameObject.layer = LayerMask.NameToLayer("Player");
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(true);
                break;

        }

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
                break;
            case PlayerState.DRIVING:
                if (!recoverStamina) RecoverStamina();
                if(vc)
                {
                    vc.Follow = transform;
                    if(orthoSizeChangeCoroutine != null) StopCoroutine(orthoSizeChangeCoroutine); 
					orthoSizeChangeCoroutine = StartCoroutine(SmoothOrthographicSizeChange(7f, 1f));
				}
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(false);
                input.SwitchCurrentActionMap("Vehicle");
                rb.bodyType = RigidbodyType2D.Kinematic;
                break;
            case PlayerState.HIDING:
                if (!recoverStamina) RecoverStamina();
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(false);
                rb.bodyType = RigidbodyType2D.Kinematic;
                gameObject.layer = LayerMask.NameToLayer("HidingPlayer");
                break;
        }

        currentState = newState;
    }

    private Coroutine orthoSizeChangeCoroutine;
	private IEnumerator SmoothOrthographicSizeChange(float targetSize, float duration)
	{
		float startSize = vc.m_Lens.OrthographicSize;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			vc.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}

		vc.m_Lens.OrthographicSize = targetSize;
        if (startSize > targetSize)
        {
            vc.Follow = groupCam;
        }
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

    [HideInInspector] public HidingSpot currentHidingSpot = null;
    public float enterExitHidingSpeed = 1f;
    public IEnumerator EnterHidingSpot(Vector3 position, Quaternion rotation)
    {
        ChangeState(PlayerState.HIDING);
        transform.parent = currentHidingSpot.transform.parent;
        bodyCollider.isTrigger = true;
        if (hands.LeftObject) hands.LeftObject.SetActive(false);
        if (hands.RightObject) hands.RightObject.SetActive(false);

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < enterExitHidingSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, position, elapsedTime / enterExitHidingSpeed);
            transform.localRotation = Quaternion.Lerp(startRotation, rotation, elapsedTime / enterExitHidingSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
        transform.localRotation = rotation;
    }

    public IEnumerator ExitHidingSpot(Vector3 position, Quaternion rotation)
    {
        transform.parent = null;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < enterExitHidingSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, position, elapsedTime / enterExitHidingSpeed);
            transform.localRotation = Quaternion.Lerp(startRotation, rotation, elapsedTime / enterExitHidingSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
        transform.localRotation = rotation;

        bodyCollider.isTrigger = false;
        if (hands.LeftObject) hands.LeftObject.SetActive(true);
        if (hands.RightObject) hands.RightObject.SetActive(true);

        ChangeState(PlayerState.IDLE);
    }
}


