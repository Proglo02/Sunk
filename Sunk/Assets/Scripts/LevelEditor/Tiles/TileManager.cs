using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] private SubTileLibrary subTileLibrary;

    public static GameObject GetSubTilePrefab(TileType tileType, int subtype)
    {
        SubTile subTile = Instance.subTileLibrary.GetSubTile(tileType, subtype);
        if (subTile != null)
        {
            return subTile.subTilePrefab;
        }
        else
        {
            Debug.LogWarning($"No SubTile found for TileType {tileType} and Subtype {subtype}.");
            return null;
        }
    }
}
