using System.Collections;
using UnityEngine;

public class DelayedAnimatorEnabler : MonoBehaviour
{
    [SerializeField] Animator animator;

	private void Start()
	{
		StartCoroutine(Delay());
	}

	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(0.4f);
		animator.enabled = true;
	}
}
