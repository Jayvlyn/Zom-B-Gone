using UnityEngine;
using UnityEngine.UI;

public class LerpingUnderBar : MonoBehaviour
{
    public Slider overSlider;
    public Slider thisUnderSlider;
    public float lerpSpeed;

    void Update()
    {
        if(thisUnderSlider.value != overSlider.value)
        {
            if(thisUnderSlider.value > overSlider.value)
            { // under value is greater, lerp to over sliders value
                thisUnderSlider.value = Mathf.Lerp(thisUnderSlider.value, overSlider.value, Time.deltaTime * lerpSpeed);
            }
            else // under slider is less than, set equal to
            {
                thisUnderSlider.value = overSlider.value;
            }
        }
    }
}
