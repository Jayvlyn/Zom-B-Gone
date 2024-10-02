using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeletePopupShower : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] TMP_Text nameText;
    public static string saveToDelete;

    public void ShowPopup(string lootrunnerName)
    {
        popup.SetActive(true);
        nameText.text = lootrunnerName;
        saveToDelete = lootrunnerName;
    }
}
