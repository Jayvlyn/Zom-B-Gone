using GameEvents;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VoidEvent OnClickEvent;
    public RectTransform rect;
	public float buttonPressedOffset = -20;

    [SerializeField]
    public UnityEvent OnClick;

	public void OnPointerDown(PointerEventData eventData)
    {
        ClickDown();
    }

    public void ClickDown()
    {
		rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, buttonPressedOffset);
		AudioManager.Instance.Play(CodeMonkey.Assets.i.buttonDown);
	}

    public void OnPointerUp(PointerEventData eventData)
    {
		ClickUp();
	}

    public void ClickUp()
    {
		rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
		if (OnClickEvent) OnClickEvent.Raise();

		if (OnClick != null)
		{
			OnClick?.Invoke();
		}
		AudioManager.Instance.Play(CodeMonkey.Assets.i.buttonUp);
	}
}

