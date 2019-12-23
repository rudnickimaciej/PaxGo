using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
public class Preload : MonoBehaviour
{
    ScenesManager ScenesManager;
    void Awake()
    {
        ScenesManager = FindObjectOfType<ScenesManager>();
    }
    void Start()
    {
        InitFiles();
        InitVolume();
        ScenesManager.LoadScene(1);
    }

    void InitFiles()
    {
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + @"/Levels");
        foreach (Level l in FindObjectOfType<GameManager>().Levels)
        {
            FileStream file = File.Open(Application.persistentDataPath + @"/Levels/level_" + l.id + ".dat", FileMode.OpenOrCreate);
            if (file.Length == 0)
            {
                file.Close();
                if (l.id == 1)

                    Database.SaveProgress(new Progress(l.id, true, true, -1));
                else
                {
                    Database.SaveProgress(new Progress(l.id, false, true, -1));
                }
                file.Close();
            }
        }
    }

    void InitVolume()
    {
        FileStream file = File.Open(Application.persistentDataPath + @"/volume.dat", FileMode.OpenOrCreate);

        if (file.Length == 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            VolumeData v = new VolumeData(0.5f, 0.5f);
            bf.Serialize(file, v);
            file.Close();
            AudioManager.instance.SetAudioVolume(0.5f, 0.5f);
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            VolumeData volData = (VolumeData)bf.Deserialize(file);

            AudioManager.instance.SetAudioVolume(volData);
        }

        file.Close();
    }

}
