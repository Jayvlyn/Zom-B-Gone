using System.Collections;
using UnityEditor;
using UnityEngine;

public class JollyChimp : Tool
{
    public Sprite crashSprite;
	private Sprite regularSprite;
	private float crashInterval = .5f;

	private Coroutine activateRoutine;
	private Coroutine activateTimerRoutine;

	private void Start()
	{
		base.Start();
		regularSprite = itemRenderer.sprite;
	}

	public override void PickUp(Transform parent, bool rightHand, bool adding = false)
	{
		base.PickUp(parent, rightHand);

		if(activated || activateRoutine != null)
		{
			if(activateRoutine != null )	 StopCoroutine(activateRoutine);
			if(activateTimerRoutine != null) StopCoroutine(activateTimerRoutine);
			itemRenderer.sprite = regularSprite;
			activated = false;
		}
	}

	public override void Use()
	{
		base.Use();
		Drop();
		if(!activated)
		{
			activateRoutine = StartCoroutine(Activate());
		}
	}

	private IEnumerator Activate()
	{
		yield return new WaitForSeconds(toolData.activateDelay);

		activated = true;

		activateTimerRoutine = StartCoroutine(ActivateTimer());

		while(activated)
		{
			itemRenderer.sprite = crashSprite;

			Utils.MakeSoundWave(transform.position, itemData.noiseRadius);

			yield return new WaitForSeconds(0.2f);
			itemRenderer.sprite = regularSprite;



			yield return new WaitForSeconds(crashInterval);
		}
		itemRenderer.sprite = regularSprite;
		activateRoutine = null;
	}

	private IEnumerator ActivateTimer()
	{
		activatedTimer = toolData.timeActivated;
		while (activatedTimer > 0)
		{
			activatedTimer -= Time.deltaTime;
			yield return null;
		}
		activated = false;
		activateTimerRoutine = null;
	}
}
