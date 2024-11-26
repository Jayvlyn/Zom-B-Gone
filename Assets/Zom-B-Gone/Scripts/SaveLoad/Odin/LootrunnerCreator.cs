using GameEvents;
using OdinSerializer.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LootrunnerCreator : MonoBehaviour
{
    public GameObject takenNamePrompt;
    public GameObject noEmptyPrompt;
    public GameObject nameEntryPopup;
    public TMP_InputField nameField;
    public PlayerData defaultPlayerData;
    public VoidEvent startGameEvent;

    public void TryCreateLootrunner()
    {
        string lootrunnerName = nameField.text;

        if(SaveManager.saves.lootrunnerSaves.ContainsKey(lootrunnerName))
        {
            noEmptyPrompt.SetActive(false);
			takenNamePrompt.SetActive(true);
			return;
		}
        else if(lootrunnerName.IsNullOrWhitespace())
        {
            takenNamePrompt.SetActive(false);
            noEmptyPrompt.SetActive(true);
            return;
        }

        LootrunnerSave newSave = new LootrunnerSave();
        newSave.playerData = SaveManager.DeepCopyPlayerData(defaultPlayerData);
        newSave.playerData.characterName = lootrunnerName;

        SaveManager.saves.lootrunnerSaves[lootrunnerName] = newSave;
        OdinSaveSystem.Save(SaveManager.saves);
        nameEntryPopup.SetActive(false);

		SaveManager.loadedSave = newSave.playerData.characterName;

        startGameEvent.Raise();
		//SceneManager.LoadScene("Unit");
	}
}
