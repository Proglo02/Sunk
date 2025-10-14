using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TileObject))]
public class TileObjectEditor : Editor
{
    private struct TileMoveInfo
    {
        public TileObject TileObject;
        public Transform Transform;
        public Vector3 Position;
        public Vector3 LastPosition;
        public int YAngle;
    }

    private TileManager tileManager;
    private TileMoveInfo[] moveInfos;
    private Vector3 moveDirection;

    private Vector3 savedMove;
    private float savedRotate;
    private Vector3 savedGridSize;
    private bool savedGridEnabled;

    private bool wasUndoRedo = false;

    private void OnEnable()
    {
        tileManager = FindObjectOfType<TileManager>();

        if (!tileManager)
        {
            tileManager = new TileManager();
            tileManager.tag = "TileManager";
            tileManager.name = "TileManager";
        }

        if (EditorUtility.IsPersistent(Selection.activeObject) && target.IsPrefabInstance())
        {
            Transform transform = target.GetComponent<Transform>();

            if (!transform.parent)
                transform.parent = tileManager.transform;
        }

        int length = targets.Length;
        moveInfos = new TileMoveInfo[length];

        for(int i = 0; i < length; i++)
        {
            moveInfos[i].TileObject = targets[i].GetComponent<TileObject>();
            moveInfos[i].Transform = targets[i].GetComponent<Transform>();
            moveInfos[i].Position = moveInfos[i].Transform.position;
            moveInfos[i].LastPosition = moveInfos[i].Position;
            moveInfos[i].YAngle = (int)moveInfos[i].Transform.localRotation.y;

            moveInfos[i].Transform.hideFlags = HideFlags.NotEditable;
        }

        savedMove = EditorSnapSettings.move;
        savedGridSize = EditorSnapSettings.gridSize;
        savedRotate = EditorSnapSettings.rotate;
        savedGridEnabled = EditorSnapSettings.gridSnapEnabled;

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
        EditorSnapSettings.move = savedMove;
        EditorSnapSettings.rotate = savedRotate;
        EditorSnapSettings.gridSize = savedGridSize;
        EditorSnapSettings.gridSnapEnabled = savedGridEnabled;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        EditorSnapSettings.move = new Vector3(1f, 0.5f, 1f);
        EditorSnapSettings.rotate = 45;
        EditorSnapSettings.gridSize = new Vector3(1f, 0.5f, 1f);
        EditorSnapSettings.gridSnapEnabled = true;

        Tools.pivotMode = PivotMode.Pivot;

        if (Tools.current != Tool.Move)
            Tools.current = Tool.Move;

        Event currentEvent = Event.current;

        if (moveInfos[0].Transform.position.y < 0)
        {
            for (int i = 0; i < moveInfos.Length; i++)
            {
                Vector3 position = moveInfos[i].LastPosition;
                position.y = Mathf.Max(0, position.y);
                moveInfos[i].Transform.position = position;
            }
        }

        if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            moveDirection = Vector3.zero;

        if (moveInfos[0].LastPosition != moveInfos[0].Position)
        {
            for (int i = 0; i < moveInfos.Length; i++)
            {
                moveInfos[i].Transform.position = moveInfos[i].Position;
                moveInfos[i].LastPosition = moveInfos[i].Position;
                EditorUtility.SetDirty(moveInfos[i].TileObject);
            }
        }
        else if (moveInfos[0].LastPosition != moveInfos[0].Transform.position)
        {
            if (!currentEvent.control)
            {
                for (int i = 0; i < moveInfos.Length; i++)
                {
                    moveInfos[i].LastPosition = moveInfos[i].Transform.position;
                    moveInfos[i].Position = moveInfos[i].LastPosition;
                    EditorUtility.SetDirty(moveInfos[i].TileObject);
                }
                return;
            }
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

        if (CollisionCheck() || moveInfos[0].Transform.position.y < 0)
        {
            for(int i = 0; i < moveInfos.Length; i++)
                moveInfos[i].Transform.position = moveInfos[i].LastPosition;
            return;
        }

        for (int i = 0; i < moveInfos.Length; i++)
        {
            int numTiles = (int)(moveInfos[i].Transform.position - moveInfos[i].LastPosition).magnitude;
            Vector3 direction = (moveInfos[moveInfos.Length - 1].Transform.position - moveInfos[moveInfos.Length - 1].LastPosition).normalized;

            if (direction.y != 0)
            {
                moveInfos[i].Transform.position = moveInfos[i].LastPosition + new Vector3(0f, .5f * direction.y, 0f);

                Vector3 scale = moveInfos[i].Transform.localScale;
                scale.y += direction.y;
                scale.y = Mathf.Max(1f, scale.y);

                if (moveInfos[i].Transform.childCount > 1)
                {
                    GameObject subTileObject = moveInfos[i].Transform.GetChild(1).gameObject;
                    subTileObject.transform.localScale = new Vector3(.5f, .5f / scale.y, .5f);
                }

                moveInfos[i].Transform.localScale = scale;

                moveInfos[i].LastPosition = moveInfos[i].Transform.position;
                moveInfos[i].Position = moveInfos[i].LastPosition;
                EditorUtility.SetDirty(moveInfos[i].TileObject);
            }
            else
            {
                if (moveDirection.magnitude == 0)
                    moveDirection = direction;
                else if (moveDirection != direction)
                {
                    moveInfos[i].Transform.position = moveInfos[i].LastPosition;
                    return;
                }

                CreateTile(moveInfos[i].TileObject, moveInfos[i].LastPosition, moveInfos[i].Transform.rotation);

                if (numTiles > 1)
                    moveInfos[i].Transform.position = moveInfos[i].LastPosition + direction;

                moveInfos[i].LastPosition = moveInfos[i].Transform.position;
                moveInfos[i].Position = moveInfos[i].LastPosition;
                EditorUtility.SetDirty(moveInfos[i].TileObject);
            }
        }
    }

    private void OnUndoRedo()
    {
        for(int i = 0; i < moveInfos.Length; i++)
            moveInfos[i].Transform.hideFlags = HideFlags.NotEditable;

        if (moveInfos[0].LastPosition.magnitude == moveInfos[0].Transform.position.magnitude)
            return;

        Event e = Event.current;

        if (e.type == EventType.KeyDown)
            if(e.keyCode == KeyCode.Z)
                OnUndo();
            //else if(e.keyCode == KeyCode.Y)
                //OnRedo();
    }

    private void OnUndo()
    {
        for(int i = 0; i < moveInfos.Length; i++)
        {
            int numTiles = (int)(moveInfos[i].Transform.position - moveInfos[i].LastPosition).magnitude;
            Vector3 direction = (moveInfos[moveInfos.Length - 1].Transform.position - moveInfos[moveInfos.Length - 1].LastPosition).normalized;

            for (int j = 0; j <= numTiles; j++)
            {
                RaycastHit[] hits = Physics.BoxCastAll(moveInfos[i].Transform.position - direction * j, new Vector3(.125f, moveInfos[i].Transform.localScale.y * .5f, .125f), -direction, Quaternion.identity, .25f);

                foreach (var hit in hits)
                {
                    if (hit.collider && hit.collider.CompareTag("Tile") && hit.transform != moveInfos[i].Transform.GetChild(0))
                    {
                        DestroyImmediate(hit.transform.parent.gameObject);
                        break;
                    }
                }
            }

            moveInfos[i].LastPosition = moveInfos[i].Transform.position;
            moveInfos[i].Position = moveInfos[i].LastPosition;
            EditorUtility.SetDirty((TileObject)target);
        }
    }

    private void OnRedo()
    {
        for (int i = 0; i < moveInfos.Length; i++)
        {
            int numTiles = (int)(moveInfos[i].Transform.position - moveInfos[i].LastPosition).magnitude;
            Vector3 direction = (moveInfos[moveInfos.Length - 1].Transform.position - moveInfos[moveInfos.Length - 1].LastPosition).normalized;

            for (int j = 0; j <= numTiles; j++)
            {
                CreateTile(moveInfos[i].TileObject, moveInfos[i].Transform.position - direction * j, moveInfos[i].Transform.rotation);
            }

            moveInfos[i].LastPosition = moveInfos[i].Transform.position;
            moveInfos[i].Position = moveInfos[i].LastPosition;
            EditorUtility.SetDirty(moveInfos[i].TileObject);
        }
    }

    private bool CollisionCheck()
    {
        float distance = Vector3.Distance(moveInfos[0].LastPosition, moveInfos[0].Transform.position);
        if (distance > 1)
            return true;

        for(int i = 0; i < moveInfos.Length; i++)
        {
            for (int j = 0; j < moveInfos.Length; j++)
            {
                if (moveInfos[i].Transform.position == moveInfos[j].LastPosition)
                    return true;
            }
        }

        foreach (var moveInfo in moveInfos)
        {
            Vector3 startPos = moveInfo.LastPosition + (moveInfo.Transform.position - moveInfo.LastPosition).normalized;

            RaycastHit[] hits = Physics.BoxCastAll(startPos, new Vector3(.125f, moveInfo.Transform.localScale.y * .5f, .125f), (moveInfo.Transform.position - moveInfo.LastPosition).normalized, Quaternion.identity, .25f);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Tile") && hit.collider.transform != moveInfo.Transform.GetChild(0))
                    return true;
            }
        }

        return false;
    }

    private void CreateTile(Object target, Vector3 position, Quaternion rotation)
    {
        Object obj = Instantiate(target, position, rotation);
        obj.name = "Tile";
        obj.GetComponent<Transform>().parent = tileManager.transform;
        obj.GetComponent<TileObject>().OnCreate();
    }
}
