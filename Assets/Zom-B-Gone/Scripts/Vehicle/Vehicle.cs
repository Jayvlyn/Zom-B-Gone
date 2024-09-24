using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour, IInteractable
{
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float brakeSpeed;
    [SerializeField] private float steeringSpeed;
    [SerializeField] private float maxTurnAngle = 45;


    public Transform driveSeat;

    private float currentTurnAngle = 0;

    private bool active = false;

    public void Accelerate()
    {

    }

    public void Steer(float steerDirection)
    {

    }

    public void Brake()
    {

    }

    public void Interact(Head head)
    {
        active = true;
    }

    public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }
}
