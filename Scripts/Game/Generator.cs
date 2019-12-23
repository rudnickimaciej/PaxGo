using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Text;
using System.Linq;
public class Generator : MonoBehaviour
{
    public Dictionary<Direction, string> dic;
    int KindsCount;
    StringBuilder sb;

    public int width = 10;
    public int height = 10;
    [SerializeField] public Entity[,] board;
    [SerializeField] public List<Block> BlocksOnBoard;

    public List<Kind> AvailableKinds;

    public Kind nextKind;
    public Dictionary<Direction, Vector2> dict = new Dictionary<Direction, Vector2>
        {
           { Direction.North,new Vector2(0,1)},
           {Direction.South,new Vector2(0,-1)},
           {Direction.West,new Vector2(-1,0)},
           {Direction.East,new Vector2(1,0)}
        };

    void Awake()
    {
        sb = new StringBuilder();
        KindsCount = Kind.GetNames(typeof(Kind)).Length;
        BlocksOnBoard = new List<Block>();
    }

    void Start()
    {
        AvailableKinds = new List<Kind>();
    }

    public void InitBoard(int x, int y)
    {
        board = new Entity[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if ((i == 0 && j == 0) || (i == 0 && j == y - 1) || (i == x - 1 && j == 0) || (i == x - 1 && j == y - 1))
                {
                    board[i, j] = new Entity(i, j, Direction.None, "background");
                    continue;
                }

                if (j == 0)
                {
                    board[i, j] = new BoardButton(Direction.North, i, j); continue;
                }
                if (j == y - 1)
                {
                    board[i, j] = new BoardButton(Direction.South, i, j); continue;
                }
                if (i == 0)
                {
                    board[i, j] = new BoardButton(Direction.East, i, j); continue;
                }
                if (i == x - 1)
                {
                    board[i, j] = new BoardButton(Direction.West, i, j); continue;
                }

                board[i, j] = new Entity(i, j);
            }
        }

    }

    public void InsertBlockOnBoard(Block block)
    {
        board[block.x, block.y] = block;
        BlocksOnBoard.Add(block);
    }

    public void CleanBlockOnBoard(Vector2 coords)
    {
        board[(int)coords.x, (int)coords.y] = new Entity((int)coords.x, (int)coords.y);

        BlocksOnBoard.Remove(BlocksOnBoard.Where(b => b.GetPos() == coords).SingleOrDefault());
    }

    public void DebugBoard()
    {
        sb.Clear();
        int x = board.GetLength(0);
        int y = board.GetLength(1);
        int j = y - 1;
        for (; j >= 0; j--)
        {
            for (int i = 0; i < x; i++)
            {
                if (i == 0)
                    sb.Append("\n");
                sb.Append(board[i, j] + "   ");
            }
        }
        Debug.Log(sb);
    }

    public bool ThereIsATriplet(ref List<Block> memorizedBlocks)
    {
        for (int j = 1; j < height; j++)
        {
            memorizedBlocks.Clear();

            for (int i = 1; i < width; i++)
            {
                if (board[i, j] is Block)
                {
                    Block block = (Block)board[i, j];
                    if (memorizedBlocks.Count == 0)
                    {
                        memorizedBlocks.Add(block);
                    }
                    else if (memorizedBlocks.Last().kind == block.kind)
                    {
                        memorizedBlocks.Add(block);
                    }
                    else if (memorizedBlocks.Last().kind != block.kind)
                    {
                        memorizedBlocks.Clear();
                        memorizedBlocks.Add(block);
                    }

                    if (memorizedBlocks.Count == 3)
                    {
                        return true;
                    }
                }
                else if (!(board[i, j] is Block))
                {
                    memorizedBlocks.Clear();
                }
            }
        }


        for (int i = 1; i < width; i++)
        {
            memorizedBlocks.Clear();
            for (int j = 1; j < height; j++)
            {
                if (board[i, j] is Block)
                {
                    Block block = (Block)board[i, j];
                    if (memorizedBlocks.Count == 0)
                    {
                        memorizedBlocks.Add(block);
                    }
                    else if (memorizedBlocks.Last().kind == block.kind)
                    {
                        memorizedBlocks.Add(block);
                    }
                    else if (memorizedBlocks.Last().kind != block.kind)
                    {
                        memorizedBlocks.Clear();
                        memorizedBlocks.Add(block);
                    }

                    if (memorizedBlocks.Count == 3)
                    {
                        return true;
                    }
                }
                else if (!(board[i, j] is Block))
                {
                    memorizedBlocks.Clear();

                }
            }
        }
        return false;
    }

    public void TripletFound(List<Block> blocks)
    {

        foreach (Block b in blocks)
        {
            board[b.x, b.y] = new Entity(b.x, b.y, Direction.None);
            BlocksOnBoard.Remove(b);
        }
    }

    public Vector2 ReturnTargetCoordinates(Entity button)
    {
        int x = button.x;
        int y = button.y;
        Vector2 dir = dict[button.direction];
        int delta = 1;

        while (delta < width)
        {
            if (!(board[(int)(x + dir.x * (delta)), (int)(y + dir.y * (delta))] is Block))
            {
                delta++;
            }
            else
            {
                return new Vector2((int)(x + dir.x * (delta - 1)), (int)(y + dir.y * (delta - 1)));
            }
        }

        return Vector2.zero;
    }
    public Vector2 ReturnTargetCoordinates2(Entity button)
    {
        int x = button.x;
        int y = button.y;
        Vector2 dir = dict[button.direction];
        int delta = 1;

        while (!CoordsAreOutOfBoard(new Vector2((int)(x + dir.x * (delta)), (int)(y + dir.y * (delta)))))
        {
            int i = (int)(x + dir.x * delta);
            int j = (int)(y + dir.y * delta);
            if (!(board[(int)(x + dir.x * delta), (int)(y + dir.y * delta)] is Block))
            {
                delta++;
            }
            else
            {
                return new Vector2(x + dir.x * (delta - 1), (y + dir.y * (delta - 1)));
            }
        }
        return new Vector2(x + dir.x * (delta), (y + dir.y * (delta)));
    }

    public Kind SetNextKind()
    {
        nextKind = GetRandomKind();
        return nextKind;
    }

    Kind GetRandomKind()
    {
        return ReturnAllKindsOnBoard()[Random.Range(0, AvailableKinds.Count())];
    }

    List<Kind> ReturnAllKindsOnBoard()
    {
        AvailableKinds.Clear();

        for (int i = 1; i < KindsCount; i++)
        {
            foreach (Block b in BlocksOnBoard)
            {
                if (b.kind == (Kind)i)
                {
                    AvailableKinds.Add((Kind)i);
                    break;
                }
            }
        }
        return AvailableKinds;
    }

    #region Deciding whether block can be added
    public bool BlockCanBeAddedToBoard(Entity e)
    {
        Vector2 targetCoords = ReturnTargetCoordinates(e);
        return ThereIsPlaceForBlockToMove(e, targetCoords) && MoveIsNotMeaningless(targetCoords);
    }

    public bool ThereIsPlaceForBlockToMove(Entity entity, Vector2 targetCoords)
    {
        if ((entity.x == targetCoords.x) && (entity.y == targetCoords.y))
            return false;
        else return true;
    }

    bool MoveIsNotMeaningless(Vector2 targetCoords)
    {
        return !CoordsAreOutOfBoard(targetCoords);
    }

    public bool CoordsAreOutOfBoard(Vector2 coords)
    {
        if (coords.x == 0 || coords.x == width - 1 || coords.y == 0 || coords.y == height - 1)
            return true;
        else return false;
    }
    #endregion

    public List<BlockToMove> ReturnAllMovingBlocks(MovingBlocksOption option)
    {
        switch (option)
        {
            case MovingBlocksOption.Horizontal:
                return ReturnAllMovingBlocksInDimension(width, height, option);
            case MovingBlocksOption.Vertical:
                return ReturnAllMovingBlocksInDimension(height, width, option);
        }
        return null;
    }


    public List<BlockToMove> ReturnAllMovingBlocksInDimension(int searchedDimLength, int anotherDimLength, MovingBlocksOption option)
    {
        List<BlockToMove> BlocksToMoveInDimension = new List<BlockToMove>();

        for (int i = 1; i < anotherDimLength - 1; i++)
        {
            FindBlocksInAxis(ref BlocksToMoveInDimension, searchedDimLength, i, option);
        }
        return BlocksToMoveInDimension;
    }
    void FindBlocksInAxis(ref List<BlockToMove> BlocksToMoveInDimension, int searchedDimLength, int constAxis, MovingBlocksOption option)
    {
        List<BlockToMove> blocksToMoveInDim = new List<BlockToMove>();
        BlockToMove b = FindFreeBlockInAxis(searchedDimLength, constAxis, option);
        if (b != null)
        {
            blocksToMoveInDim.Add(b);
            FindAllBlocksInDimWithTheSameDirectionBehindBlock(b, ref blocksToMoveInDim, option);
            BlocksToMoveInDimension.AddRange(blocksToMoveInDim);
        }

        else return;

    }

    BlockToMove FindFreeBlockInAxis(int searchedDimLength, int constAxis, MovingBlocksOption option)
    {
        if (option == MovingBlocksOption.Horizontal)
        {
            for (int i = 1; i < searchedDimLength - 1; i++)
            {
                if (board[i, constAxis] is Block && ((board[i, constAxis].direction == Direction.West) || (board[i, constAxis].direction == Direction.East)))
                {
                    Block b = (Block)board[i, constAxis];
                    Vector2 targetCoords = ReturnTargetCoordinates2(b);
                    if (ThereIsPlaceForBlockToMove(b, targetCoords))
                    {
                        return new BlockToMove(b, targetCoords, CoordsAreOutOfBoard(targetCoords));
                    }
                }
            }
        }
        else if (option == MovingBlocksOption.Vertical)
        {
            for (int i = 1; i < searchedDimLength - 1; i++)
            {
                if (board[constAxis, i] is Block && ((board[constAxis, i].direction == Direction.North) || (board[constAxis, i].direction == Direction.South)))
                {
                    Block b = (Block)board[constAxis, i];
                    Vector2 targetCoords = ReturnTargetCoordinates2(b);
                    if (ThereIsPlaceForBlockToMove(b, targetCoords))
                    {
                        return new BlockToMove(b, targetCoords, CoordsAreOutOfBoard(targetCoords));
                    }

                }
            }
        }
        return null;
    }
    void FindAllBlocksInDimWithTheSameDirectionBehindBlock(BlockToMove b, ref List<BlockToMove> BlocksInRow, MovingBlocksOption option)
    {
        bool keepLooking = true;
        int dir = -1 * ((int)dict[b.Block.direction].x + (int)dict[b.Block.direction].y);

        int j = 1;

        if (option == MovingBlocksOption.Horizontal)
        {
            int constAxis = b.Block.y;

            while (keepLooking)
            {
                if (board[b.Block.x + dir * j, constAxis] is Block && board[b.Block.x + dir * j, constAxis].direction == b.Block.direction)
                {

                    BlockToMove nextBlockToMove = new BlockToMove((Block)board[b.Block.x + dir * j, constAxis]);

                    if (CoordsAreOutOfBoard(b.EndCoords))
                    {
                        nextBlockToMove.EndCoords = b.EndCoords;
                    }
                    else
                    {
                        nextBlockToMove.EndCoords += b.EndCoords + new Vector2(dir * j, 0);
                    }
                    nextBlockToMove.OutOfBoard = CoordsAreOutOfBoard(nextBlockToMove.EndCoords);
                    BlocksInRow.Add(nextBlockToMove);
                    j++;
                }
                else
                {
                    return;
                }
            }
            return;
        }

        else if (option == MovingBlocksOption.Vertical)
        {
            int constAxis = b.Block.x;
            while (keepLooking)
            {
                if (board[constAxis, b.Block.y + dir * j] is Block && board[constAxis, b.Block.y + dir * j].direction == b.Block.direction)
                {
                    BlockToMove nextBlockToMove = new BlockToMove((Block)board[constAxis, b.Block.y + dir * j]);
                    if (CoordsAreOutOfBoard(b.EndCoords))
                    {
                        nextBlockToMove.EndCoords = b.EndCoords;
                    }
                    else
                    {
                        nextBlockToMove.EndCoords += b.EndCoords + new Vector2(0, dir * j);
                    }

                    nextBlockToMove.OutOfBoard = CoordsAreOutOfBoard(nextBlockToMove.EndCoords);
                    BlocksInRow.Add(nextBlockToMove);
                    j++;
                }
                else
                {
                    return;
                }
            }
            return;
        }
    }

    public bool ThereAreBlocksWithFreeSpaces()
    {
        List<BlockToMove> horizontalBlocks = ReturnAllMovingBlocks(MovingBlocksOption.Horizontal);
        if (horizontalBlocks.Count != 0)
        {
            return true;
        }

        List<BlockToMove> verticalBlocks = ReturnAllMovingBlocks(MovingBlocksOption.Vertical);

        if (verticalBlocks.Count != 0)
        {
            return true;
        }

        return false;
    }




}

