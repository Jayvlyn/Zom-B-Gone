using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
	public GameObject glass;
	public AudioSource audioSource;
	public AudioClip[] glassBreakSounds;
	public ParticleSystem shatterParticles;

	public void ShatterGlass(bool flip)
	{
		int roll = Random.Range(0, glassBreakSounds.Length);
		audioSource.resource = glassBreakSounds[roll];
		audioSource.Play();

		StartCoroutine(DestroyTimer());

		// do particle stuff
		shatterParticles.gameObject.SetActive(true);
		if (flip) shatterParticles.gameObject.transform.Rotate(0, 0, 180);
		shatterParticles.Play();
	}

	private IEnumerator DestroyTimer()
	{
		glass.SetActive(false);
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}

}
