using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string playerDataPath = Application.persistentDataPath + "/player.boid";
    private static string settingsDataPath = Application.persistentDataPath + "/settings.boid";

    public static void SaveSettings(SettingsMenu settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(settingsDataPath, FileMode.Create);
        SettingsData data = new SettingsData(settings);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static SettingsData LoadSettings()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        if (File.Exists(settingsDataPath))
        {
            FileStream stream = new FileStream(settingsDataPath, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Settings save file not found in " + settingsDataPath + ", creating save");
            FileStream stream = new FileStream(settingsDataPath, FileMode.Create);

            SettingsData data = new SettingsData();
            formatter.Serialize(stream, data);

            stream.Close();
            return null;
        }
    }

    #region EXAMPLE SAVE-LOAD FUNCTIONS
    //public static void SavePlayer(DataTestPlayer player)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    FileStream stream = new FileStream(playerDataPath, FileMode.Create); // file created at persistentDataPath/player.skrimp

    //    PlayerData data = new PlayerData(player);

    //    formatter.Serialize(stream, data);

    //    stream.Close(); // always close stream
    //}

    //public static PlayerData LoadPlayer()
    //{
    //    if(File.Exists(playerDataPath))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(playerDataPath, FileMode.Open);

    //        PlayerData data = formatter.Deserialize(stream) as PlayerData;
    //        stream.Close();

    //        return data;
    //    }
    //    else
    //    {
    //        Debug.LogError("Player save file not found in " + playerDataPath);
    //        return null;
    //    }
    //}
    #endregion
}
