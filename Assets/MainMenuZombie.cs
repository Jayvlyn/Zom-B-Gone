using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuZombie : MonoBehaviour
{

	public Animator animator;
	public void ZombieAnimEnd()
	{
		SceneManager.LoadScene("Unit");
	}

	public void EntryEnd()
	{
		animator.enabled = false;
	}
}
