using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North,
    South,
    West,
    East,
    None
}

[System.Serializable]
public class Entity
{

    public string slug = "empty3";
    public int x;
    public int y;
    public Dictionary<Direction, string> dict = new Dictionary<Direction, string>
        {
           { Direction.North," ↑"},
           {Direction.South," ↓"},
           {Direction.West,"←"},
           {Direction.East,"→"},
        };

    public Direction direction;

    public Entity(int x, int y, Direction dir = Direction.None, string slug = "background")
    {
        this.direction = dir;
        this.x = x;
        this.y = y;
        this.slug = slug;
    }

    public Vector2 GetPos()
    {
        return new Vector2(x, y);
    }

    public override string ToString()
    {
        return " 0 ";
    }
}


public class BoardButton : Entity
{
    public BoardButton(Direction dir, int x, int y) : base(x, y, dir)
    {
        slug = "btn_" + dict[direction];
    }

    public override string ToString()
    {
        return "b" + dict[direction];
    }
}
