using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using GameEvents;
using UnityEngine.SceneManagement;

public class VehicleDriver : MonoBehaviour
{
    [HideInInspector] public Vehicle vehicle;
    [SerializeField] private float enterExitSpeed = 1;
    [SerializeField] private VoidEvent onTravel;

    private Collider2D playerCollider;
    private PlayerController playerController;

    private float steerDirection;
    private void FixedUpdate()
    {
        if(vehicle && vehicle.Active)
        {
            if (TravelHeld && vehicle.transform.parent.name == "Van")
            {
                pressTimer += Time.deltaTime;
                if (pressTimer >= pressTimeRequired) onTravel.Raise();
                travelPercent = pressTimer / pressTimeRequired;
            }

            if (steering) vehicle.Steer(steerDirection);
            if (accelerateHeld) vehicle.Accelerate();
            if (brakeHeld) vehicle.Brake();
            if((accelerateHeld || brakeHeld) && !steering) vehicle.CorrectSteering();

            //Debug.Log(vehicle.GetLongitudinalVelocity());

        }
    }

	private void Update()
	{
		if (vehicle && vehicle.Active)
		{
			if (playerController.vc)
			{
				float targetLookaheadTime = 1.3f - Mathf.Clamp(Mathf.Abs(vehicle.GetLateralVelocity()), 0, 1.3f);
                Debug.Log(targetLookaheadTime);
				playerController.framingTransposer.m_LookaheadTime = Mathf.Lerp(playerController.framingTransposer.m_LookaheadTime, targetLookaheadTime, Time.deltaTime * 5f);
			}
		}
	}

	private bool accelerateHeld = false;
    private void OnAccelerate(InputValue inputValue)
    {
        if (inputValue.isPressed) accelerateHeld = true;
        else accelerateHeld = false;

    }

    private bool brakeHeld = false;
    private void OnBrake(InputValue inputValue)
    {
        if (inputValue.isPressed) brakeHeld = true;
        else brakeHeld = false;

        if (vehicle) vehicle.braking = brakeHeld;
    }

    private bool steering = false;
    private void OnSteer(InputValue inputValue)
    {
        if (vehicle && vehicle.Active)
        {
            steerDirection = inputValue.Get<float>();
            steering = steerDirection != 0;
        }
    }

    private bool driftHeld = false;
    private void OnDrift(InputValue inputValue)
    {
        if (inputValue.isPressed) driftHeld = true;
        else driftHeld = false;

        if (vehicle) vehicle.drift = driftHeld;
    }


    private void OnExit(InputValue inputValue)
    {
        vehicle.OnExit();

        Vector2 localExitPosition = new Vector2(-vehicle.exitDistance, 0);
        Vector2 exitPos = (Vector2)vehicle.driveSeat.position + (Vector2)(vehicle.transform.rotation * localExitPosition);

        //Vector2 exitPos = new Vector2 (vehicle.driveSeat.position.x - vehicle.exitDistance, vehicle.driveSeat.position.y);
        Quaternion exitRot = vehicle.transform.rotation * Quaternion.Euler(0, 0, 90);
        StartCoroutine(ExitVehicle(exitPos, exitRot));

        vehicle = null;
    }

    private bool travelHeld = false;
    private bool TravelHeld
    {
        get { return travelHeld; }
        set { travelHeld = value;
            if (!value) travelPercent = 0;
        }
    }
    
    private float pressTimer = 0;
    private float pressTimeRequired;
    private float extractTime = 3;
    private float startRunTime = 1;
    [HideInInspector] public static float travelPercent;
	private void OnTravel(InputValue inputValue)
    {
        if (SceneManager.GetActiveScene().name == "Unit") pressTimeRequired = startRunTime;
        else pressTimeRequired = extractTime;

        pressTimer = 0;
        if (inputValue.isPressed) TravelHeld = true;
        else TravelHeld = false;
    }

    public void Enter(Collider2D playerCollider, PlayerController playerController)
    {
        travelPercent = pressTimer / pressTimeRequired;
        this.playerCollider = playerCollider;
        this.playerController = playerController;

        StartCoroutine(EnterVehicle(vehicle.driveSeat.position, vehicle.transform.rotation));
    }

    public IEnumerator EnterVehicle(Vector3 position, Quaternion rotation)
    {
        playerController.ChangeState(PlayerController.PlayerState.DRIVING);
        transform.parent = vehicle.transform.parent;
        playerCollider.isTrigger = true;
        if (playerController.hands.LeftObject) playerController.hands.LeftObject.SetActive(false);
        if (playerController.hands.RightObject) playerController.hands.RightObject.SetActive(false);

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < enterExitSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, position, elapsedTime / enterExitSpeed);
            transform.localRotation = Quaternion.Lerp(startRotation, rotation, elapsedTime / enterExitSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
        transform.localRotation = rotation;
    }

    public IEnumerator ExitVehicle(Vector3 position, Quaternion rotation)
    {
        Unhold();
        transform.parent = null;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < enterExitSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, position, elapsedTime / enterExitSpeed);
            transform.localRotation = Quaternion.Lerp(startRotation, rotation, elapsedTime / enterExitSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
        transform.localRotation = rotation;


        playerCollider.isTrigger = false;
        if (playerController.hands.LeftObject) playerController.hands.LeftObject.SetActive(true);
        if (playerController.hands.RightObject) playerController.hands.RightObject.SetActive(true);

        playerController.ChangeState(PlayerController.PlayerState.IDLE);
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
		accelerateHeld = false;
		brakeHeld = false;
		driftHeld = false;
		TravelHeld = false;
        if (vehicle) vehicle.drift = driftHeld;
    }
}
