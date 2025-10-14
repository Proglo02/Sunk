using System;
using Unity.VisualScripting;
using UnityEditor;
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
        var tileDirectionProp = property.FindPropertyRelative("TileDirection");

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

        TileDirection tileDirection = (TileDirection)tileDirectionProp.enumValueIndex;
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), tileDirectionProp);

        if (tileDirection != (TileDirection)tileDirectionProp.enumValueIndex)
            OnRotationChanged(property, (TileDirection)tileDirectionProp.enumValueIndex);

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
        var subTileObjectProp = property.serializedObject.FindProperty("SubTileObject");
        UnityEngine.Object[] targets = property.serializedObject.targetObjects;

        foreach (var target in targets)
        {
            TileObject tileObject = target.GetComponent<TileObject>();
            tileObject.OnTileTypeChanged(tileType, subTileType);
        }
    }

    private void OnRotationChanged(SerializedProperty property, TileDirection direction)
    {
        var subTileObjectProp = property.serializedObject.FindProperty("SubTileObject");
        UnityEngine.Object[] targets = property.serializedObject.targetObjects;

        foreach (var target in targets)
        {
            TileObject tileObject = target.GetComponent<TileObject>();
            tileObject.OnRotationChanged(direction);
        }
    }
}
