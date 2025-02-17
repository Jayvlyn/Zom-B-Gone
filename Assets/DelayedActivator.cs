using System.Collections;
using UnityEngine;

public class DelayedActivator : MonoBehaviour
{
    public GameObject toActivate;

    private void Start()
    {
        StartCoroutine(DelayedActivate(10));   
    }

    private IEnumerator DelayedActivate(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        toActivate.SetActive(true);
    }
}
