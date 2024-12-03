using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitNavigation : MonoBehaviour
{
    public Animator circleAnimator;

    public void OpenMainMenu()
    {
        circleAnimator.SetTrigger("CloseCircle");
        StartCoroutine(DelayedMainMenu());
    }

    public IEnumerator DelayedMainMenu()
    {
        yield return new WaitForSeconds(2.5f);
		SaveManager.UpdateCurrentSave(GameManager.Instance.dataRefs);
		OdinSaveSystem.Save(SaveManager.saves);
		SceneManager.LoadScene("MainMenu");
	}
}
