using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LootrunnerLoader : MonoBehaviour
{
    [SerializeField] TMP_Text nameMesh;

    public void PlayRunner()
    {
        SaveManager.loadedSave = nameMesh.text;
        SceneManager.LoadScene("Unit");
    }
}
