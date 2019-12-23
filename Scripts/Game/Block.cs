using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Kind
{
    blue = 1,
    red = 2,
    yellow = 3,
    green = 4,
    joker = 5
}


[System.Serializable]
public class Block : Entity
{


    public Kind kind;
    public int ID;
    public Block(Direction dir, Kind kind, int x, int y) : base(x, y, dir)
    {
        this.kind = kind;
        slug = kind + "_" + dict[direction];
    }

    public override string ToString()
    {
        return ((int)kind + dict[direction]);
    }
}
