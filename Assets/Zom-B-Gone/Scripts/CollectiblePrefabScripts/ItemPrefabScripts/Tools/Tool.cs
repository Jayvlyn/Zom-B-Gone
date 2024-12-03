using UnityEngine;

public class Tool : Item
{
	protected ToolData toolData;
	protected bool activated = false;
	protected float activatedTimer;

	public override void Use()
	{
		if(toolData.playToolSoundOnUse && toolData.toolSound != null)
		{
			audioSource.PlayOneShot(toolData.toolSound);
		}
	}

	protected void Start()
    {
        if(data as ToolData != null)
		{
			toolData = (ToolData)data;
		}
    }

}
