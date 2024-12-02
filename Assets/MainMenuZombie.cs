using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuZombie : MonoBehaviour
{
	public Image buttonBlocker;
	public Animator animator;
	public void ZombieAnimEnd()
	{
        if (buttonBlocker!=null)
        {
			SceneManager.LoadScene("Unit");
        }
	}

	public void EntryEnd()
	{
		animator.enabled = false;
		if (buttonBlocker != null)
		{
			StartCoroutine(BlockerFade());
		}
	}

	private IEnumerator BlockerFade()
	{
		float elapsedTime = 0;
		float duration = 0.3f;
		while (elapsedTime < duration)
		{
			float alpha = buttonBlocker.color.a;

			alpha = Mathf.Lerp(buttonBlocker.color.a, 0, elapsedTime / duration);

			buttonBlocker.color = new Color(buttonBlocker.color.r, buttonBlocker.color.g, buttonBlocker.color.b, alpha);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		buttonBlocker.gameObject.SetActive(false);
	}
}
