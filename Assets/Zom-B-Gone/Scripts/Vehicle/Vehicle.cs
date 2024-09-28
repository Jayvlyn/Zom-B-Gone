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
    protected float currentTurnAngle = 0;

    [SerializeField] protected float baseDriftFactor = 0.3f;
    [SerializeField] protected float driftingDriftFactor = 0.99f;
    [SerializeField] protected float driftDrag = 0.1f;
    [SerializeField] protected float driveDrag = 1f;
    protected float driftFactor;
    public bool drift = false;
    public bool braking = false;

    public Transform driveSeat;
    public Rigidbody2D rb;
    public SpriteRenderer litHeadlights;



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

    private void Start()
    {
        rb.drag = driveDrag;
    }

    protected void FixedUpdate()
    {
        if (Mathf.Abs(currentTurnAngle) > 30 && drift && !braking)
        {
            if (rb.drag > driftDrag) rb.drag -= 0.05f;
            if (rb.drag < driftDrag) rb.drag = driftDrag;
        }
        else
        {
            if (rb.drag < driveDrag) rb.drag += 0.05f;
            if (rb.drag > driveDrag) rb.drag = driveDrag;
        }
    }

    abstract public void Accelerate();

    abstract public void Steer(float steerDirection);

    abstract public void Brake();

    protected void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        //if (drift) driftFactor = driftingDriftFactor;
        //else driftFactor = baseDriftFactor;

        if (drift && !braking)
        {
            if (driftFactor < driftingDriftFactor) driftFactor += 0.05f;
            if (driftFactor > driftingDriftFactor) driftFactor = driftingDriftFactor;
        }
        else
        {
            if (driftFactor > baseDriftFactor) driftFactor -= 0.005f;
            if (driftFactor < baseDriftFactor) driftFactor = baseDriftFactor;
        }


        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, rb.velocity);
    }

    float GetLogitudinalVelocity()
    {
        return Vector2.Dot(transform.up, rb.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if(braking && GetLogitudinalVelocity() > 0)
        {
            isBraking = true;
            return true;
        }

        if(Mathf.Abs(GetLateralVelocity()) > 4.0f)
        {
            return true;
        }

        return false;
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
