using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonWithSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public AudioClip overrideUpSound = null;
    public AudioClip overrideDownSound = null;
    public bool noUpSound = false;
    public bool noDownSound = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (overrideDownSound != null)
        {
            AudioManager.Instance.Play(overrideDownSound);
        }
        else if (!noDownSound)
        {
            AudioManager.Instance.Play(CodeMonkey.Assets.i.buttonDown);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (overrideUpSound != null)
        {
            AudioManager.Instance.Play(overrideUpSound);
        }
        else if (!noUpSound)
        {
            AudioManager.Instance.Play(CodeMonkey.Assets.i.buttonUp);
        }
    }
}
