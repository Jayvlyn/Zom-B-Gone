using OdinSerializer;
using System.IO;
using UnityEngine;

public static class OdinSaveSystem
{
    public static string path = Application.persistentDataPath + "/leona.boid";

    public static void Save(Saves data)
    {
        byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);
        File.WriteAllBytes(path, bytes);
    }

    public static Saves Load()
    {
        byte[] bytes = File.ReadAllBytes(path);
        return SerializationUtility.DeserializeValue<Saves>(bytes, DataFormat.Binary);
    }



	//public static void Save(Saves data, string filePath)
	//{
	//	byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);
	//	File.WriteAllBytes(filePath, bytes);
	//}

	//public static Saves Load(string filePath)
	//{
	//	byte[] bytes = File.ReadAllBytes(filePath);
	//	return SerializationUtility.DeserializeValue<Saves>(bytes, DataFormat.Binary);
	//}
}