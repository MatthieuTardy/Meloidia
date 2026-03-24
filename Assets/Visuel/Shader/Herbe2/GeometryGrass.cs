using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGrass : MonoBehaviour {
    [Header("Terrain")]
    public Terrain terrain;

    [Header("Grass Settings")]
    public Material grassMaterial;

    [Header("Brush Settings")]
    [Range(1f, 50f)]
    public float brushSize = 10f;
    [Range(1, 30)]
    public int brushDensity = 10;
    [Range(0f, 1f)]
    public float randomOffset = 0.5f;

    [Header("Placement Filters")]
    [Range(0f, 90f)]
    public float maxSlopeAngle = 45f;

    [Header("Culling")]
    [Tooltip("Distance max de rendu d'un brin")]
    public float renderDistance = 150f;
    [Tooltip("Marge autour du frustum en degrés (évite le pop-in)")]
    [Range(0f, 20f)]
    public float frustumPadding = 5f;

    [Header("Debug")]
    public bool showDebugLogs = false;

    [HideInInspector]
    [SerializeField]
    private List<Vector3> allGrassPositions = new List<Vector3>();

    private Mesh grassMesh;
    private Plane[] frustumPlanes = new Plane[6];

    private List<Vector3> visibleVertices = new List<Vector3>();
    private List<int> visibleIndices = new List<int>();

    private Vector3 lastCamPos;
    private Quaternion lastCamRot;
    private int lastVisibleCount;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    void OnEnable() {
        CreateMesh();
        UpdateVisibleGrass();
    }

    void Awake() {
        CreateMesh();
    }

    void Start() {
        UpdateVisibleGrass();
    }

    void LateUpdate() {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (allGrassPositions.Count > 0 && (mf.sharedMesh == null || mf.sharedMesh.vertexCount == 0)) {
            CreateMesh();
        }

        if (!Application.isPlaying) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;
        Quaternion camRot = cam.transform.rotation;

        if (camPos != lastCamPos || camRot != lastCamRot) {
            lastCamPos = camPos;
            lastCamRot = camRot;
            UpdateVisibleGrassWithCamera(cam);
        }
    }

    Camera GetActiveCamera() {
        if (Application.isPlaying) {
            return Camera.main;
        }
        #if UNITY_EDITOR
        if (UnityEditor.SceneView.lastActiveSceneView != null)
            return UnityEditor.SceneView.lastActiveSceneView.camera;
        #endif
        return null;
    }

    // ==========================================
    // ÉDITEUR - appelé par TerrainGrassEditor
    // ==========================================

    public void EditorUpdate(Camera sceneCam) {
        if (sceneCam == null) return;
        if (allGrassPositions.Count == 0) return;

        Vector3 camPos = sceneCam.transform.position;
        Quaternion camRot = sceneCam.transform.rotation;

        if (camPos == lastCamPos && camRot == lastCamRot) return;

        lastCamPos = camPos;
        lastCamRot = camRot;

        UpdateVisibleGrassWithCamera(sceneCam);
    }

    // ==========================================
    // CULLING PAR BRIN
    // ==========================================

    void UpdateVisibleGrass() {
        Camera cam = GetActiveCamera();
        if (cam == null) return;
        UpdateVisibleGrassWithCamera(cam);
    }

    void UpdateVisibleGrassWithCamera(Camera cam) {
        if (grassMesh == null) CreateMesh();
        if (allGrassPositions.Count == 0) {
            ClearMesh();
            return;
        }

        Matrix4x4 projMatrix = cam.projectionMatrix;

        if (frustumPadding > 0f && !cam.orthographic) {
            float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
            float paddedFov = (cam.fieldOfView + frustumPadding * 2f) * Mathf.Deg2Rad;
            float scale = Mathf.Tan(fovRad * 0.5f) / Mathf.Tan(paddedFov * 0.5f);
            projMatrix[0, 0] *= scale;
            projMatrix[1, 1] *= scale;
        }

        GeometryUtility.CalculateFrustumPlanes(projMatrix * cam.worldToCameraMatrix, frustumPlanes);

        Vector3 camPos = cam.transform.position;
        float renderDistSq = renderDistance * renderDistance;

        visibleVertices.Clear();
        visibleIndices.Clear();

        for (int i = 0; i < allGrassPositions.Count; i++) {
            Vector3 worldPos = allGrassPositions[i];

            float dx = worldPos.x - camPos.x;
            float dy = worldPos.y - camPos.y;
            float dz = worldPos.z - camPos.z;
            float distSq = dx * dx + dy * dy + dz * dz;

            if (distSq > renderDistSq) continue;
            if (!IsPointInFrustum(worldPos)) continue;

            visibleVertices.Add(transform.InverseTransformPoint(worldPos));
            visibleIndices.Add(visibleVertices.Count - 1);
        }

        lastVisibleCount = visibleVertices.Count;

        grassMesh.Clear();

        if (visibleVertices.Count > 0) {
            grassMesh.SetVertices(visibleVertices);
            grassMesh.SetIndices(visibleIndices, MeshTopology.Points, 0);

            grassMesh.RecalculateBounds();
            Bounds b = grassMesh.bounds;
            b.Expand(new Vector3(20f, 30f, 20f));
            grassMesh.bounds = b;
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = visibleVertices.Count > 0;

        if (showDebugLogs && Time.frameCount % 60 == 0) {
            Debug.Log($"[Grass] Visible: {visibleVertices.Count}/{allGrassPositions.Count}");
        }
    }

    bool IsPointInFrustum(Vector3 point) {
        for (int i = 0; i < 6; i++) {
            if (frustumPlanes[i].GetDistanceToPoint(point) < 0f)
                return false;
        }
        return true;
    }

    // ==========================================
    // MESH
    // ==========================================

    void CreateMesh() {
        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf.sharedMesh != null && mf.sharedMesh.name == "PaintedGrass") {
            if (Application.isPlaying) Destroy(mf.sharedMesh);
            else DestroyImmediate(mf.sharedMesh);
        }

        grassMesh = new Mesh {
            name = "PaintedGrass",
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        grassMesh.MarkDynamic();

        mf.sharedMesh = grassMesh;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (grassMaterial != null)
            mr.sharedMaterial = grassMaterial;
    }

    void ClearMesh() {
        if (grassMesh != null)
            grassMesh.Clear();
    }

    // ==========================================
    // PEINTURE
    // ==========================================

    public void PaintGrass(Vector3 center) {
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrainData.size;

        float step = brushSize / Mathf.Sqrt(brushDensity * brushSize);

        for (float x = -brushSize; x < brushSize; x += step) {
            for (float z = -brushSize; z < brushSize; z += step) {
                float dist = Mathf.Sqrt(x * x + z * z);
                if (dist > brushSize) continue;

                float edgeFalloff = 1f - (dist / brushSize);
                if (Random.value > edgeFalloff) continue;

                float offsetX = Random.Range(-randomOffset * step, randomOffset * step);
                float offsetZ = Random.Range(-randomOffset * step, randomOffset * step);

                float worldX = center.x + x + offsetX;
                float worldZ = center.z + z + offsetZ;

                float localX = worldX - terrainPos.x;
                float localZ = worldZ - terrainPos.z;

                if (localX < 0 || localX > terrainSize.x || localZ < 0 || localZ > terrainSize.z)
                    continue;

                float normX = localX / terrainSize.x;
                float normZ = localZ / terrainSize.z;

                float steepness = terrainData.GetSteepness(normX, normZ);
                if (steepness > maxSlopeAngle) continue;

                float height = terrainData.GetInterpolatedHeight(normX, normZ);
                Vector3 pos = new Vector3(worldX, height + terrainPos.y, worldZ);

                if (!IsTooClose(pos, step * 0.5f)) {
                    allGrassPositions.Add(pos);
                }
            }
        }

        lastCamPos = Vector3.zero;
        UpdateVisibleGrass();
    }

    public void EraseGrass(Vector3 center) {
        float radiusSq = brushSize * brushSize;

        allGrassPositions.RemoveAll(pos => {
            float dx = pos.x - center.x;
            float dz = pos.z - center.z;
            return (dx * dx + dz * dz) < radiusSq;
        });

        lastCamPos = Vector3.zero;
        UpdateVisibleGrass();
    }

    bool IsTooClose(Vector3 pos, float minDist) {
        float minDistSq = minDist * minDist;
        int startIndex = Mathf.Max(0, allGrassPositions.Count - 500);
        for (int i = startIndex; i < allGrassPositions.Count; i++) {
            float dx = allGrassPositions[i].x - pos.x;
            float dz = allGrassPositions[i].z - pos.z;
            if (dx * dx + dz * dz < minDistSq)
                return true;
        }
        return false;
    }

    // ==========================================
    // UTILITAIRES
    // ==========================================

    public int GrassCount => allGrassPositions.Count;

    public void ClearAllGrass() {
        allGrassPositions.Clear();
        ClearMesh();
    }

    public void ForceRebuild() {
        CreateMesh();
        lastCamPos = Vector3.zero;
        UpdateVisibleGrass();
    }

    public void RebuildMesh() => ForceRebuild();

    void OnValidate() {
        #if UNITY_EDITOR
        if (allGrassPositions.Count > 0) {
            UnityEditor.EditorApplication.delayCall += () => {
                if (this != null) ForceRebuild();
            };
        }
        #endif
    }

    void OnDestroy() {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null && mf.sharedMesh.name == "PaintedGrass") {
            if (Application.isPlaying) Destroy(mf.sharedMesh);
            else DestroyImmediate(mf.sharedMesh);
        }
    }
}