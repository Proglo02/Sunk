using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SubTile", menuName = "ScriptableObjects/Tiles/SubTile", order = 1)]
public class SubTile : ScriptableObject
{
    public GameObject subTilePrefab;

    [HideInInspector] public TileType TileType;
    [HideInInspector] public int SubType;
}
