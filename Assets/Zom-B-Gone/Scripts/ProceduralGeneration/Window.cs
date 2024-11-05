using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
	public GameObject glass;
	public AudioSource audioSource;
	public AudioClip[] glassBreakSounds;

	public void ShatterGlass()
	{
		int roll = Random.Range(0, glassBreakSounds.Length);
		audioSource.resource = glassBreakSounds[roll];
		audioSource.Play();

		StartCoroutine(DestroyTimer());

		// do particle stuff
	}

	private IEnumerator DestroyTimer()
	{
		glass.SetActive(false);
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}

}
