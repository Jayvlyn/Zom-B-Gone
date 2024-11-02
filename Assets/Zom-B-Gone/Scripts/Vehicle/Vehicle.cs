using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour, IInteractable
{
    public VehicleData vehicleData;
    public AudioSource tireScreechSource; // should loop
    public AudioSource engineSource; // should loop
    public AudioSource mainSource;

	protected float currentTurnAngle = 0;
	protected float driftFactor;

	[HideInInspector] public bool drift = false;
	[HideInInspector] public bool braking = false;

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
            if(active)
            {
                engineSource.resource = vehicleData.engineSounds[0];
                engineSource.Play();
            }
            else
            {
                engineSource.Stop();
            }

        }
    }

    private void Start()
    {
        rb.linearDamping = vehicleData.driveDrag;
        if(tireScreechSource && vehicleData.tireScreechSound) tireScreechSource.resource = vehicleData.tireScreechSound;
    }

    protected void FixedUpdate()
    {
        if (Mathf.Abs(currentTurnAngle) > 30 && drift && !braking)
        {
            if (rb.linearDamping > vehicleData.driftDrag) rb.linearDamping -= 0.05f;
            if (rb.linearDamping < vehicleData.driftDrag) rb.linearDamping = vehicleData.driftDrag;
        }
        else
        {
            if (rb.linearDamping < vehicleData.driveDrag) rb.linearDamping += 0.05f;
            if (rb.linearDamping > vehicleData.driveDrag) rb.linearDamping = vehicleData.driveDrag;
        }


        if(rb.linearVelocity.magnitude > vehicleData.maxSpeed * 0.9)
        {
            engineSource.resource = vehicleData.engineSounds[5];
        }
		else if (rb.linearVelocity.magnitude > vehicleData.maxSpeed * 0.7)
		{
			engineSource.resource = vehicleData.engineSounds[4];
		}
		else if (rb.linearVelocity.magnitude > vehicleData.maxSpeed * 0.5)
		{
			engineSource.resource = vehicleData.engineSounds[3];
		}
		else if (rb.linearVelocity.magnitude > vehicleData.maxSpeed * 0.3)
		{
			engineSource.resource = vehicleData.engineSounds[2];
		}
		else if (rb.linearVelocity.magnitude > vehicleData.maxSpeed * 0.1)
		{
			engineSource.resource = vehicleData.engineSounds[1];
		}
		else
		{
			engineSource.resource = vehicleData.engineSounds[0];
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
            if (driftFactor < vehicleData.driftingDriftFactor) driftFactor += 0.05f;
            if (driftFactor > vehicleData.driftingDriftFactor) driftFactor = vehicleData.driftingDriftFactor;
        }
        else
        {
            if (driftFactor > vehicleData.baseDriftFactor) driftFactor -= 0.005f;
            if (driftFactor < vehicleData.baseDriftFactor) driftFactor = vehicleData.baseDriftFactor;
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
			if (tireScreechSource) tireScreechSource.Play();
			isBraking = true;
            return true;
        }

        if(Mathf.Abs(GetLateralVelocity()) > 4.0f)
        {
            if(tireScreechSource) tireScreechSource.Play();
            return true;
        }
        else if (tireScreechSource.isPlaying)
		{
			if (tireScreechSource) tireScreechSource.Stop();
        }

        return false;
    }

    public virtual void CorrectSteering()
    {
        if (currentTurnAngle > 0)
        {
            currentTurnAngle -= Time.deltaTime * vehicleData.steeringSpeed;
            if (currentTurnAngle < 0) currentTurnAngle = 0;
        }
        else
        {
            currentTurnAngle += Time.deltaTime * vehicleData.steeringSpeed;
            if (currentTurnAngle > 0) currentTurnAngle = 0;
        }
    }

    public void Interact(Head head)
    {
		StartCoroutine(Activate(true));
		vehicleData.enterEvent.Raise();
    }

    public void OnExit()
    {
        Active = false;
		vehicleData.exitEvent.Raise();
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
        if(vehicleData.enterSound) mainSource.PlayOneShot(vehicleData.enterSound); 
        yield return new WaitForSeconds(1f);
        Active = active;
    }


}
