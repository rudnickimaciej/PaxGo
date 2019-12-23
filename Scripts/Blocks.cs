using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    [System.Serializable]
    public class Blocks
    {
        public Blocks(List<Block> blocks)
        {
            this.blocks = blocks;
        }
        public List<Block> blocks;
    }
}