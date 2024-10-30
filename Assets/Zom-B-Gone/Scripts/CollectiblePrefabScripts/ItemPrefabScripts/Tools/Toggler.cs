using UnityEngine;

public class Toggler : Item
{
    public GameObject toggledObject;
    private bool toggled = false;

	public override void Use()
	{
        toggled = !toggled;

        toggledObject.SetActive(toggled);
	}
}
