using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleDriver : MonoBehaviour
{
    [HideInInspector] public Vehicle vehicle;
    [SerializeField] private float enterExitSpeed = 1;

    private Collider2D playerCollider;
    private PlayerController playerController;

    private float steerDirection;
    private void Update()
    {
        if(vehicle && vehicle.Active)
        {
            if (steering) vehicle.Steer(steerDirection);
            if (accelerateHeld) vehicle.Accelerate();
            if (brakeHeld) vehicle.Brake();
            //if((accelerateHeld || brakeHeld) && !steering) vehicle.CorrectSteering();
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

    public void Enter(Collider2D playerCollider, PlayerController playerController)
    {
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
}