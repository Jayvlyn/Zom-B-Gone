using UnityEngine;

public class Toggler : Tool
{
    public GameObject toggledObject;
    private bool toggled = false;

	public override void Use()
	{
        base.Use();
        toggled = !toggled;

        toggledObject.SetActive(toggled);
	}
}
