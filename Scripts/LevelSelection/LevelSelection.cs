using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelSelection : MonoBehaviour
{
    Database progressDB;
    public List<Level> Levels;
    public LevelContainer LevelContainerPrefab;
    public List<LevelContainer> LevelContainers;
    public ScrollRect ScrollRect;

    void Start()
    {
        GenerateLevelContainers();
        // SetScrollPosition();
    }

    void GenerateLevelContainers()
    {
        Levels = GameObject.FindObjectOfType<GameManager>().Levels;
        List<Progress> progressList = Database.GetAllProgresses();

        foreach (Level L in Levels)
        {
            GameObject levelContainerGO = Instantiate(LevelContainerPrefab.gameObject);
            levelContainerGO.transform.SetParent(this.transform);
            levelContainerGO.transform.localScale = new Vector3(0.6f, 0.5f, 1);

            LevelContainer LevelContainer = levelContainerGO.GetComponent<LevelContainer>();
            LevelContainer.Level = L;
            LevelContainer.LevelProgress = progressList.Where(x => x.id == L.id).SingleOrDefault();

            LevelContainers.Add(LevelContainer);
        }
    }


    void SetScrollPosition()
    {
        Level l = GameManager.CurrentLevel;

        if (l != null)
        {
            ScrollRect.verticalNormalizedPosition = calc(l.id);
        }
        else
        {
            ScrollRect.verticalNormalizedPosition = calc(LevelContainers.Where(lev => lev.unlocked == true).LastOrDefault().Level.id);
        }
    }


    float calc(int ID)
    {
        if (ID > 15)
        {

            return 0;
        }
        else return 1;
    }
    void SetScrollRectPosition()
    {
        Level l = GameManager.CurrentLevel;
        if (l != null)
        {
            float i = funct(l.id, LevelContainers.Count);
            ScrollRect.verticalNormalizedPosition = i;
        }

        else
        {
            LevelContainer levCont = LevelContainers.Where(lev => lev.unlocked == true).LastOrDefault();
            float j = funct(levCont.Level.id, LevelContainers.Count);
            ScrollRect.verticalNormalizedPosition = j;

        }
    }

    float funct(int activeLevel, int levelsCount)
    {
        float x = (float)activeLevel / (1 - (float)levelsCount) + (1 - (1 / (1 - (float)levelsCount)));
        return x;
    }
}
