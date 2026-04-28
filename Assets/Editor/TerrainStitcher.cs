using UnityEngine;
using UnityEditor;

public class TerrainStitcher : EditorWindow
{
    public Terrain terrain1;
    public Terrain terrain2;
    public Terrain terrain3;
    public Terrain terrain4;

    public float snapTolerance = 5f;
    public int blendWidth = 4;
    public int smoothIterations = 3;

    [MenuItem("Tools/Stitch Terrains")]
    public static void ShowWindow()
    {
        GetWindow<TerrainStitcher>("Terrain Stitcher");
    }

    void OnGUI()
    {
        GUILayout.Label("Stitch & Snap Terrain Borders", EditorStyles.boldLabel);

        terrain1 = (Terrain)EditorGUILayout.ObjectField("Terrain 1", terrain1, typeof(Terrain), true);
        terrain2 = (Terrain)EditorGUILayout.ObjectField("Terrain 2", terrain2, typeof(Terrain), true);
        terrain3 = (Terrain)EditorGUILayout.ObjectField("Terrain 3", terrain3, typeof(Terrain), true);
        terrain4 = (Terrain)EditorGUILayout.ObjectField("Terrain 4", terrain4, typeof(Terrain), true);

        GUILayout.Space(5);
        snapTolerance = EditorGUILayout.FloatField("Tolerance de distance", snapTolerance);
        blendWidth = EditorGUILayout.IntSlider("Largeur de lissage", blendWidth, 1, 20);
        smoothIterations = EditorGUILayout.IntSlider("Passes de lissage (Smooth)", smoothIterations, 0, 10);

        GUILayout.Space(10);

        if (GUILayout.Button("Stitch Terrains"))
        {
            Terrain[] terrains = { terrain1, terrain2, terrain3, terrain4 };

            for (int i = 0; i < terrains.Length; i++)
            {
                for (int j = i + 1; j < terrains.Length; j++)
                {
                    if (terrains[i] != null && terrains[j] != null)
                    {
                        StitchPair(terrains[i], terrains[j]);
                    }
                }
            }
        }
    }

    void StitchPair(Terrain t1, Terrain t2)
    {
        TerrainData d1 = t1.terrainData;
        TerrainData d2 = t2.terrainData;

        if (d1.heightmapResolution != d2.heightmapResolution) return;

        int res = d1.heightmapResolution;

        float[,] h1 = d1.GetHeights(0, 0, res, res);
        float[,] h2 = d2.GetHeights(0, 0, res, res);

        Vector3 pos1 = t1.transform.position;
        Vector3 pos2 = t2.transform.position;
        Vector3 diff = pos2 - pos1;

        bool isStitched = false;

        Undo.RegisterCompleteObjectUndo(d1, "Stitch Terrain");
        Undo.RegisterCompleteObjectUndo(d2, "Stitch Terrain");
        Undo.RecordObject(t1.transform, "Snap Terrain");
        Undo.RecordObject(t2.transform, "Snap Terrain");

        float sizeY1 = d1.size.y;
        float sizeY2 = d2.size.y;

        if (Mathf.Abs(diff.x - d1.size.x) < snapTolerance)
        {
            pos2.x = pos1.x + d1.size.x; pos2.z = pos1.z; t2.transform.position = pos2;
            t1.SetNeighbors(t1.leftNeighbor, t1.topNeighbor, t2, t1.bottomNeighbor);
            t2.SetNeighbors(t1, t2.topNeighbor, t2.rightNeighbor, t2.bottomNeighbor);

            for (int z = 0; z < res; z++)
            {
                float worldY1 = pos1.y + (h1[z, res - 1] * sizeY1);
                float worldY2 = pos2.y + (h2[z, 0] * sizeY2);
                float avgY = (worldY1 + worldY2) / 2f;

                for (int w = 0; w < blendWidth; w++)
                {
                    float t = (float)w / blendWidth;
                    float curY1 = pos1.y + (h1[z, res - 1 - w] * sizeY1);
                    float curY2 = pos2.y + (h2[z, w] * sizeY2);

                    h1[z, res - 1 - w] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[z, w] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            if (smoothIterations > 0) SmoothEdge(h1, h2, res, pos1.y, pos2.y, sizeY1, sizeY2, true);
            isStitched = true;
        }
        else if (Mathf.Abs(diff.x + d2.size.x) < snapTolerance)
        {
            pos1.x = pos2.x + d2.size.x; pos1.z = pos2.z; t1.transform.position = pos1;
            t2.SetNeighbors(t2.leftNeighbor, t2.topNeighbor, t1, t2.bottomNeighbor);
            t1.SetNeighbors(t2, t1.topNeighbor, t1.rightNeighbor, t1.bottomNeighbor);

            for (int z = 0; z < res; z++)
            {
                float worldY1 = pos1.y + (h1[z, 0] * sizeY1);
                float worldY2 = pos2.y + (h2[z, res - 1] * sizeY2);
                float avgY = (worldY1 + worldY2) / 2f;

                for (int w = 0; w < blendWidth; w++)
                {
                    float t = (float)w / blendWidth;
                    float curY1 = pos1.y + (h1[z, w] * sizeY1);
                    float curY2 = pos2.y + (h2[z, res - 1 - w] * sizeY2);

                    h1[z, w] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[z, res - 1 - w] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            if (smoothIterations > 0) SmoothEdge(h2, h1, res, pos2.y, pos1.y, sizeY2, sizeY1, true);
            isStitched = true;
        }
        else if (Mathf.Abs(diff.z - d1.size.z) < snapTolerance)
        {
            pos2.z = pos1.z + d1.size.z; pos2.x = pos1.x; t2.transform.position = pos2;
            t1.SetNeighbors(t1.leftNeighbor, t2, t1.rightNeighbor, t1.bottomNeighbor);
            t2.SetNeighbors(t2.leftNeighbor, t2.topNeighbor, t2.rightNeighbor, t1);

            for (int x = 0; x < res; x++)
            {
                float worldY1 = pos1.y + (h1[res - 1, x] * sizeY1);
                float worldY2 = pos2.y + (h2[0, x] * sizeY2);
                float avgY = (worldY1 + worldY2) / 2f;

                for (int w = 0; w < blendWidth; w++)
                {
                    float t = (float)w / blendWidth;
                    float curY1 = pos1.y + (h1[res - 1 - w, x] * sizeY1);
                    float curY2 = pos2.y + (h2[w, x] * sizeY2);

                    h1[res - 1 - w, x] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[w, x] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            if (smoothIterations > 0) SmoothEdge(h1, h2, res, pos1.y, pos2.y, sizeY1, sizeY2, false);
            isStitched = true;
        }
        else if (Mathf.Abs(diff.z + d2.size.z) < snapTolerance)
        {
            pos1.z = pos2.z + d2.size.z; pos1.x = pos2.x; t1.transform.position = pos1;
            t2.SetNeighbors(t2.leftNeighbor, t1, t2.rightNeighbor, t2.bottomNeighbor);
            t1.SetNeighbors(t1.leftNeighbor, t1.topNeighbor, t1.rightNeighbor, t2);

            for (int x = 0; x < res; x++)
            {
                float worldY1 = pos1.y + (h1[0, x] * sizeY1);
                float worldY2 = pos2.y + (h2[res - 1, x] * sizeY2);
                float avgY = (worldY1 + worldY2) / 2f;

                for (int w = 0; w < blendWidth; w++)
                {
                    float t = (float)w / blendWidth;
                    float curY1 = pos1.y + (h1[w, x] * sizeY1);
                    float curY2 = pos2.y + (h2[res - 1 - w, x] * sizeY2);

                    h1[w, x] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[res - 1 - w, x] = Mathf.Clamp01((Mathf.SmoothStep(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            if (smoothIterations > 0) SmoothEdge(h2, h1, res, pos2.y, pos1.y, sizeY2, sizeY1, false);
            isStitched = true;
        }

        if (isStitched)
        {
            d1.SetHeights(0, 0, h1);
            d2.SetHeights(0, 0, h2);
            t1.Flush();
            t2.Flush();
        }
    }

    void SmoothEdge(float[,] hA, float[,] hB, int res, float posAy, float posBy, float sizeAy, float sizeBy, bool isXAxis)
    {
        int length = blendWidth * 2;
        float[] buffer = new float[length];
        float[] temp = new float[length];

        for (int i = 0; i < res; i++)
        {
            for (int w = 0; w < blendWidth; w++)
            {
                if (isXAxis)
                {
                    buffer[blendWidth - 1 - w] = posAy + hA[i, res - 1 - w] * sizeAy;
                    buffer[blendWidth + w] = posBy + hB[i, w] * sizeBy;
                }
                else
                {
                    buffer[blendWidth - 1 - w] = posAy + hA[res - 1 - w, i] * sizeAy;
                    buffer[blendWidth + w] = posBy + hB[w, i] * sizeBy;
                }
            }

            for (int iter = 0; iter < smoothIterations; iter++)
            {
                buffer.CopyTo(temp, 0);
                for (int j = 1; j < length - 1; j++)
                {
                    buffer[j] = (temp[j - 1] + temp[j] + temp[j + 1]) / 3f;
                }
            }

            for (int w = 0; w < blendWidth; w++)
            {
                if (isXAxis)
                {
                    hA[i, res - 1 - w] = Mathf.Clamp01((buffer[blendWidth - 1 - w] - posAy) / sizeAy);
                    hB[i, w] = Mathf.Clamp01((buffer[blendWidth + w] - posBy) / sizeBy);
                }
                else
                {
                    hA[res - 1 - w, i] = Mathf.Clamp01((buffer[blendWidth - 1 - w] - posAy) / sizeAy);
                    hB[w, i] = Mathf.Clamp01((buffer[blendWidth + w] - posBy) / sizeBy);
                }
            }
        }
    }
}