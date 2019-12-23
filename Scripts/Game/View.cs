using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class View : MonoBehaviour
{
    [Header("UI")]
    public Text MoveAmount;
    public Text LevelNumber;
    public Image Background;

    [Header("Panels")]
    public GameObject FailurePanel;
    public VictoryPanel VictoryPanel;

    [Header("Parents")]
    public Transform entityCointainersTopParent;
    public Transform entityCointainersBottomParent;

    [Header("Prefabs")]
    public GameObject buttonPrefab;
    public GameObject blockPrefab;
    public GameObject entityPrefab;

    [Header("Variables")]
    public float blockMoveSpeed = 2f;

    [Header("References")]
    AudioManager AudioManager;
    [SerializeField] public EntityContainer[,] entityCointainersTop;
    [SerializeField] public Animator[,] entityCointainersBottom;



    void Awake()
    {
        AudioManager = AudioManager.instance;
    }

    public void ChangeBackground(Sprite background, Color color)
    {
        Background.sprite = background;
        Background.color = color;
    }

    #region INITIAL VISUALIZATION 
    public void VisualizeBoard(Entity[,] board)
    {
        int x = board.GetLength(0);
        int y = board.GetLength(1);

        entityCointainersBottom = new Animator[x, y]; //9,9

        for (int i = 1; i < x - 1; i++)
        {
            for (int j = 1; j < y - 1; j++)
            {
                GameObject eGO;
                eGO = (GameObject)Instantiate(entityPrefab);
                entityCointainersBottom[i, j] = eGO.GetComponent<Animator>();
                eGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(100 * i, 100 * j);
                eGO.transform.SetParent(entityCointainersBottomParent, false);
                eGO.GetComponent<EntityContainer>().Entity = new Entity(i, j, Direction.None, "board");

            }
        }
    }

    ///<summary>
    /// Creates visual reprezentation of board.
    ///</summary>
    ///<param name = "board"> Entity[,] board to visualize </param>

    public void VisualizeBlocks(Entity[,] board)
    {
        int x = board.GetLength(0);
        int y = board.GetLength(1);

        entityCointainersTop = new EntityContainer[x, y]; //9,9

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject eGO;


                if (board[i, j] is BoardButton)
                {
                    eGO = (GameObject)Instantiate(buttonPrefab);
                }
                else if (board[i, j] is Block)
                {
                    eGO = (GameObject)Instantiate(blockPrefab);
                }
                else
                {
                    eGO = (GameObject)Instantiate(blockPrefab);
                }
                eGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(100 * i, 100 * j);
                entityCointainersTop[i, j] = eGO.GetComponent<EntityContainer>();
                eGO.transform.SetParent(entityCointainersTopParent, false);
                if (board[i, j] != null)
                    entityCointainersTop[i, j].Entity = board[i, j];

            }
        }
    }

    #endregion


    ///<summary>
    /// Animates movement of Block
    ///</summary>
    ///<param name = "StartBoardCoords"> Animation starting coords </param>
    ///<param name = "EndBoardCoords"> Animation ending coords </param>
    ///<param name = "block"> Block that will be visualized  </param>
    ///<param name = "FillContainer"> If true, at the end of animation, EntityContainer corresponding to EndBoardCoords                        will change its Visual Reprezentation from background to Block</param>

    public IEnumerator AnimateBlockMovement(Vector2 StartBoardCoords, Vector2 EndBoardCoords, Block block, bool FillContainer)
    {
        Vector2 startBoardCoords = StartBoardCoords;

        Vector2 startRealCoords = entityCointainersTop[(int)startBoardCoords.x, (int)startBoardCoords.y].GetPosition();
        Vector2 endRealCoords = entityCointainersTop[(int)EndBoardCoords.x, (int)EndBoardCoords.y].GetPosition();


        GameObject Block = (GameObject)Instantiate(blockPrefab);
        Block.transform.SetParent(entityCointainersTopParent, false);

        EntityContainer container = Block.GetComponent<BlockContainer>();
        container.Entity = block;

        container.SetPosition(startRealCoords);

        Vector2 dir = (endRealCoords - startRealCoords).normalized;

        while (Vector2.Distance(container.GetPosition(), endRealCoords) >= 10)
        {
            container.SetPosition(container.GetPosition() + dir * blockMoveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        if (FillContainer)
        {
            FillContainerWithEntity(EndBoardCoords, block);
        }
        else
        {
            AudioManager.PlayIfNotAlreadyPlayed("block_out");
        }
        Destroy(Block);
        yield break;
    }


    public IEnumerator AnimateBlocksMovement(List<BlockToMove> blocksToMove)
    {
        List<Task> Tasks = new List<Task>();

        foreach (BlockToMove b in blocksToMove)
        {
            Tasks.Add(new Task(AnimateBlockMovement(b.Block.GetPos(), b.EndCoords, b.Block, !b.OutOfBoard)));
        }

        foreach (Task t in Tasks)
        {
            t.Start();
        }

        yield return StartCoroutine(HelpCoroutine(Tasks));

        yield return null;
    }


    IEnumerator HelpCoroutine(List<Task> tasks)
    {
        int i = 0;
        while (true)
        {
            i = 0;
            yield return new WaitForSeconds(0.1f);
            foreach (Task t in tasks)
            {
                if (t.Running) continue;
                else i++;
            }
            if (i >= tasks.Count)
                yield break;
        }
    }

    public void FillContainerWithEntity(Vector2 coords, Entity e)
    {
        entityCointainersTop[(int)coords.x, (int)coords.y].Entity = e;
    }


    public Vector2 ReturnPosOfContainer(Vector2 boardCoords)
    {
        return entityCointainersTop[(int)boardCoords.x, (int)boardCoords.y].GetComponent<RectTransform>().anchoredPosition;
    }


    public void AnimateTriplateDissapear(List<Block> blocksToDissapear)
    {
        foreach (Block b in blocksToDissapear)
        {
            entityCointainersTop[b.x, b.y].Entity = new Entity(b.x, b.y);
            entityCointainersBottom[b.x, b.y].SetTrigger("BlockDissapear");
        }
    }

    #region UPDATING INFO

    public void SetLevelNumber(int level)
    {
        LevelNumber.text = level.ToString();
    }

    public void UpdateMoves(int amount)
    {
        MoveAmount.text = amount.ToString();
    }
    #endregion


    public void ShowFailurePanel()
    {
        FailurePanel.SetActive(true);
    }

    public void ShowVictoryPanel(int numberOfStars)
    {
        VictoryPanel.ShowPanel(numberOfStars);
    }

}

