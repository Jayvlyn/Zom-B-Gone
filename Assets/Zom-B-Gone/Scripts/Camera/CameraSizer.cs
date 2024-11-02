using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraSizer : MonoBehaviour
{
	public PixelPerfectCamera pixelPerfectCamera;
	public int baseZoomLevel = 1;
	private Coroutine zoomCoroutine; 
	public float zoomDuration = 1f;

	void Start()
	{
		SetZoomLevel(baseZoomLevel);
	}

	public void SetZoomLevel(int zoomLevel)
	{
		pixelPerfectCamera.assetsPPU = 16 * zoomLevel;
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
		int targetPPU = 16 * targetZoomLevel;
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
}
