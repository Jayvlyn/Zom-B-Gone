using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuTransitioner : MonoBehaviour
{
	public Animator zombieAnimator;
	public FloatingEffect floatingEffect;


	public void StartAnim()
	{
		floatingEffect.enabled = false;
		zombieAnimator.SetTrigger("ScreenTransition");
	}

	public void SceneLoadAnim()
	{
	}
}

