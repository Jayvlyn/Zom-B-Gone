using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackOpener : MonoBehaviour
{
    [SerializeField] RectTransform targetRect;
    [SerializeField] RectTransform heightRef;

    private bool backpackOpened = true; // set to false when done, should start closed

    public void OnBackpackToggled()
    {
        if(backpackOpened)
        { // SLIDE CLOSED
            backpackOpened = false;

            // preserve transform values to recreate adjustment of anchor preset
            Vector3 oldWorldPos = targetRect.position;
            Vector2 oldSizeDelta = targetRect.sizeDelta;
            float height = targetRect.rect.height;

            // change anchor, this goes from stretch right to anchor bottom right
            targetRect.anchorMax = new Vector2(targetRect.anchorMax.x, 0);

            // apply preserved transforms to persist position and height
            targetRect.sizeDelta = new Vector2(oldSizeDelta.x, height);
            targetRect.position = oldWorldPos;

            // lerp Pos Y to 0
            StartCoroutine(SlideDown(.5f));
        }
        else
        { // SLIDE OPEN
            backpackOpened = true;

            // lerp Pos Y to height + 40
            StartCoroutine(SlideUp(.5f));

        }
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

        // Ensure the final position is set exactly to avoid any rounding issues
        targetRect.position = endPosition;
    }

    public IEnumerator SlideUp(float duration)
    {
        targetRect.sizeDelta = new Vector2(targetRect.sizeDelta.x, heightRef.rect.height);
        float height = targetRect.rect.height;
        Vector3 startPosition = targetRect.position;
        Vector3 endPosition = new Vector3(startPosition.x, height * .7f, startPosition.z);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetRect.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration); // lerps to value different from height for some reason
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetRect.position = endPosition; // ensure no rounding issue


        // set anchor max y back to 1
        targetRect.anchorMax = new Vector2(targetRect.anchorMax.x, 1);

        targetRect.sizeDelta = heightRef.sizeDelta;
        targetRect.position = heightRef.position;
    }

    //public IEnumerator SlideDownBad()
    //{
    //    Vector3 newPosition = targetRect.position;
    //    newPosition.y = 0; // Example new Y position

    //    while (targetRect.position.y > 0)
    //    {
    //        targetRect.position = Vector3.Lerp(targetRect.position, newPosition, 1 * Time.deltaTime);
    //        yield return null;
    //    }
    //}
}
