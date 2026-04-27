using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

[CustomEditor(typeof(NavPainter))]
public class NavPainterEditor : Editor
{
    private NavPainter painter;
    private bool isPainting = false;

    private void OnEnable()
    {
        painter = (NavPainter)target;

        if (painter.volumeContainer == null)
        {
            GameObject container = new GameObject("Painted_NavVolumes");
            container.transform.SetParent(painter.transform);
            painter.volumeContainer = container.transform;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(15);

        GUI.backgroundColor = isPainting ? Color.green : Color.white;

        if (GUILayout.Button(isPainting ? "Stop Painting" : "Start Painting", GUILayout.Height(40)))
        {
            isPainting = !isPainting;
            Tools.current = UnityEditor.Tool.View;
        }

        GUI.backgroundColor = Color.white;

        if (isPainting)
        {
            EditorGUILayout.HelpBox("Left Click & Drag in the Scene View to paint unwalkable areas. Press Ctrl+Z to undo strokes.", MessageType.Info);
        }
    }

    private void OnSceneGUI()
    {
        if (!isPainting) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Handles.color = painter.brushColor;
            Handles.DrawSolidDisc(hit.point, hit.normal, painter.brushSize);
            Handles.DrawWireDisc(hit.point, hit.normal, painter.brushSize);

            if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
            {
                PaintVolume(hit.point);
                e.Use();
            }
        }

        SceneView.RepaintAll();
    }

    private void PaintVolume(Vector3 position)
    {
        Collider[] overlaps = Physics.OverlapSphere(position, painter.brushSize * 0.4f);
        foreach (Collider col in overlaps)
        {
            if (col.transform.parent == painter.volumeContainer) return;
        }

        GameObject volume = GameObject.CreatePrimitive(PrimitiveType.Cube);
        volume.name = "NavModifier";

        volume.transform.position = position + (Vector3.up * 0.5f);
        volume.transform.localScale = new Vector3(painter.brushSize, 2f, painter.brushSize);
        volume.transform.SetParent(painter.volumeContainer);

        DestroyImmediate(volume.GetComponent<MeshRenderer>());
        DestroyImmediate(volume.GetComponent<MeshFilter>());
        DestroyImmediate(volume.GetComponent<BoxCollider>());

        NavMeshModifierVolume modifier = volume.AddComponent<NavMeshModifierVolume>();
        modifier.area = 1;

        Undo.RegisterCreatedObjectUndo(volume, "Paint Nav Volume");
    }
}