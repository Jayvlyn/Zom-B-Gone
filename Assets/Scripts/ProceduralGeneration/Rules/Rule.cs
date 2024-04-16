using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="ProceduralCity/Rule")]
public class Rule : ScriptableObject
{
    public string letter;
    [SerializeField] private string[] results = null;
    [SerializeField] private bool randomResult = false;

    public string GetResult()
    {
        if(!randomResult) return results[0];

        int randomIndex = Random.Range(0, results.Length);
        return results[randomIndex];
    }
}
