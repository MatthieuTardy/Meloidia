using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGrass))]
public class TerrainGrassEditor : Editor
{
    // ==========================================
    // INSPECTEUR (UI)
    // ==========================================
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGrass grassScript = (TerrainGrass)target;

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Peinture & Validation", EditorStyles.boldLabel);

        if (grassScript.previewGrassPositions.Count > 0)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox($"{grassScript.previewGrassPositions.Count} brins en attente de validation.", MessageType.Warning);
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Appliquer la peinture", GUILayout.Height(30)))
            {
                grassScript.ApplyPreviewGrass();
                EditorUtility.SetDirty(grassScript); // Sauvegarde
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Annuler", GUILayout.Height(30)))
            {
                grassScript.CancelPreviewGrass();
                EditorUtility.SetDirty(grassScript); // Sauvegarde
                SceneView.RepaintAll();
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("Peignez sur le terrain avec Shift+Clic Gauche. Les boutons apparaîtront ici.", MessageType.Info);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField($"Total d'herbes générées : {grassScript.GrassCount}", EditorStyles.label);

        if (GUILayout.Button("Tout Effacer (Danger)", GUILayout.Height(20)))
        {
            if (EditorUtility.DisplayDialog("Confirmer", "Es-tu sûr de vouloir supprimer toute l'herbe de ce script ?", "Oui", "Non"))
            {
                grassScript.ClearAllGrass();
                EditorUtility.SetDirty(grassScript); // Sauvegarde
                SceneView.RepaintAll();
            }
        }
    }

    // ==========================================
    // VUE SCÈNE (PINCEAU)
    // ==========================================
    void OnSceneGUI()
    {
        TerrainGrass grassScript = (TerrainGrass)target;

        if (SceneView.currentDrawingSceneView != null)
        {
            grassScript.EditorUpdate(SceneView.currentDrawingSceneView.camera);
        }

        Event e = Event.current;

        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, grassScript.paintableLayer);

        RaycastHit terrainHit = new RaycastHit();
        bool foundTerrain = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<Terrain>() != null)
            {
                terrainHit = hit;
                foundTerrain = true;
                break;
            }
        }

        if (foundTerrain)
        {
            Handles.color = new Color(0, 1, 0, 0.1f);
            Handles.DrawSolidDisc(terrainHit.point, terrainHit.normal, grassScript.brushSize);
            Handles.color = Color.green;
            Handles.DrawWireDisc(terrainHit.point, terrainHit.normal, grassScript.brushSize);

            // PEINDRE (Shift + Clic gauche) -> Plus de lag de Undo !
            if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0 && e.shift)
            {
                grassScript.PaintGrass(terrainHit.point);
                e.Use();
            }

            // EFFACER (Ctrl + Clic gauche) -> Plus de lag de Undo !
            else if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0 && e.control)
            {
                grassScript.EraseGrass(terrainHit.point);
                e.Use();
            }
        }

        // On continue de sauvegarder la scène (pour de vrai) quand tu relâches le clic
        if (e.rawType == EventType.MouseUp && e.button == 0)
        {
            EditorUtility.SetDirty(grassScript);
        }

        if (e.type == EventType.MouseMove)
        {
            SceneView.RepaintAll();
        }
    }
}