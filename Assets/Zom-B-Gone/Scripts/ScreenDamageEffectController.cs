using System.Collections;
using UnityEngine;

public class ScreenDamageEffectController : MonoBehaviour
{
    public Material screenDamageMat;
    public float speed = 5;
    private Coroutine screenDamageTask;

    private static ScreenDamageEffectController instance;

	//private void Update()
	//{
	//       if (Input.GetMouseButtonDown(0))
	//       {
	//           ScreenDamageEffect(Random.Range(0.1f, 1));
	//       }
	//}

	private void Awake()
	{
		instance = this;
        ResetRadius();
	}

	private void ScreenDamageEffect(float intensity)
    {
        if (screenDamageTask != null)
        {
            StopCoroutine(screenDamageTask);
        }

        screenDamageTask = StartCoroutine(screenDamage(intensity));
    }

    private IEnumerator screenDamage(float intensity)
    {
        float targetRadius = Remap(intensity, 0, 1, 0.5f, 0.22f);
        float curRadius = 1;
        for (float t = 0; curRadius != targetRadius; t += Time.deltaTime * speed * 4)
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_Vignette_radius", curRadius);
            yield return null;
        }

		for (float t = 0; curRadius < 1; t += Time.deltaTime * speed)
		{
			curRadius = Mathf.Lerp(targetRadius, 1, t);
			screenDamageMat.SetFloat("_Vignette_radius", curRadius);
			yield return null;
		}
	}

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }

	private void OnDisable()
	{
        ResetRadius();
	}


    private void ResetRadius()
    {
		screenDamageMat.SetFloat("_Vignette_radius", 1);
	}

	public static class DamageEffect
    {
	    public static void DoDamageEffect(float intensity) => instance.ScreenDamageEffect(intensity);
    }
}



