using GameEvents;
using UnityEngine;

public class ButtonEnterReader : MonoBehaviour
{
    public CustomButton button;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            button.ClickDown();
        }
		else if (Input.GetKeyUp(KeyCode.Return))
		{
            button.ClickUp();
		}
	}
}
