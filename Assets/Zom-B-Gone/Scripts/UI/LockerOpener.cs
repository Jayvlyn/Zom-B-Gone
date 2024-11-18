using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerOpener : MonoBehaviour
{
	[SerializeField] GameObject worldCounterpart;
	[SerializeField] Interactor interactor;
	[SerializeField] RectTransform targetRect;
	[SerializeField] RectTransform heightRef;

	public void OnLockerOpened()
    {
		if (Interactor.interactedContainer != null)
			interactor.CloseOpenedContainer();
		Interactor.interactedContainer = worldCounterpart;
		
		StartCoroutine(PreSlideUp(-30, .1f));

		AudioManager.Instance.Play(CodeMonkey.Assets.i.openLocker);
	}

	public void OnLockerClosed()
	{

		Vector3 oldWorldPos = targetRect.position;
		Vector2 oldSizeDelta = targetRect.sizeDelta;
		float height = targetRect.rect.height;

		targetRect.anchorMax = new Vector2(targetRect.anchorMax.x, 0);

		targetRect.sizeDelta = new Vector2(oldSizeDelta.x, height);
		targetRect.position = oldWorldPos;
		StartCoroutine(PreSlideDown(30, .1f));
	}

	public IEnumerator SlideDown(float duration)
	{
		Vector3 startPosition = targetRect.position;
		Vector3 endPosition = new Vector3(startPosition.x, 0, startPosition.z);
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			targetRect.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		targetRect.position = endPosition;
	}

	public IEnumerator SlideUp(float duration)
	{
		targetRect.sizeDelta = new Vector2(targetRect.sizeDelta.x, heightRef.rect.height);
		float height = targetRect.rect.height;
		Vector2 startPosition = targetRect.anchoredPosition;
		Vector2 endPosition = new Vector2(startPosition.x, height + heightRef.anchorMin.y);
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			targetRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		targetRect.anchorMax = new Vector2(targetRect.anchorMax.x, 1);
		targetRect.sizeDelta = heightRef.sizeDelta;
		targetRect.position = heightRef.position;
	}


	public IEnumerator PreSlideDown(float amt, float duration)
	{
		Vector3 startPosition = targetRect.position;
		Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + amt, startPosition.z);
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			targetRect.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		targetRect.position = endPosition;

		StartCoroutine(SlideDown(.3f));
	}

	public IEnumerator PreSlideUp(float amt, float duration)
	{
		Vector3 startPosition = targetRect.position;
		Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + amt, startPosition.z);
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			targetRect.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		targetRect.position = endPosition;

		StartCoroutine(SlideUp(.3f));
	}
}
