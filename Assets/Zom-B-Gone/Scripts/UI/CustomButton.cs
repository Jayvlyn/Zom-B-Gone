using GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VoidEvent OnClickEvent;
    public RectTransform rect;

    public void OnPointerDown(PointerEventData eventData)
    {

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 9.36f);
        OnClickEvent.Raise();
    }
}

