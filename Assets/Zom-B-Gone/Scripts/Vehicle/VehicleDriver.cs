using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleDriver : MonoBehaviour
{
    [HideInInspector] public Vehicle vehicle;
    [SerializeField] private float enterExitSpeed = 1;

    private void OnAccelerate(InputValue inputValue)
    {
        vehicle.Accelerate();
    }

    private void OnBrake(InputValue inputValue)
    {
        vehicle.Brake();
    }

    private void OnSteer(InputValue inputValue)
    {
        float steerDirection = inputValue.Get<float>();
        vehicle.Steer(steerDirection);
    }

    private void OnExit(InputValue inputValue)
    {
        
    }

    public IEnumerator TransferPosition(Vector3 position, Quaternion rotation)
    {
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
}
