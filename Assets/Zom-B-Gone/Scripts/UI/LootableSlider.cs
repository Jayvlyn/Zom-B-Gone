using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableSlider : MonoBehaviour
{
    [SerializeField] RectTransform targetRect;

    [HideInInspector] public static bool windowOpened = false;

    public void OnLootableOpened()
    {
        if(windowOpened)
        {
            StartCoroutine(slideOutThenIn());
        }
        else
        {
            StartCoroutine(slideIn(0.1f));
            windowOpened = true;
        }
    }

    public void OnLootableClosed()
    {
        StartCoroutine(slideOut(0.1f));
        windowOpened = false;
    }

    private IEnumerator slideOutThenIn()
    {
        StartCoroutine(slideOut(0.05f));
        yield return new WaitForSeconds(0.06f);
        StartCoroutine(slideIn(0.05f));
    }

    private IEnumerator slideOut(float duration)
    {
        Vector2 startPosition = targetRect.anchoredPosition;
        Vector2 endPosition = new Vector2(-165, startPosition.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetRect.anchoredPosition = endPosition;
    }

    private IEnumerator slideIn(float duration)
    {
        Vector2 startPosition = targetRect.anchoredPosition;
        Vector2 endPosition = new Vector2(0, startPosition.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetRect.anchoredPosition = endPosition;
    }
}
