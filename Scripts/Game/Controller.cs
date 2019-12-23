using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Controller : MonoBehaviour
{
    public enum LevelState
    {
        ANIMATING,
        WAITING_FOR_MOVE,
        FAILURE,
        VICTORY
    }

    [Header("REFERENCES")]
    public GameManager GameManager;
    [SerializeField] Generator generator;
    [SerializeField] View view;

    [SerializeField] ChooseBlock chooseBlock;
    AudioManager AudioManager;
    public Button currentButtonClicked;



    [Header("INFO")]
    public Level Level;
    public LevelState CurrentState = LevelState.WAITING_FOR_MOVE;
    [SerializeField] int MovesOnStart;
    [SerializeField] int MovesLeft;
    public List<Block> BlocksToGenerate;
    public List<Block> triplet;
    public bool ThereIsTriplet;
    public bool ThereIsFreeSpace;
    List<BlockToMove> currentlyMovingBlocks;

    [Header("BOARD")]
    public int width = 10;
    public int height = 10;
    public bool again = true;


    void Awake()
    {
        #region Setting References And Variables
        AudioManager = AudioManager.instance;
        GameManager = FindObjectOfType<GameManager>();
        Level = GameManager.CurrentLevel;

        MovesOnStart = Level.Moves;
        MovesLeft = Level.Moves;

        BlocksToGenerate = Level.ReturnBlocks();

        triplet = new List<Block>();
        currentlyMovingBlocks = new List<BlockToMove>();
        #endregion

        #region Initializing Level

        view.ChangeBackground(Level.BackgroundImage, Level.BackgroundColor);

        generator.InitBoard(width, height);

        foreach (Block b in BlocksToGenerate)
        {
            generator.InsertBlockOnBoard(b);
        }

        // generator.DebugBoard();
        view.VisualizeBoard(generator.board);
        view.VisualizeBlocks(generator.board);
        view.SetLevelNumber(Level.id);
        view.UpdateMoves(MovesLeft);
        #endregion
    }
    void Start()
    {
        AudioManager.instance.Play("start_game");
    }
    public void TurnMainLoop(ChooseBlockButton buttonClicked)
    {
        StartCoroutine(MainLoop(buttonClicked));
    }

    public IEnumerator MainLoop(ChooseBlockButton buttonClicked)
    {
        if (CurrentState == LevelState.WAITING_FOR_MOVE)
        {
            if (generator.BlockCanBeAddedToBoard(buttonClicked.Block))
            {
                CurrentState = LevelState.ANIMATING;
                UpdateMoves(-1);
                yield return StartCoroutine(AddBlockCoroutine(buttonClicked));
                // generator.DebugBoard();

                if (generator.ThereIsATriplet(ref triplet))
                {
                    ThereIsTriplet = true;

                    while (ThereIsTriplet || generator.ThereAreBlocksWithFreeSpaces())
                    {
                        yield return StartCoroutine(DestroyTripletCoroutine(triplet));
                        // generator.DebugBoard();
                        yield return StartCoroutine(MoveAllBlocksToFreeSpacesCoroutine());
                        // generator.DebugBoard();
                    }
                    if (ThereAreNoBlocksOnBoard())
                    {
                        yield return new WaitForSeconds(0.7f);
                        Victory();
                        yield break;
                    }

                }



                if (MovesLeft <= 0)
                {
                    //there was no triplet found and you have no moves left. There are though still blocks on board so you lose.
                    Failure();
                    yield break;
                }
                else
                {   //there was no triplet found but you have more moves left
                    CurrentState = LevelState.WAITING_FOR_MOVE;
                    yield break;
                }
            }

            else
            {   //It is impossible to add Block because of either no place or no block on the way (meaningless move).
                yield break;
            }
        }
    }
    IEnumerator AddBlockCoroutine(ChooseBlockButton buttonClicked)
    {
        Vector2 targetCoord = generator.ReturnTargetCoordinates(buttonClicked.Block);
        Block generatedBlock = new Block(buttonClicked.Block.direction, buttonClicked.Block.kind, (int)targetCoord.x, (int)targetCoord.y);
        generator.InsertBlockOnBoard(generatedBlock);
        yield return StartCoroutine(view.AnimateBlockMovement(buttonClicked.Block.GetPos(), targetCoord, generatedBlock, true));
    }

    IEnumerator DestroyTripletCoroutine(List<Block> triplet)
    {

        AudioManager.Play("triplet");
        view.AnimateTriplateDissapear(triplet);
        generator.TripletFound(triplet);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator MoveAllBlocksToFreeSpacesCoroutine()
    {
        yield return MoveBlocksToFreeSpaces(MovingBlocksOption.Horizontal);
        yield return MoveBlocksToFreeSpaces(MovingBlocksOption.Vertical);

        if (!generator.ThereIsATriplet(ref triplet))
            ThereIsTriplet = false;

    }

    #region MOVING BLOCKS TO FREE SPACE

    public IEnumerator MoveBlocksToFreeSpaces(MovingBlocksOption movingOption)
    {
        currentlyMovingBlocks = generator.ReturnAllMovingBlocks(movingOption);
        foreach (BlockToMove b in currentlyMovingBlocks)
        {
            Vector2 blockCoords = b.Block.GetPos();
            generator.CleanBlockOnBoard(blockCoords);
            view.FillContainerWithEntity(blockCoords, new Entity((int)blockCoords.x, (int)blockCoords.y));

            if (!b.OutOfBoard)
            {
                b.Block.x = (int)b.EndCoords.x;
                b.Block.y = (int)b.EndCoords.y;
                generator.InsertBlockOnBoard(b.Block);
            }
        }
        yield return StartCoroutine(view.AnimateBlocksMovement(currentlyMovingBlocks));
    }

    bool ThereAreNoBlocksOnBoard()
    {
        if (generator.BlocksOnBoard.Count == 0)
        {
            return true;
        }

        else return false;
    }

    #endregion


    #region Choosing Block 
    public void ShowBlockPanel(Entity e, Button b)
    {
        currentButtonClicked = b;
        if (!chooseBlock.gameObject.activeInHierarchy)
        {
            chooseBlock.gameObject.SetActive(true);
        }
        chooseBlock.Set(e);
    }

    public void HideBlockPanel()
    {
        chooseBlock.gameObject.SetActive(false);
    }
    public void ChooseBlock(ChooseBlockButton buttonClicked)
    {
        HideBlockPanel();
        TurnMainLoop(buttonClicked);
    }



    #endregion
    void Failure()
    {
        CurrentState = LevelState.FAILURE;
        view.ShowFailurePanel();
    }

    void Victory()
    {
        CurrentState = LevelState.VICTORY;
        AudioManager.Play("win", false, 0.8f);
        GameManager.SaveLevelProgress(Level.id, MovesOnStart - MovesLeft);
        view.ShowVictoryPanel(CalculateNumberOfStars());
    }

    int CalculateNumberOfStars()
    {
        int i = 1;
        if (MovesOnStart - MovesLeft <= Level.MaxMovesForTwoStars)
            i = 2;
        if (MovesOnStart - MovesLeft <= Level.MaxMovesForThreeStars)
            i = 3;

        return i;
    }
    void UpdateMoves(int delta)
    {
        MovesLeft += delta;
        view.UpdateMoves(MovesLeft);
    }
}
public enum MovingBlocksOption
{
    Horizontal,
    Vertical
}
public class BlockToMove
{
    public Block Block;
    public Vector2 EndCoords;
    public bool OutOfBoard;

    public BlockToMove(Block b, Vector2 endCoords, bool OutOfBoard)
    {
        this.Block = b;
        this.EndCoords = endCoords;
        this.OutOfBoard = OutOfBoard;
    }
    public BlockToMove(Block b, Vector2 endCoords)
    {
        this.Block = b;
        this.EndCoords = endCoords;
    }

    public BlockToMove(Block b)
    {
        this.Block = b;
    }
}