using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWheeler : Vehicle
{
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;

    public override void Accelerate()
    {
        rb.AddForce(transform.up * accelerationSpeed);
        RotateToTurnAngle(false);
    }

    public override void Brake()
    {
        rb.AddForce(-transform.up * brakeSpeed);
        RotateToTurnAngle(true);
    }

    public override void Steer(float steerDirection)
    {
        if(Mathf.Abs(currentTurnAngle) <= maxTurnAngle)
        {
            currentTurnAngle += (Time.deltaTime * steeringSpeed) * steerDirection;

            if(currentTurnAngle < -maxTurnAngle) currentTurnAngle = -maxTurnAngle + 0.01f;
            else if(currentTurnAngle > maxTurnAngle) currentTurnAngle = maxTurnAngle - 0.01f;

            MatchWheelsToTurnAngle();
        }
    }

    public override void CorrectSteering()
    {
        base.CorrectSteering();
        MatchWheelsToTurnAngle();
    }

    private void MatchWheelsToTurnAngle()
    {
        frontLeftWheel.localRotation = Quaternion.Euler(0, 0, currentTurnAngle);
        frontRightWheel.localRotation = Quaternion.Euler(0, 0, currentTurnAngle);
    }

    private void RotateToTurnAngle(bool reverse)
    {
        if (reverse) rb.MoveRotation(rb.rotation + -currentTurnAngle * Time.deltaTime * 7);
        else rb.MoveRotation(rb.rotation + currentTurnAngle * Time.deltaTime * 7);
    }
}
