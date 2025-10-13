using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubTileLibrary", menuName = "ScriptableObjects/Tiles/SubTileLibrary", order = 1)]
public class SubTileLibrary : ScriptableObject
{
    [SerializeField] private List<SubTile> subTileList;

    public Dictionary<TileType, Dictionary<int, SubTile>> subTileDictionary;

    private void OnValidate()
    {
        PopulateDictionary();
    }

    private void OnEnable()
    {
        PopulateDictionary();
    }

    public SubTile GetSubTile(TileType tileType, int subtype)
    {
        if (subTileDictionary != null && subTileDictionary.ContainsKey(tileType) && subTileDictionary[tileType].ContainsKey(subtype))
        {
            return subTileDictionary[tileType][subtype];
        }

        return null;
    }

    private void PopulateDictionary()
    {
        subTileDictionary = new Dictionary<TileType, Dictionary<int, SubTile>>();
        foreach (SubTile subTile in subTileList)
        {
            TileType tileType = subTile.TileType;
            int subtype = subTile.SubType;
            if (!subTileDictionary.ContainsKey(tileType))
            {
                subTileDictionary[tileType] = new Dictionary<int, SubTile>();
            }
            if (!subTileDictionary[tileType].ContainsKey(subtype))
            {
                subTileDictionary[tileType][subtype] = subTile;
            }
            else
            {
                Debug.LogWarning($"Duplicate SubTile for TileType {tileType} and Subtype {subtype} in SubTileLibrary.");
            }
        }
    }
}
