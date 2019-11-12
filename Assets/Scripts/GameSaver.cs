
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public float MusicVol;
    public float SFXVol;
    public int resX;
    public int resY;
    public string name = "Player";
    public bool fullscreen;
}

[System.Serializable]
public static class GameSaver
{
    private static string path { get { return Application.persistentDataPath; } }
    private static SaveData data = null;

    public static SaveData LoadGame()
    {
        SaveData saveData;
        if(data != null)
        {
            return data;
        }
        if (File.Exists(path+"/gamesave.td"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + "/gamesave.td", FileMode.Open);
            file.Position = 0;
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();
#if UNITY_EDITOR
            saveData.name = "test";
#endif
            data = saveData;
            return saveData;
        }
        else
        {
            Debug.Log("No saved game!");
            return null;
        }
    }

    public static void Savegame(SaveData _data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path + "/gamesave.td");
        bf.Serialize(file, _data);
        Debug.Log("Game Saved");
        data = _data;
        file.Close();
    }
}
