using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEvent enterEvent;
    [SerializeField] private VoidEvent exitEvent;

    [SerializeField] protected float accelerationSpeed;
    [SerializeField] protected float brakeSpeed;
    [SerializeField] protected float steeringSpeed;
    [SerializeField] protected float maxTurnAngle = 45;
    [SerializeField] protected float maxSpeed = 100;
    public float exitDistance = 3;
    protected float currentTurnAngle = 0;
    //public int contactDamage = 50;

    [SerializeField] protected float baseDriftFactor = 0.3f;
    [SerializeField] protected float driftingDriftFactor = 0.99f;
    [SerializeField] protected float driftDrag = 0.1f;
    [SerializeField] protected float driveDrag = 1f;
    protected float driftFactor;
    public bool drift = false;
    public bool braking = false;

    public Transform driveSeat;
    public Rigidbody2D rb;
    public GameObject litHeadlights;

    public bool movedFromExplosion = false;

    protected bool active = false;
    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            litHeadlights.SetActive(value);
        }
    }

    private void Start()
    {
        rb.linearDamping = driveDrag;
    }

    protected void FixedUpdate()
    {
        if (Mathf.Abs(currentTurnAngle) > 30 && drift && !braking)
        {
            if (rb.linearDamping > driftDrag) rb.linearDamping -= 0.05f;
            if (rb.linearDamping < driftDrag) rb.linearDamping = driftDrag;
        }
        else
        {
            if (rb.linearDamping < driveDrag) rb.linearDamping += 0.05f;
            if (rb.linearDamping > driveDrag) rb.linearDamping = driveDrag;
        }
    }

    abstract public void Accelerate();

    abstract public void Steer(float steerDirection);

    abstract public void Brake();

    protected void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

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


        rb.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, rb.linearVelocity);
    }

    public float GetLongitudinalVelocity()
    {
        return Vector2.Dot(transform.up, rb.linearVelocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if(braking && GetLongitudinalVelocity() > 0)
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
		StartCoroutine(Activate(true));
		enterEvent.Raise();
    }

    public void OnExit()
    {
        Active = false;
        exitEvent.Raise();
    }

    public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ExplodedTimer()
    {
        movedFromExplosion = true;
        yield return new WaitForSeconds(2);
        movedFromExplosion = false;
    }

    public IEnumerator Activate(bool active)
    {
        yield return new WaitForSeconds(1f);
        Active = active;
    }


}
