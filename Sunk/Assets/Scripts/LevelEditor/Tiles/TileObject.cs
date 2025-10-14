using UnityEngine;

[SelectionBase]
public class TileObject : MonoBehaviour
{
    [SerializeReference] public Tile Tile = new Tile();

    [HideInInspector] public GameObject SubTileObject;
    //[HideInInspector] public Vector3 Position;
}
