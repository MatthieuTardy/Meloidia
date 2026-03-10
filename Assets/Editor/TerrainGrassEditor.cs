using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGrass))]
public class TerrainGrassEditor : Editor {

    private bool isPainting = false;
    private bool isErasing = false;
    private Vector3 brushPosition;
    private bool hasBrushPosition;

    private bool isMouseDown = false;
    private float paintInterval = 0.05f;
    private float lastPaintTime;

    private GUIStyle headerStyle;

    private void InitStyles() {
        if (headerStyle == null) {
            headerStyle = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.4f, 0.85f, 0.3f) }
            };
        }
    }

    // ==========================================
    // INSPECTOR
    // ==========================================

    public override void OnInspectorGUI() {
        InitStyles();
        TerrainGrass grass = (TerrainGrass)target;

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("🌿 Grass Painter", headerStyle);
        EditorGUILayout.Space(5);

        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        // Mode peinture
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Mode de peinture", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = isPainting ? new Color(0.3f, 1f, 0.3f) : Color.white;
        if (GUILayout.Button(isPainting ? "✅ PEINTURE ACTIVE" : "🖌 Peindre", GUILayout.Height(35))) {
            isPainting = !isPainting;
            if (isPainting) isErasing = false;
            ActiveEditorTracker.sharedTracker.isLocked = isPainting || isErasing;
            SceneView.RepaintAll();
        }

        GUI.backgroundColor = isErasing ? new Color(1f, 0.3f, 0.3f) : Color.white;
        if (GUILayout.Button(isErasing ? "✅ GOMME ACTIVE" : "🧹 Gomme", GUILayout.Height(35))) {
            isErasing = !isErasing;
            if (isErasing) isPainting = false;
            ActiveEditorTracker.sharedTracker.isLocked = isPainting || isErasing;
            SceneView.RepaintAll();
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        if (isPainting || isErasing) {
            EditorGUILayout.Space(3);
            EditorGUILayout.HelpBox(
                (isPainting ? "Mode PEINTURE" : "Mode GOMME") + " activé !\n\n" +
                "• Clic gauche dans la Scene View pour peindre/effacer\n" +
                "• Maintiens le clic pour peindre en continu\n" +
                "• [ et ] pour changer la taille du brush\n" +
                "• Échap pour désactiver",
                isPainting ? MessageType.Info : MessageType.Warning
            );
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // Statistiques
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📊 Statistiques", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Brins d'herbe total : {grass.GrassCount:N0}");
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        // Actions
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
        if (GUILayout.Button("🗑 Tout supprimer", GUILayout.Height(30))) {
            if (EditorUtility.DisplayDialog(
                "Supprimer toute l'herbe ?",
                $"Tu vas supprimer {grass.GrassCount:N0} brins d'herbe.",
                "Supprimer", "Annuler"
            )) {
                Undo.RecordObject(grass, "Clear All Grass");
                grass.ClearAllGrass();
                EditorUtility.SetDirty(grass);
            }
        }

        GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
        if (GUILayout.Button("🔄 Reconstruire", GUILayout.Height(30))) {
            grass.ForceRebuild();
            EditorUtility.SetDirty(grass);
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    // ==========================================
    // SCENE VIEW
    // ==========================================

    void OnEnable() {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable() {
        SceneView.duringSceneGui -= OnSceneGUI;
        ActiveEditorTracker.sharedTracker.isLocked = false;
    }

    void OnSceneGUI(SceneView sceneView) {
        TerrainGrass grass = (TerrainGrass)target;
        if (grass == null) return;

        // ===== TOUJOURS rafraîchir l'herbe visible dans l'éditeur =====
        if (grass.GrassCount > 0 && sceneView.camera != null) {
            grass.EditorUpdate(sceneView.camera);
        }

        if (!isPainting && !isErasing) return;
        if (grass.terrain == null) return;

        Event e = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        // Raccourcis
        if (e.type == EventType.KeyDown) {
            if (e.keyCode == KeyCode.Escape) {
                isPainting = false;
                isErasing = false;
                ActiveEditorTracker.sharedTracker.isLocked = false;
                Repaint();
                e.Use();
                return;
            }
            if (e.keyCode == KeyCode.LeftBracket) {
                grass.brushSize = Mathf.Max(1f, grass.brushSize - 2f);
                EditorUtility.SetDirty(grass);
                Repaint();
                e.Use();
            }
            if (e.keyCode == KeyCode.RightBracket) {
                grass.brushSize = Mathf.Min(50f, grass.brushSize + 2f);
                EditorUtility.SetDirty(grass);
                Repaint();
                e.Use();
            }
        }

        // Raycast
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        hasBrushPosition = false;

        if (grass.terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity)) {
            brushPosition = hit.point;
            hasBrushPosition = true;
        }

        // Dessiner le brush
        if (hasBrushPosition) {
            Color brushColor = isPainting
                ? new Color(0f, 1f, 0f, 0.3f)
                : new Color(1f, 0f, 0f, 0.3f);

            Color outlineColor = isPainting
                ? new Color(0f, 1f, 0f, 0.8f)
                : new Color(1f, 0f, 0f, 0.8f);

            Handles.color = brushColor;
            Handles.DrawSolidDisc(brushPosition, Vector3.up, grass.brushSize);

            Handles.color = outlineColor;
            Handles.DrawWireDisc(brushPosition, Vector3.up, grass.brushSize, 2f);

            Handles.color = Color.white;
            Handles.DrawSolidDisc(brushPosition, Vector3.up, 0.2f);

            Handles.Label(
                brushPosition + Vector3.up * 3f,
                isPainting ? $"🖌 Brush: {grass.brushSize:F0}" : $"🧹 Eraser: {grass.brushSize:F0}",
                new GUIStyle(EditorStyles.boldLabel) {
                    normal = { textColor = Color.white },
                    fontSize = 12
                }
            );
        }

        // Clic
        if (e.type == EventType.MouseDown && e.button == 0 && hasBrushPosition) {
            isMouseDown = true;
            DoPaintOrErase(grass);
            e.Use();
        }

        if (e.type == EventType.MouseDrag && e.button == 0 && hasBrushPosition && isMouseDown) {
            if (EditorApplication.timeSinceStartup - lastPaintTime > paintInterval) {
                DoPaintOrErase(grass);
                lastPaintTime = (float)EditorApplication.timeSinceStartup;
            }
            e.Use();
        }

        if (e.type == EventType.MouseUp && e.button == 0) {
            isMouseDown = false;
            e.Use();
        }

        sceneView.Repaint();
    }

    void DoPaintOrErase(TerrainGrass grass) {
        Undo.RecordObject(grass, isPainting ? "Paint Grass" : "Erase Grass");

        if (isPainting)
            grass.PaintGrass(brushPosition);
        else if (isErasing)
            grass.EraseGrass(brushPosition);

        EditorUtility.SetDirty(grass);
    }
}