using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour, IInteractable
{
    [SerializeField] protected float accelerationSpeed;
    [SerializeField] protected float brakeSpeed;
    [SerializeField] protected float steeringSpeed;
    [SerializeField] protected float maxTurnAngle = 45;
    [SerializeField] protected float maxSpeed = 100;
    public float exitDistance = 3;
    [SerializeField] protected float driftFactor = 0.6f;

    public Transform driveSeat;
    public Rigidbody2D rb;
    public SpriteRenderer litHeadlights;

    protected float currentTurnAngle = 0;

    protected bool active = false;
    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            litHeadlights.enabled = value;
        }
    }

    abstract public void Accelerate();

    abstract public void Steer(float steerDirection);

    abstract public void Brake();

    private void FixedUpdate()
    {
        KillOrthogonalVelocity();
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public virtual void CorrectSteering()
    {
        if (currentTurnAngle > 0)
        {
            currentTurnAngle -= Time.deltaTime * steeringSpeed;
            if (currentTurnAngle < 0) currentTurnAngle = 0;
        }
        else
        {
            currentTurnAngle += Time.deltaTime * steeringSpeed;
            if (currentTurnAngle > 0) currentTurnAngle = 0;
        }
    }

    public void Interact(Head head)
    {
        Active = true;
    }

    public void OnExit()
    {
        Active = false;
    }

    public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }
}
