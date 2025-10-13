using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class TileObject : MonoBehaviour
{
    [SerializeReference] public Tile Tile = new Tile();

    [HideInInspector] public Vector3 Position;
}
