using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomPropertyDrawer(typeof(Tile))]
public class TileDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var tileTypeProp = property.FindPropertyRelative("TileType");
        var subTileTypeProp = property.FindPropertyRelative("SubTileType");
        var tileDirectionProp = property.FindPropertyRelative("tileDirection");

        var lineHeight = EditorGUIUtility.singleLineHeight + 2;
        var y = position.y;

        TileType tileType = (TileType)tileTypeProp.enumValueIndex;
        int subTileType = subTileTypeProp.intValue;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), tileTypeProp);
        y += lineHeight;

        subTileTypeProp.intValue = DrawSubType(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), (TileType)tileTypeProp.enumValueIndex, subTileTypeProp.intValue);
        if (tileTypeProp.intValue > 0)
            y += lineHeight;

        if (tileType != (TileType)tileTypeProp.enumValueIndex || subTileType != subTileTypeProp.intValue)
            OnTileTypeChanged(property, (TileType)tileTypeProp.enumValueIndex, subTileTypeProp.intValue);

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), tileDirectionProp);

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + 2) * (property.FindPropertyRelative("TileType").intValue > 0 ? 3 : 2);
    }

    private int DrawSubType(Rect position, TileType tileType, int subType)
    {
        switch (tileType)
        {
            case TileType.Default:
                return 0;
            case TileType.Slope:
                return DrawEnum<SubTileTypeSlope>(position, subType);
            default:
                return 0;
        }
    }

    private int DrawEnum<T>(Rect position, int subType) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), subType))
            return 0;

        T enumValue = (T)Enum.ToObject(typeof(T), subType);
        enumValue = (T)(object)EditorGUI.EnumPopup(position, "SubType", enumValue);
        return Convert.ToInt32(enumValue);
    }

    private void OnTileTypeChanged(SerializedProperty property, TileType tileType, int subTileType)
    {
        Tile tile = fieldInfo.GetValue(property.serializedObject.targetObject) as Tile;
        TileObject tileObject = property.serializedObject.targetObject as TileObject;

        GameObject prefab = TileManager.GetSubTilePrefab(tileType, subTileType);

        if (tile.SubTileObject != null && prefab == null)
            GameObject.DestroyImmediate(tile.SubTileObject);
        else if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab, tileObject.transform) as GameObject;
            tile.SubTileObject = instance;
        }

        EditorUtility.SetDirty(tileObject);
        EditorSceneManager.MarkSceneDirty(tileObject.gameObject.scene);
    }
}
