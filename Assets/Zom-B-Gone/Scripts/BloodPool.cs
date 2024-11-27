using System.Collections;
using UnityEngine;

public class BloodPool : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] float scaleInDuration = 0.5f;
	[SerializeField] float fullOpacityLifetime = 5f;
	[SerializeField] float fadeOutDuration = 60f;

	public static int orderInLayer = 1;

	private void Awake()
	{
		spriteRenderer.sortingOrder = orderInLayer;
		orderInLayer++;
		transform.localScale = Vector3.zero;
		StartCoroutine(ScalingAndFading());
	}

	private IEnumerator ScalingAndFading()
	{
		// Scale in ---------------------------------------------------------------------
		Vector3 startScale = transform.localScale;
		Vector3 endScale = Vector3.one;
		float timeElapsed = 0f;

		while (timeElapsed < scaleInDuration)
		{
			timeElapsed += Time.deltaTime;
			transform.localScale = Vector3.Lerp(startScale, endScale, timeElapsed / scaleInDuration);
			yield return null;
		}
		transform.localScale = endScale;


		// Full opacity time -----------------------------------------------------------
		yield return new WaitForSeconds(fullOpacityLifetime);


		// Slowly fade out -------------------------------------------------------------
		Color color = spriteRenderer.color;
		float startAlpha = color.a;
		timeElapsed = 0f;

		while (timeElapsed < fadeOutDuration)
		{
			timeElapsed += Time.deltaTime;
			float newAlpha = Mathf.Lerp(startAlpha, 0f, timeElapsed / fadeOutDuration);
			color.a = newAlpha;
			spriteRenderer.color = color;
			yield return null;
		}
		color.a = 0f;
		spriteRenderer.color = color;


		// Destroy after fade out ------------------------------------------------------
		Destroy(gameObject);
	}
}
