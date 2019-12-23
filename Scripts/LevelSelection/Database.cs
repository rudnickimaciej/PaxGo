using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public struct Progress
{
    public int id;
    public bool unlocked;
    public bool firstTimeUnlocked;
    public int MovesRecord;

    public Progress(int id, bool unlocked = true, bool firstTimeUnlocked = false, int MovesRecord = 0)
    {
        this.id = id;
        this.firstTimeUnlocked = firstTimeUnlocked;
        this.unlocked = unlocked;
        this.MovesRecord = MovesRecord;
    }
}

[System.Serializable]
public struct VolumeData
{
    public float EffectsVolume;
    public float MusicVolume;
    public VolumeData(float EffectVol, float MusicVol)
    {
        EffectsVolume = EffectVol;
        MusicVolume = MusicVol;
    }
}

public class Database : MonoBehaviour
{
    public static List<Progress> GetAllProgresses()
    {
        BinaryFormatter bf = new BinaryFormatter();
        List<Progress> progresess = new List<Progress>();
        foreach (string filePath in System.IO.Directory.GetFiles(Application.persistentDataPath + @"/Levels"))
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            Progress record = (Progress)bf.Deserialize(stream);
            progresess.Add(record);
            stream.Close();
        }
        return progresess;
    }

    public static Progress GetOneProgress(int id)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream f = File.Open(Application.persistentDataPath + @"/Levels/level_" + id + ".dat", FileMode.Open);
        Progress record = (Progress)bf.Deserialize(f);
        f.Close();
        return record;
    }

    public static void SaveProgress(Progress progressToSave)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + @"/Levels/level_" + progressToSave.id + ".dat");
        bf.Serialize(file, progressToSave);
        file.Close();
    }


    public static void SaveVolumeData(VolumeData volumeData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + @"/volume.dat");
        bf.Serialize(file, volumeData);
        file.Close();
    }
}
