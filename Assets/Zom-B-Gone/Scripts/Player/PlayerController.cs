using Cinemachine;
using GameEvents;
using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    public PlayerInput input;
    public VehicleDriver vehicleDriver;
    public Collider2D playerCollider;
    public SpriteRenderer playerSprite;
    public PlayerRenderingChanger renderingChanger;
    public CinemachineImpulseSource impulseSource;
    public Health health;
    private Slider staminaSlider;
    private Interactor interactor;

    public static PlayerController instance;


    private Camera gameCamera;
    [HideInInspector] public CinemachineVirtualCamera vc;
    [HideInInspector] public Transform groupCam;
    [HideInInspector] public CinemachineFramingTransposer framingTransposer;
    [HideInInspector] public LookaheadChanger lookaheadChanger;

	public static bool holdingRun;
    public static bool holdingSneak;
    public static bool holdingLeft;
    public static bool holdingRight;
    public static bool hiding;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public bool recoverStamina;
    private float currentMoveSpeed;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;

    public static ContainerDragHandler mouseHeldIcon = null;

    private bool playerSwinging = false;
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        // previous state check
        switch (currentState)
        {
            case PlayerState.DRIVING: // exiting driving state
                renderingChanger.DoDefaultSorting();
                if (input.currentActionMap != null && input.currentActionMap.name != "Player") input.SwitchCurrentActionMap("Player");
                if (vc)
                {
                    framingTransposer.m_LookaheadTime = 0f;
                    if (orthoSizeChangeCoroutine != null) StopCoroutine(orthoSizeChangeCoroutine);
                    orthoSizeChangeCoroutine = StartCoroutine(SmoothOrthographicSizeChange(4f, 1f));
                }
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(true);
                rb.bodyType = RigidbodyType2D.Dynamic;
                break;

            case PlayerState.HIDING:
                hiding = false;
                renderingChanger.DoDefaultSorting();
                playerCollider.isTrigger = false;
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(true);
                rb.bodyType = RigidbodyType2D.Dynamic;
                gameObject.layer = LayerMask.NameToLayer("Player");
                break;
        }

        if (hands.rightItem is MeleeWeapon m && m.IsSwinging) playerSwinging = true;
        else if (hands.leftItem is MeleeWeapon m2 && m2.IsSwinging) playerSwinging = true;
        else playerSwinging = false;
        

        switch (newState)
        {
            case PlayerState.WALKING:
                if (!recoverStamina && !playerSwinging) RecoverStamina();
                currentMoveSpeed = playerData.walkSpeed;
                break;

            case PlayerState.RUNNING:
                currentMoveSpeed = playerData.runSpeed;
                recoverStamina = false;
                break;

            case PlayerState.SNEAKING:
                if (!recoverStamina && !playerSwinging) RecoverStamina();
                currentMoveSpeed = playerData.sneakSpeed;
                break;

            case PlayerState.IDLE:
                if (!recoverStamina && !playerSwinging) RecoverStamina();
                currentMoveSpeed = playerData.walkSpeed;
                break;

            case PlayerState.DRIVING:
				if (hands.RightObstacle != null) LetGoRightObstacle();
				if (hands.LeftObstacle != null) LetGoLeftObstacle();
				if (!recoverStamina && !playerSwinging) RecoverStamina();
                if (vc)
                {
                    vc.Follow = transform;
                    if (orthoSizeChangeCoroutine != null) StopCoroutine(orthoSizeChangeCoroutine);
                    orthoSizeChangeCoroutine = StartCoroutine(SmoothOrthographicSizeChange(7f, 1f));
                }
                renderingChanger.DoLowerSorting();
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(false);
                input.SwitchCurrentActionMap("Vehicle");
                rb.bodyType = RigidbodyType2D.Kinematic;
                break;

            case PlayerState.HIDING:
                if (hands.RightObstacle != null) LetGoRightObstacle();
                if (hands.LeftObstacle != null) LetGoLeftObstacle();
                hiding = true;
                playerCollider.isTrigger = true;
                renderingChanger.DoLowerSorting();
                if (head.wornHat && head.wornHat.activateOnWear) head.wornHat.activateOnWear.SetActive(false);
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector3.zero;
                gameObject.layer = LayerMask.NameToLayer("HidingPlayer");
                break;
        }

        currentState = newState;
    }

    private void Awake()
    {
        instance = this;
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
        ChangeState(PlayerState.IDLE);

		// Set currents
		currentStamina = playerData.maxStamina;
        currentMoveSpeed = playerData.walkSpeed;
    }

    private float recoverTimer;
    private void Update()
    {
        //Debug.Log(MusicManager.instance.AggroEnemies);
		UpdateStaminaBar();

        if (currentState == PlayerState.RUNNING) 
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                ChangeState(PlayerState.WALKING);
                
            }
        }
        else
        {
            if (recoverStamina)
            {
                //Debug.Log("recover stam tru");
                //Debug.Log(currentStamina);
                //Debug.Log(playerData.maxStamina);
                if (currentStamina < playerData.maxStamina)
                {
                    //Debug.Log("adding to stamina");
                    currentStamina += Time.deltaTime * playerData.staminaRecoverySpeed;
                    if (currentStamina > playerData.maxStamina) currentStamina = playerData.maxStamina;
                }
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
        if (currentState != PlayerState.HIDING) SetPlayerVelocity();
        if (currentState != PlayerState.DRIVING && currentState != PlayerState.HIDING && !dead) RotateToMouse();
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
        rb.linearVelocity = newVelocity;
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

        if(currentState != PlayerState.SNEAKING && currentState != PlayerState.HIDING)
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
            if (currentState != PlayerState.HIDING)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                if (!hands.UsingLeft) interactor.Interact(false);

                else if (hands.leftItem != null) hands.leftItem.Use();
            }
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

    private void OnRightHand(InputValue inputValue)
    {
		if (inputValue.isPressed)
		{
            if (currentState != PlayerState.HIDING)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                if (!hands.UsingRight) interactor.Interact(true);

                else if (hands.rightItem != null) hands.rightItem.Use();
            }
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

    private void OnLeftHold(InputValue inputValue)
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

    private void OnRightHold(InputValue inputValue)
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

    private void OnThrowLeft(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
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
        else // hiding, leave hiding spot
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up * 2, transform.rotation));
        }
	}

	private void OnThrowRight(InputValue inputValue)
	{
        if (currentState != PlayerState.HIDING)
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
        else // hiding, leave hiding spot
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up * 2, transform.rotation));
        }
	}

    private void OnDropLeft(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
        {
            if (hands.leftItem != null) DropLeft();
            else if (hands.LeftObstacle != null) LetGoLeftObstacle();
        }
        else // hiding, leave hiding spot
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up * 2, transform.rotation));
        }
    }

    private void OnDropRight(InputValue inputValue)
    {
        if (currentState != PlayerState.HIDING)
        {
            if (hands.rightItem != null) DropRight();
            else if (hands.RightObstacle != null) LetGoRightObstacle();
        }
        else // hiding, leave hiding spot
        {
            StartCoroutine(ExitHidingSpot(transform.position + transform.up * 2, transform.rotation));
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

            if (currentState != PlayerState.HIDING)
            {
                if (rb.linearVelocity.magnitude > 0 && movementInput != Vector2.zero && currentStamina > 0)
                {
                    ChangeState(PlayerState.RUNNING);
                }
            }
        }

        else // Run Key Released
        {
            holdingRun = false;

            if (currentState != PlayerState.HIDING)
            {
                if (currentState == PlayerState.RUNNING) // Letting go of run key only matters if running
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
        bool sneakPressed = Convert.ToBoolean(inputValue.Get<float>());

        if (sneakPressed) // Sneak Key Pressed
        {
            holdingSneak = true;
            if (currentState != PlayerState.HIDING)
            {
                ChangeState(PlayerState.SNEAKING);
            }
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

    private void OnGodMode(InputValue inputValue)
    {
        if(inputValue.isPressed)
        {
            godMode = !godMode;
        }
    }

	#endregion -----------------------------------------

	public void DropLeft()
	{
		if (hands.leftItem)
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
		if (hands.rightItem)
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
    private float enterExitHidingSpeed = 0.3f;
    public IEnumerator EnterHidingSpot(Vector3 position, Quaternion rotation)
    {
        ChangeState(PlayerState.HIDING);
        transform.parent = currentHidingSpot.transform.parent;
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
        Unhold();
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

        if (hands.LeftObject) hands.LeftObject.SetActive(true);
        if (hands.RightObject) hands.RightObject.SetActive(true);

        ChangeState(PlayerState.IDLE);
    }

    bool dead = false;
    public static bool godMode = false;
    public void Die()
    {
        DropLeft();
        DropRight();
        dead = true;
		input.SwitchCurrentActionMap("Vehicle");
		rb.bodyType = RigidbodyType2D.Kinematic;
        Utils.ClearPlayerTemporaryContainers();
        StartCoroutine(DeathTimer());
        
	}

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(2);
        GameManager.Instance.circleAnimator.SetTrigger("CloseCircle");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Unit");
    }
}


