using System;
using UnityEditor;

[CustomEditor(typeof(SubTile))]
public class SubTileEditor : Editor
{
    private SubTile subTile;

    private void OnEnable()
    {
        subTile = (SubTile)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        DrawType();
        DrawSubType();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawType()
    {
        TileType type = subTile.TileType;
        subTile.TileType = (TileType)EditorGUILayout.EnumPopup("Type", (TileType)(object)subTile.TileType);

        if (type != subTile.TileType)
            subTile.SubType = 0;
    }

    private void DrawSubType()
    {
        switch (subTile.TileType)
        {
            case TileType.Default:
                subTile.SubType = 0;
                break;
            case TileType.Slope:
                subTile.SubType = DrawEnum<SubTileTypeSlope>();
                break;
        }
    }

    private int DrawEnum<T>() where T : System.Enum
    {
        if (!Enum.IsDefined(typeof(T), subTile.SubType))
            subTile.SubType = 0;

        return Convert.ToInt32(EditorGUILayout.EnumPopup("SubType", (T)(object)subTile.SubType));
    }
}
