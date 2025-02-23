using GameEvents;
using System.Collections;
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
		if (onClickDownRoutine != null) StopCoroutine(onClickDownRoutine);
		onClickDownRoutine = StartCoroutine(OnClickDownRoutine());
	}

    public void OnPointerUp(PointerEventData eventData)
    {
		if(allowClickUp)
		{
			ClickUp();
		}
		else
		{
			if (onClickUpRoutine != null) StopCoroutine(onClickUpRoutine);
			onClickUpRoutine = StartCoroutine(OnClickUpRoutine());
		}
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

	private bool allowClickUp = true;
	private Coroutine onClickDownRoutine;
	private IEnumerator OnClickDownRoutine()
	{
		allowClickUp = false;
		yield return new WaitForSecondsRealtime(0.15f);
		allowClickUp = true;

		onClickDownRoutine = null;
    }

    private Coroutine onClickUpRoutine;
    private IEnumerator OnClickUpRoutine()
    {
        yield return new WaitUntil(() => allowClickUp);
		ClickUp();
    }

    private void OnDisable()
    {
		if (onClickUpRoutine != null)
		{
            AudioManager.Instance.Play(CodeMonkey.Assets.i.buttonUp);
        }
    }
}

