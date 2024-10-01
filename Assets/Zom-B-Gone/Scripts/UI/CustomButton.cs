using GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VoidEvent OnClickEvent;
    public RectTransform rect;
    public float buttonPressedOffset = -20;

    public void OnPointerDown(PointerEventData eventData)
    {

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, buttonPressedOffset);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
        OnClickEvent.Raise();
    }
}

