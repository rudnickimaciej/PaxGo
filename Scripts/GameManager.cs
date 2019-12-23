using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    [Header("Important References")]
    [SerializeField] ScenesManager ScenesManager;
    [SerializeField] Database ProgressDatabase;
    [SerializeField] AudioManager AudioManager;
    [SerializeField] public List<Level> Levels;
    public static Level CurrentLevel;

    void Awake()
    {
        ScenesManager = GetComponent<ScenesManager>();
        ProgressDatabase = GetComponent<Database>();
        AudioManager = AudioManager.instance;
        AudioManager.Play("background", true);
    }

    public void PlayLevel(Level level)
    {
        CurrentLevel = level;
        ScenesManager.LoadScene(3);
    }

    public void SaveLevelProgress(int levelId, int MovesRecord)
    {
        Progress progressOld = Database.GetOneProgress(levelId);
        if (MovesRecord < progressOld.MovesRecord || progressOld.MovesRecord <= 0)
        {
            Progress progressUpdate = new Progress(levelId, true, false, MovesRecord);
            Database.SaveProgress(progressUpdate);
        }

        int nextLevelId = ++levelId;
        if (nextLevelId <= Levels.Count)
        {
            Progress nextProgress = Database.GetOneProgress(nextLevelId);
            if (nextProgress.unlocked == true) return;
            else
            {
                nextProgress.unlocked = true;
                Database.SaveProgress(nextProgress);
            }
        }
    }
}
