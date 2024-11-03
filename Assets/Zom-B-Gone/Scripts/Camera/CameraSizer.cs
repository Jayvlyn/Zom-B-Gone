using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraSizer : MonoBehaviour
{
	public PixelPerfectCamera pixelPerfectCamera;
	public int baseZoomLevel = 1;
	public Vector2 referenceResolution = new Vector2(1920, 1080); // Set your desired reference resolution
	private Coroutine zoomCoroutine;
	public float zoomDuration = 1f;

	void Start()
	{
		SetZoomLevel(baseZoomLevel);
	}

	public void SetZoomLevel(int zoomLevel)
	{
		// Calculate the scaling factor based on the current screen size
		float scaleFactor = Mathf.Min(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y);
		pixelPerfectCamera.assetsPPU = Mathf.RoundToInt((16 * zoomLevel) * scaleFactor);
	}

	public void LerpZoomLevel(int targetZoomLevel)
	{
		if (zoomCoroutine != null)
		{
			StopCoroutine(zoomCoroutine);
		}
		zoomCoroutine = StartCoroutine(LerpZoomCoroutine(targetZoomLevel));
	}

	private IEnumerator LerpZoomCoroutine(int targetZoomLevel)
	{
		int startPPU = pixelPerfectCamera.assetsPPU;
		int targetPPU = Mathf.RoundToInt(16 * targetZoomLevel * GetScaleFactor());
		float elapsedTime = 0f;

		while (elapsedTime < zoomDuration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / zoomDuration;

			// Lerp between the starting and target assetsPPU
			pixelPerfectCamera.assetsPPU = Mathf.RoundToInt(Mathf.Lerp(startPPU, targetPPU, t));

			yield return null;
		}

		// Ensure the final value is set exactly to the target
		pixelPerfectCamera.assetsPPU = targetPPU;
		zoomCoroutine = null;
	}

	private float GetScaleFactor()
	{
		return Mathf.Min(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y);
	}
}
