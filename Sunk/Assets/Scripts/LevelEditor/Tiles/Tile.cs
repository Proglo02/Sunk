using System;
using UnityEngine;

[Serializable]
public enum TileType
{
    Default,
    Slope,
}

public enum SubTileTypeSlope
{
    Default,
    CornerTop,
    CornerMiddle,
    CornerBottom,
}

public enum TileDirection
{
    North,
    East,
    South,
    West
}

[Serializable]
public class Tile
{
    public TileType TileType;
    public int SubTileType;
    public TileDirection tileDirection;
}
