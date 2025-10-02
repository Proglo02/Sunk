using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileObject))]
public class TileObjectEditor : Editor
{
    Transform transform;
    SerializedProperty position;
    Vector3 lastPosition;

    private bool wasUndoRedo = false;

    private void OnEnable()
    {
        transform = target.GetComponent<Transform>();
        position = serializedObject.FindProperty("Position");

        lastPosition = position.vector3Value;

        Tools.current = Tool.Move;
        Tools.pivotMode = PivotMode.Pivot;

        transform.hideFlags = HideFlags.NotEditable;

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(position);

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        if (lastPosition != position.vector3Value)
        {
            transform.position = position.vector3Value;
            lastPosition = position.vector3Value;
            EditorUtility.SetDirty((TileObject)target);
        }
        else if (lastPosition != transform.position)
        {
            OnMove();
        }
    }

    private void OnMove()
    {
        if(wasUndoRedo)
        {
            wasUndoRedo = false;
            return;
        }

        if (CollisionCheck())
        {
            transform.position = lastPosition;
            return;
        }

        transform.position = Tools.handlePosition;

        int numTiles = (int)(transform.position - lastPosition).magnitude;
        Vector3 direction = (transform.position - lastPosition).normalized;

        CreateTile(lastPosition);

        if (numTiles > 1)
            transform.position = lastPosition + direction;

        lastPosition = transform.position;
        position.vector3Value = lastPosition;
        target.GetComponent<TileObject>().Position = lastPosition;
        EditorUtility.SetDirty((TileObject)target);
        Debug.Log("Moved");
    }

    private void OnUndoRedo()
    {
        wasUndoRedo = true;

        if (UndoCheck(out RaycastHit[] hits))
            OnUndo(hits);
        else
            OnRedo();
    }

    private void OnUndo(RaycastHit[] hits)
    {
        foreach (var hit in hits)
        {
            if (hit.collider && hit.collider.CompareTag("Tile") && hit.transform != transform.GetChild(0))
                DestroyImmediate(hit.transform.parent.gameObject);
        }

        lastPosition = transform.position;
        position.vector3Value = lastPosition;
        target.GetComponent<TileObject>().Position = lastPosition;
        EditorUtility.SetDirty((TileObject)target);

        Debug.Log("Undo");
    }

    private void OnRedo()
    {
        int numTiles = (int)(transform.position - lastPosition).magnitude;
        Vector3 direction = (transform.position - lastPosition).normalized;

        for(int i = 0; i <= numTiles; i++)
        {
            CreateTile(transform.position - direction * i);
        }

        lastPosition = transform.position;
        position.vector3Value = lastPosition;
        target.GetComponent<TileObject>().Position = lastPosition;
        EditorUtility.SetDirty((TileObject)target);
    }

    private bool CollisionCheck()
    {
        Vector3 startPos = lastPosition + (Tools.handlePosition - lastPosition).normalized;

        RaycastHit[] hits = Physics.BoxCastAll(startPos, new Vector3(.125f, .125f, .125f), (Tools.handlePosition - lastPosition).normalized, Quaternion.identity, .25f);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Tile") && hit.collider.transform != transform.GetChild(0))
                return true;
        }

        return false;
    }

    private bool UndoCheck(out RaycastHit[] hits)
    {
        Vector3 startPos = lastPosition;

        Vector3 direction = (lastPosition - transform.position).normalized;

        hits = Physics.BoxCastAll(startPos, new Vector3(.125f, .125f, .125f), -direction, Quaternion.identity);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Tile") && hit.transform != transform.GetChild(0))
            {
                return true;
            }
        }

        return false;
    }

    private void CreateTile(Vector3 position)
    {
        Object obj = Instantiate(target, position, Quaternion.identity);
        obj.name = "Tile";
    }
}
