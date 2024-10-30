using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWheeler : Vehicle
{
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;

    protected void FixedUpdate()
    {
        base.FixedUpdate();
        if(active) KillOrthogonalVelocity();
        if (rb.linearVelocity.magnitude > 0 && !movedFromExplosion)
        {
            float dot = Vector2.Dot(rb.linearVelocity, transform.up);

            if(dot > 0.1)
            {
                RotateToTurnAngle(false);
            }
            else if (dot < -0.1)
            {
                RotateToTurnAngle(true);
            }
        }

    }

    public override void Accelerate()
    {
        if (rb.linearVelocity.magnitude > maxSpeed) return;
        rb.AddForce(transform.up * accelerationSpeed);
        
    }

    public override void Brake()
    {
        rb.AddForce(-transform.up * brakeSpeed);
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
        if (reverse) rb.MoveRotation(rb.rotation + -currentTurnAngle * rb.linearVelocity.magnitude * 0.005f);
        else rb.MoveRotation(rb.rotation + currentTurnAngle * rb.linearVelocity.magnitude * 0.005f);
    }
}
