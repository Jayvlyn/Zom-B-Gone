using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    public float minPitch = 0.8f;
    public float maxPitch = 1.6f;

    public float CurrentSpeed => rb.linearVelocity.magnitude;

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
                engineSource.Play();
                if (engineSoundCoroutine == null) engineSoundCoroutine = StartCoroutine(EngineSoundLoop());
            }
            else
            {
                engineSource.Stop();
                if(engineSoundCoroutine != null) StopCoroutine(engineSoundCoroutine);
                engineSoundCoroutine = null;
            }

        }
    }

    private void Start()
    {
        rb.linearDamping = vehicleData.driveDrag;
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
    }

    private void Update()
    {
        if(active)
        {
            float pitch = Mathf.Lerp(minPitch, maxPitch, CurrentSpeed / vehicleData.maxSpeed);
            engineSource.pitch = pitch;
        }

        if(IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            if(!tireScreechSource.isPlaying)
                StartScreechNoise();
        }
        else if (tireScreechSource.isPlaying)
        {

            StopScreechNoise();
        }
    }

    abstract public void Accelerate(float speedMod = 1);

    abstract public void Steer(float steerDirection);

    abstract public void Brake(float speedMod = 1);

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

    public bool IsMovingFoward()
    {
        return Vector2.Dot(transform.up, rb.linearVelocity.normalized) > 0;
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



        if(Mathf.Abs(lateralVelocity) > 4.0f)
        {
            return true;
        }

        return false;
    }

    private void StartScreechNoise()
    {
		if (tireScreechSource && !tireScreechSource.isPlaying) tireScreechSource.Play();
        if(tireScreechCoroutine == null)
        {
            tireScreechCoroutine = StartCoroutine(TireScreechLoop());
        }
	}

    private void StopScreechNoise()
    {
        if (tireScreechSource && tireScreechSource.isPlaying) tireScreechSource.Stop();
        if (tireScreechCoroutine != null)
        {
            StopCoroutine(tireScreechCoroutine);
            tireScreechCoroutine = null;
        }
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

    public void OnExit()
    {
        Active = false;
		vehicleData.exitEvent.Raise();
    }

    public void Interact(bool rightHand, PlayerController playerController)
    {
		StartCoroutine(Activate(true));
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
		vehicleData.enterEvent.Raise();

		Utils.MakeSoundWave(transform.position, 8);
    }

    private Coroutine engineSoundCoroutine;
    private IEnumerator EngineSoundLoop()
    {
        while(true)
        { 
            Utils.MakeSoundWave(transform.position, 22);
            yield return new WaitForSeconds(2.5f);
        }
    }

    private Coroutine tireScreechCoroutine;
    private IEnumerator TireScreechLoop()
    {
        while(true)
        {
			Utils.MakeSoundWave(transform.position, 33);

			yield return new WaitForSeconds(2.5f);
		}
    }

}
