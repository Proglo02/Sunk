using UnityEditor;
using UnityEngine;

[SelectionBase]
public class TileObject : MonoBehaviour
{
    [SerializeReference] public Tile Tile = new Tile();

    private GameObject subTileObject;

    public void OnCreate()
    {
        if (transform.childCount > 1)
        {
            subTileObject = transform.GetChild(1).gameObject;
            subTileObject.transform.localScale = new Vector3(.5f, .5f / transform.localScale.y, .5f);
        }
    }

    public void OnTileTypeChanged(TileType tileType, int subTileType)
    {
        if (subTileObject != null)
        {
            GameObject.DestroyImmediate(subTileObject);
            subTileObject = null;
        }

        GameObject prefab = TileManager.GetSubTilePrefab(tileType, subTileType);

        if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
            instance.transform.rotation = Quaternion.Euler(0, ((int)Tile.TileDirection) * 90f, 0);
            subTileObject = instance;
        }
    }

    public void OnRotationChanged(TileDirection direction)
    {
        if (subTileObject)
            subTileObject.transform.rotation = Quaternion.Euler(0, ((int)direction) * 90f, 0);
    }
}