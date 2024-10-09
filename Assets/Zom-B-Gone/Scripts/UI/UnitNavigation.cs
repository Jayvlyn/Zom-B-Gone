using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitNavigation : MonoBehaviour
{
    public LootrunnerDataRefs dataRefs;

    public void OpenMainMenu()
    {
        SaveManager.UpdateCurrentSave(dataRefs);
        OdinSaveSystem.Save(SaveManager.saves);
        SceneManager.LoadScene("MainMenu");
    }
}
