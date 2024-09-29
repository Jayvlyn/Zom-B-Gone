using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


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

    public static float MapWeightToRange(float scalarValue, float minValue, float maxValue, bool invert)
    {
        // Normalize the weight value between 0 and 1
        float maxWeight = 20000.0f;
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
    
    public static bool WallInFront(Transform t, float dist = 1f)
    {
        RaycastHit2D hit = Physics2D.Raycast(t.position, t.up, dist, LayerMask.GetMask("World","Vehicle"));
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

    public static void CreateExplosion(Vector2 sourcePosition, float radius, float force, int damage)
    {
        LayerMask lm = LayerMask.GetMask("Player", "Enemy", "Vehicle", "AirborneItem");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(sourcePosition, radius, lm);
        foreach (Collider2D collider in colliders)
        {
            if(collider.TryGetComponent(out Rigidbody2D rb))
            {
                float finalForce = force;
                if(collider.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
                {
                    Vehicle v = collider.gameObject.GetComponentInChildren<Vehicle>();
                    v.StartCoroutine(v.ExplodedTimer());
                    finalForce *= 1000;
                    Vector2 explosionToVehicle = (Vector2)collider.transform.position - sourcePosition;
                    Vector2 explosionForceDirection = explosionToVehicle.normalized;
                    float distance = explosionToVehicle.magnitude;

                    float torque = 0f;
                    float torqueAmount = 10000f;

                    float inverseDistance = 1f / distance;

                    // Calculate 2D torque (cross product approximation)
                    torque = (explosionToVehicle.x * explosionForceDirection.y - explosionToVehicle.y * explosionForceDirection.x)
                                   * finalForce;

                    // Scale torque by inverse distance
                    float scaledTorque = torque * inverseDistance;


                    //if (explosionToVehicle.x > 0 && explosionToVehicle.y > 0)
                    //{
                    //    torque = torqueAmount;
                    //}
                    //else if (explosionToVehicle.x < 0 && explosionToVehicle.y > 0)
                    //{
                    //    torque = -torqueAmount;
                    //}
                    //else if (explosionToVehicle.x < 0 && explosionToVehicle.y < 0)
                    //{
                    //    torque = torqueAmount;
                    //}
                    //else if (explosionToVehicle.x > 0 && explosionToVehicle.y < 0)
                    //{
                    //    torque = -torqueAmount;
                    //}
                    Debug.Log(scaledTorque);
                    rb.AddTorque(scaledTorque * 1000000, ForceMode2D.Impulse);
                }


                Vector2 dir = ((Vector2)collider.transform.position - sourcePosition).normalized;
                rb.AddForce(dir * finalForce);
            }

            if(collider.TryGetComponent(out Health h))
            {
                h.TakeDamage(damage, 30, 1, false);
            }
        }
    }

}
