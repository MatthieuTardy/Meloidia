using UnityEngine;
using UnityEditor;

public class TerrainStitcher : EditorWindow
{
    public Terrain terrain1;
    public Terrain terrain2;
    public float snapTolerance = 5f;
    public int blendWidth = 4;

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

        GUILayout.Space(5);
        snapTolerance = EditorGUILayout.FloatField("Tolerance de distance", snapTolerance);
        blendWidth = EditorGUILayout.IntSlider("Largeur de lissage", blendWidth, 1, 20);

        GUILayout.Space(10);

        if (GUILayout.Button("Stitch Terrains"))
        {
            if (terrain1 != null && terrain2 != null)
            {
                StitchTerrains(terrain1, terrain2);
            }
        }
    }

    void StitchTerrains(Terrain t1, Terrain t2)
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

        Undo.RegisterCompleteObjectUndo(d1, "Stitch Terrain 1");
        Undo.RegisterCompleteObjectUndo(d2, "Stitch Terrain 2");
        Undo.RecordObject(t1.transform, "Snap Terrain 1");
        Undo.RecordObject(t2.transform, "Snap Terrain 2");

        float sizeY1 = d1.size.y;
        float sizeY2 = d2.size.y;

        if (Mathf.Abs(diff.x - d1.size.x) < snapTolerance)
        {
            pos2.x = pos1.x + d1.size.x;
            pos2.z = pos1.z;
            t2.transform.position = pos2;

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

                    h1[z, res - 1 - w] = Mathf.Clamp01((Mathf.Lerp(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[z, w] = Mathf.Clamp01((Mathf.Lerp(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            isStitched = true;
        }
        else if (Mathf.Abs(diff.x + d2.size.x) < snapTolerance)
        {
            pos1.x = pos2.x + d2.size.x;
            pos1.z = pos2.z;
            t1.transform.position = pos1;

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

                    h1[z, w] = Mathf.Clamp01((Mathf.Lerp(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[z, res - 1 - w] = Mathf.Clamp01((Mathf.Lerp(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            isStitched = true;
        }
        else if (Mathf.Abs(diff.z - d1.size.z) < snapTolerance)
        {
            pos2.z = pos1.z + d1.size.z;
            pos2.x = pos1.x;
            t2.transform.position = pos2;

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

                    h1[res - 1 - w, x] = Mathf.Clamp01((Mathf.Lerp(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[w, x] = Mathf.Clamp01((Mathf.Lerp(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
            isStitched = true;
        }
        else if (Mathf.Abs(diff.z + d2.size.z) < snapTolerance)
        {
            pos1.z = pos2.z + d2.size.z;
            pos1.x = pos2.x;
            t1.transform.position = pos1;

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

                    h1[w, x] = Mathf.Clamp01((Mathf.Lerp(avgY, curY1, t) - pos1.y) / sizeY1);
                    h2[res - 1 - w, x] = Mathf.Clamp01((Mathf.Lerp(avgY, curY2, t) - pos2.y) / sizeY2);
                }
            }
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
}