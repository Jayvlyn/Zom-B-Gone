using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float MapScalarToRange(float scalarValue, float minValue, float maxValue, bool invert)
    {
        float normalizedValue = scalarValue;
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
        float maxWeight = 200000.0f;
        float normalizedValue = (scalarValue / maxWeight); 
        if (invert) normalizedValue = 1 - normalizedValue;

        // Map the normalized value to the specified range
        float mappedResult = minValue + normalizedValue * (maxValue - minValue);

        // Ensure the result is within the specified range
        float result = Mathf.Max(minValue, Mathf.Min(maxValue, mappedResult));

        return result;
    }
}