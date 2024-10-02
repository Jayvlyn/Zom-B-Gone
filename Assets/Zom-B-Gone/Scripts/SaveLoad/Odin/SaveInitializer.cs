using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInitializer : MonoBehaviour
{
    public static Saves lootrunnerSaves;
    private static bool hasInitialized = false;

    private void Awake()
    {
        if (hasInitialized) return;

        try
        {
            lootrunnerSaves = OdinSaveSystem.Load(OdinSaveSystem.path);
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to load lootrunner saves, creating new saves");
            Saves saves = new Saves();
            OdinSaveSystem.Save(saves, OdinSaveSystem.path);
            lootrunnerSaves = saves; // probably redundant but its okay
        }

        hasInitialized = true;
    }
}
