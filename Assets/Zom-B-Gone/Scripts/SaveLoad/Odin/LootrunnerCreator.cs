using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootrunnerCreator : MonoBehaviour
{
    public TMP_InputField nameField;
    public ItemData itemData;
    public void CreateLootrunner()
    {
        LootrunnerSave newSave = new LootrunnerSave();
        newSave.lootrunnerName = nameField.text;


        //saves.lootrunnerSaves.Add(newSave);


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Saves saves = OdinSaveSystem.Load(OdinSaveSystem.path);
            LootrunnerSave loadedSave = saves.lootrunnerSaves[0];
            Debug.Log(loadedSave.leftHandItem.name);
        }
    }
}
