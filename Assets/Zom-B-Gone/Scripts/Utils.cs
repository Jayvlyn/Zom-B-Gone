using Cinemachine;
using CodeMonkey;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public static class Utils
{
    public static float MapScalarToRange(float scalarValue, float scalarMax, float minValue, float maxValue, bool invert)
    {
        float normalizedValue = scalarValue / scalarMax;
        if (invert) normalizedValue = 1 - normalizedValue;

        // Map the normalized value to the specified range
        float mappedResult = minValue + normalizedValue * (maxValue - minValue);

        // Ensure the result is within the specified range
        float result = Mathf.Max(minValue, Mathf.Min(maxValue, mappedResult));

        return result;
    }

    public static float MapWeightToRange(float scalarValue, float minValue, float maxValue, bool invert, bool obstacle = false)
    {
        // Normalize the weight value between 0 and 1
        float maxWeight = 20000f;
        if (obstacle) maxWeight = 75000f;
        float normalizedValue = (scalarValue / maxWeight); 
        if (invert) normalizedValue = 1 - normalizedValue;

        // Map the normalized value to the specified range
        float mappedResult = minValue + normalizedValue * (maxValue - minValue);

        // Ensure the result is within the specified range
        float result = Mathf.Max(minValue, Mathf.Min(maxValue, mappedResult));

        return result;
    }

    public static Vector2[] GetDirectionsInCircle2D(int num, float angle)
    {
        List<Vector2> result = new List<Vector2>();

        // if odd number, set first direction as up (0, 1)
        if (num % 2 == 1) result.Add(Vector2.up);

        // compute the angle between rays
        float angleOffset = (angle * 2) / num;

        // add the +/- directions around the circle
        for (int i = 1; i <= num / 2; i++)
        {
            float modifier = (i == 1 && num % 2 == 0) ? 0.65f : 1;

            result.Add(Quaternion.AngleAxis(+angleOffset * i * modifier, Vector3.forward) * Vector2.up);

            result.Add(Quaternion.AngleAxis(-angleOffset * i * modifier, Vector3.forward) * Vector2.up);
        }

        return result.ToArray();
    }


	public static float RandomBinomial()
	{
        return Random.Range(0f, 1f) - Random.Range(0f, 1f); 
	}

    public static Vector2 RandomUnitVector2()
    {
        float random = Random.Range(0f, 260f);
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }

    public static Vector3 RandomUnitVector3()
    {
        float random = Random.Range(0f, 260f);
        return new Vector3(Mathf.Cos(random), Mathf.Sin(random), 0);
    }

    private static readonly LayerMask wallInFrontLm = LayerMask.GetMask("World", "Door", "Window", "Vehicle");
    public static bool WallInFront(Transform t, float dist = 1f)
    {
        RaycastHit2D hit = Physics2D.Raycast(t.position, t.up, dist, wallInFrontLm);
        return hit.collider != null;
        //return false;
    }

    public static bool IsPositionInCameraBounds(Vector3 position)
    {
        // Convert the position to viewport coordinates
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(position);

        // Check if the position is within the camera's viewport
        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1 &&
               viewportPos.z >= 0; // z should be >= 0 to ensure the position is in front of the camera
    }

	private static readonly LayerMask explosionLm = LayerMask.GetMask("Player", "Enemy", "Vehicle", "AirborneItem", "GroundedItem", "Obstacle", "Interactable", "Window");
    private static readonly LayerMask coverLm = LayerMask.GetMask("World", "Vehicle", "Door");
	public static void CreateExplosion(Vector2 sourcePosition, float radius, float force, int baseDamage, bool big = true)
    {
        // Visual
        Transform explosionPrefab = Assets.i.explosion;
        Transform e = Object.Instantiate(explosionPrefab, sourcePosition, Quaternion.identity);
        Explosion ex = e.GetComponent<Explosion>();

        CinemachineImpulseSource cis = ex.cis;
        AudioSource audioSource = ex.audioSource;


        ScreenShakeProfile explosionSSP;
        if (big)
        {
            explosionSSP = Assets.i.bigExplosionSSP;
            audioSource.clip = Assets.i.bigExplosionSound;
        }
        else
        {
            explosionSSP = Assets.i.smallExplosionSSP;
            audioSource.clip = Assets.i.smallExplosionSound;
            e.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        audioSource.Play();
        

        Vector2 randomDirection = Random.insideUnitCircle.normalized;


        CameraShakeManager.instance.CameraShake(cis, explosionSSP, randomDirection);

        // Function
        Collider2D[] colliders = Physics2D.OverlapCircleAll(sourcePosition, radius, explosionLm);
        foreach (Collider2D collider in colliders)
        {
            float dist = Vector2.Distance(collider.transform.position, sourcePosition);

            if(collider.gameObject.CompareTag("Player"))
            {
                if (radius < dist * 4) continue;
            }

            Vector2 dir = ((Vector2)collider.transform.position - sourcePosition).normalized;
            RaycastHit2D coverHit = Physics2D.Raycast(sourcePosition, dir, dist, coverLm);

            bool isVehicle = collider.gameObject.layer == LayerMask.NameToLayer("Vehicle");


			if (!isVehicle && coverHit.collider != null) continue;

			if (collider.TryGetComponent(out Health h))
			{
				Vector2 knockbackVector = ((Vector2)h.transform.position - sourcePosition).normalized;
				knockbackVector *= force;
				Vector2 popupVector = ((Vector2)h.transform.position - sourcePosition).normalized * 3;
                float damage = baseDamage - (dist * 6);
                bool invert = (sourcePosition.x > h.transform.position.x);
				bool crit = false;
				if (Random.Range(0, 20) == 0) crit = true;
				h.TakeDamage(damage, knockbackVector, damage, crit, popupVector, invert, damage*0.33f);
			}
			else if (collider.TryGetComponent(out Rigidbody2D rb))
            {
                float finalForce = force;
                if(isVehicle)
                {
                    Vehicle v = collider.gameObject.GetComponentInChildren<Vehicle>();
                    v.StartCoroutine(v.ExplodedTimer());
                    finalForce *= 1000;
                    Vector2 explosionToVehicle = (Vector2)collider.transform.position - sourcePosition;
                    Vector2 explosionForceDirection = explosionToVehicle.normalized;
                    float distance = explosionToVehicle.magnitude;

                    float torque = 0f;
                    float torqueAmount = 1000000f;

                    float inverseDistance = 1f / distance;

                    // Calculate 2D torque (cross product approximation)
                    torque = (explosionToVehicle.x * explosionForceDirection.y - explosionToVehicle.y * explosionForceDirection.x)
                                   * finalForce;

                    // Scale torque by inverse distance
                    float scaledTorque = torque * inverseDistance;
                    rb.AddTorque(scaledTorque * torqueAmount, ForceMode2D.Impulse);

                    Debug.Log("torque added");
                }



                rb.AddForce(dir * finalForce);
            }
        }
    }

    private static readonly LayerMask soundDampenersLm = LayerMask.GetMask("World","Door","Window");
    private static readonly LayerMask enemyLm = LayerMask.GetMask("Enemy");
    public static void MakeSoundWave(Vector2 sourcePosition, float soundRadius, bool sneaking = false)
    {
        if (sneaking) soundRadius *= 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(sourcePosition, soundRadius, enemyLm);
        foreach (Collider2D hit in hits)
        {
            if(hit.gameObject.CompareTag("Enemy"))
            {
                Vector2 direction = ((Vector2)hit.gameObject.transform.position - sourcePosition).normalized;
                float distance = Vector2.Distance(hit.gameObject.transform.position, sourcePosition);
                RaycastHit2D[] hitsToEnemy = Physics2D.RaycastAll(sourcePosition, direction, distance, soundDampenersLm);

                float soundDistance = soundRadius;

                foreach (var soundHit in hitsToEnemy)
                {
                    soundDistance *= 0.75f;
                }

                if(distance <= soundDistance)
                {
                    if(hit.gameObject.TryGetComponent(out Enemy e))
                    { // GPT helped with random variation based on distance
                        // Define a maximum random offset factor (tweak this to get desired variation)
                        float maxOffsetFactor = 0.2f; // For example, 0.2 means up to 20% of the distance

                        // Calculate a random offset based on the distance
                        float randomOffsetX = Random.Range(-1f, 1f) * distance * maxOffsetFactor;
                        float randomOffsetY = Random.Range(-1f, 1f) * distance * maxOffsetFactor;
                        Vector2 randomOffset = new Vector2(randomOffsetX, randomOffsetY);

                        // Apply the offset to the source position
                        Vector2 variedSourcePosition = sourcePosition + randomOffset;

                        // Alert enemy with the varied source position
                        e.StartInvestigating(variedSourcePosition);
                    }
                }

            }
        }


    }

    public static void ClearPlayerTemporaryContainers()
    {
        ClearContainerSlots(Assets.i.handsData.container);
        ClearContainerSlots(Assets.i.headData.container);
        ClearContainerSlots(Assets.i.backpackData.container);
        ClearContainerSlots(Assets.i.vanFloorData.container);
        Assets.i.vanFloorData.ClearFloorSpecificVals();
    }

	public static void ClearContainerSlots(CollectibleContainer container)
	{
		for (int i = 0; i < container.collectibleSlots.Length; i++)
		{
			container.collectibleSlots[i].CollectibleName = null;
			container.collectibleSlots[i].quantity = 0;
		}
	}

    public static void ApplyEffect(EffectData effectData, Enemy enemy)
    {
		if (enemy.activeEffect != null)
		{
            if (enemy.activeEffect.effectData = effectData) return;
        
            // Overrides if a different effect is applied
            GameObject obj = enemy.activeEffect.gameObject;
            enemy.activeEffect = null;
            GameObject.Destroy(obj);
		}

		GameObject effectObject = Object.Instantiate(Assets.i.effectPrefab, enemy.transform);
        Effect e = effectObject.GetComponent<Effect>();
		e.Initialize(effectData, enemy);
	}

    public static CollectibleData GetCollectibleFromName(string name)
    {
        return Resources.Load<CollectibleData>(name);
	}

    // GPT generated
	public static int GetWeightedRandomNumber(int min, int max)
	{
		// Generate a random value in the range [min, max]
		float randomValue = Random.Range(0f, 1f);

		// Invert the random value to favor smaller numbers
		float scaledValue = 1f - Mathf.Sqrt(randomValue);

		// Scale to the desired range
		return Mathf.FloorToInt(scaledValue * (max - min + 1)) + min;
	}

}
