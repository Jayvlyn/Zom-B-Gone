using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject pauseUI;
    public GameObject settingsUI;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                if(settingsUI.activeSelf)
                {
                    //pauseUI.SetActive(true);
                    settingsUI.SetActive(false);
                }
                else if(pauseUI.activeSelf)
                {
                    Resume();
                }
            } else
            {
                Pause();
            }
        }
    }

    private void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    private void Pause()
    { 
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }


    #region BUTTON EVENTS

    public void OnResume()
    {
        Resume();
    }

    public void OnSettings()
    {
        settingsUI.SetActive(true);
    }

    public void OnMainMenu()
    {
        Resume();

        StartCoroutine(DelayedSceneChange("MainMenu", 2));
    }

    public void OnAbandonRun()
    {
        Resume();

        StartCoroutine(DelayedSceneChange("Unit", 2));
    }


    public void OnQuit()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Utils.ClearPlayerTemporaryContainers();
        }
        SaveManager.UpdateCurrentSave(GameManager.Instance.dataRefs);
        OdinSaveSystem.Save(SaveManager.saves);
        Debug.Log("Quitting Game");
        Application.Quit();
    }
    #endregion


    private IEnumerator DelayedSceneChange(string scene, float delay)
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Utils.ClearPlayerTemporaryContainers();
        }
        SaveManager.UpdateCurrentSave(GameManager.Instance.dataRefs);
        OdinSaveSystem.Save(SaveManager.saves);
        GameManager.Instance.circleAnimator.SetTrigger("CloseCircle");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }
}
