using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootrunnerCreator : MonoBehaviour
{
    public GameObject takenNamePrompt;
    public TMP_InputField nameField;

    public void TryCreateLootrunner()
    {
        string lootrunnerName = nameField.text;

        if(SaveManager.saves.lootrunnerSaves.ContainsKey(lootrunnerName))
        {
			takenNamePrompt.SetActive(true);
			return;
		}

        LootrunnerSave newSave = new LootrunnerSave();

        SaveManager.saves.lootrunnerSaves[lootrunnerName] = newSave;
        OdinSaveSystem.Save(SaveManager.saves);
    }
}
