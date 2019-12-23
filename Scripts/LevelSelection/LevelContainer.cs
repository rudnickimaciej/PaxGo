using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelContainer : MonoBehaviour
{
    [Header("Colors")]

    public Color UnlockedLevelBackground = Color.white;
    public Color LockedLevelBackground = new Color(125, 125, 125, 255);

    public Color ActiveStarColor = new Color(255, 164, 0, 255);
    public Color NonactiveStarColor = new Color(172, 172, 172, 255);

    [SerializeField] private Level level;
    public Level Level
    {
        set
        {
            level = value;
            SetName(level.id.ToString());
            SetImage(level.BackgroundImage, level.BackgroundColor);


        }
        get { return level; }
    }
    [SerializeField] private Progress levelProgress;


    public Progress LevelProgress
    {
        set
        {
            levelProgress = value;

            SetLock(levelProgress.unlocked);
            if (unlocked)
            {
                SetBackgroundCurtain(UnlockedLevelBackground);
            }
            else
            {
                SetBackgroundCurtain(LockedLevelBackground);
            }
            MovesRecords = levelProgress.MovesRecord;
            GenerateStars(CalculateNumberOfStars(levelProgress.MovesRecord));
        }
        get { return levelProgress; }
    }

    void Awake()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => FindObjectOfType<GameManager>().PlayLevel(Level));
    }

    void Start()
    {
        if (unlocked)
        {
            if (LevelProgress.firstTimeUnlocked)
            {
                GetComponent<Animator>().SetTrigger("FirstUnlock");
            }
        }
    }

    [Header("UI Refs")]
    public Image Background;
    public Image BackgroundCurtain;
    public Text LevelName;
    public Image LockIcon;
    public List<Image> Stars;
    public int MovesRecords;
    public bool unlocked = false;

    void SetName(string name)
    {
        LevelName.text = name;
    }

    void SetImage(Sprite image, Color color)
    {
        Background.sprite = image;
        Background.color = color;
    }
    void SetBackgroundCurtain(Color color)
    {
        BackgroundCurtain.color = color;
    }

    void SetLock(bool unlocked)
    {
        this.unlocked = unlocked;
        LockIcon.enabled = !unlocked;
        LevelName.gameObject.SetActive(unlocked);
        GetComponent<UnityEngine.UI.Button>().interactable = unlocked;

    }

    void SetRecord(int MoveRecords)
    {

    }

    int CalculateNumberOfStars(int MovesRecord)
    {
        int i = 0;
        if (!unlocked || MovesRecord <= 0)
            return i;

        if (MovesRecord <= level.MaxMovesForThreeStars)
            i = 3;
        else if (MovesRecord <= level.MaxMovesForTwoStars)
            i = 2;
        else if (MovesRecord <= level.MaxMovesForOneStar)
            i = 1;

        return i;
    }

    void GenerateStars(int numbOfStars)
    {
        for (int i = 0; i < numbOfStars; i++)
        {
            Stars[i].color = ActiveStarColor;
        }

    }
}
