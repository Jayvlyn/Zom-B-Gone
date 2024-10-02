using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static Saves saves;
    public static string loadedSave;
    private static bool hasInitialized = false;

    private void Awake()
    {
        if (hasInitialized) return;

        try
        {
            saves = OdinSaveSystem.Load();
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to load lootrunner saves, creating new saves");
            Saves newSaves = new Saves();
            OdinSaveSystem.Save(newSaves);
            saves = newSaves; // probably redundant but its okay
        }

        hasInitialized = true;
    }

    public void DeleteLootrunner()
    {
        string name = DeletePopupShower.saveToDelete;
        Destroy(SaveOptionDeleteReserve.reserve); // destroys option button from list

        saves.lootrunnerSaves.Remove(name);
		OdinSaveSystem.Save(saves);

	}
}
