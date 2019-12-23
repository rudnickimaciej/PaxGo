using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "Level")]

[System.Serializable]
public class Level : ScriptableObject
{
    public int id = 0;
    public int Moves = 20;
    public int MaxMovesForOneStar = 20;
    public int MaxMovesForTwoStars;
    public int MaxMovesForThreeStars;

    [Header("Board")]
    public TextAsset blocksJson;
    [Range(1, 8)] public int BlocksWidth = 4;
    [Range(1, 8)] public int BlockHeight = 4;

    [Header("UI")]
    public Sprite BackgroundImage;
    public Color BackgroundColor;
    public List<Block> ReturnBlocks()
    {
        List<Block> blocksTotReturn = new List<Block>();
        List<Block> blocks = JsonUtility.FromJson<LevelEditor.Blocks>(blocksJson.ToString()).blocks;
        foreach (Block b in blocks)
        {
            // Kind k = b.kind;
            // b.kind = k;

            // Direction d = b.direction;
            // b.direction = d;

            blocksTotReturn.Add(new Block(b.direction, b.kind, b.x, b.y));
        }
        return blocksTotReturn;
    }
}
