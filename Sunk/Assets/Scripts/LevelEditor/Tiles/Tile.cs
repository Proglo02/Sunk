using System;
using UnityEngine;

public enum TileType
{
    Default,
    Ramp,
    Corner,
}

public enum TileDirection
{
    North,
    East,
    South,
    West
}

[Serializable]
[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    [SerializeField] private TileType tileType;
    [SerializeField] private TileDirection tileDirection;
}
