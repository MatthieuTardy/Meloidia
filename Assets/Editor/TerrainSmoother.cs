using UnityEngine;
using UnityEditor;

public class TerrainSmoother : EditorWindow
{
    public Terrain terrainToSmooth;
    public int blurRadius = 1;
    public int iterations = 1;
    [Range(0.1f, 1f)]
    public float forceLissage = 0.5f;
    [Range(0.0001f, 0.05f)]
    public float seuilTolerance = 0.002f;

    [MenuItem("Tools/Smooth Terrain")]
    public static void ShowWindow()
    {
        GetWindow<TerrainSmoother>("Terrain Smoother");
    }

    void OnGUI()
    {
        GUILayout.Label("Lissage Intelligent (Smart Smooth)", EditorStyles.boldLabel);

        terrainToSmooth = (Terrain)EditorGUILayout.ObjectField("Terrain", terrainToSmooth, typeof(Terrain), true);

        GUILayout.Space(5);
        blurRadius = EditorGUILayout.IntSlider("Rayon", blurRadius, 1, 3);
        iterations = EditorGUILayout.IntSlider("Passes", iterations, 1, 5);
        forceLissage = EditorGUILayout.Slider("Force", forceLissage, 0.1f, 1f);

        GUILayout.Space(5);
        GUILayout.Label("Protection de la hauteur :");
        seuilTolerance = EditorGUILayout.Slider("Tolerance (Seuil)", seuilTolerance, 0.0001f, 0.05f);

        GUILayout.Space(10);

        if (GUILayout.Button("Appliquer le lissage"))
        {
            if (terrainToSmooth != null)
            {
                SmoothTerrain(terrainToSmooth);
            }
        }
    }

    void SmoothTerrain(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int res = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, res, res);

        Undo.RegisterCompleteObjectUndo(terrainData, "Smooth Terrain");

        for (int iter = 0; iter < iterations; iter++)
        {
            float[,] tempHeights = heights.Clone() as float[,];

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float sum = 0;
                    int count = 0;

                    for (int ny = -blurRadius; ny <= blurRadius; ny++)
                    {
                        for (int nx = -blurRadius; nx <= blurRadius; nx++)
                        {
                            int currentX = x + nx;
                            int currentY = y + ny;

                            if (currentX >= 0 && currentX < res && currentY >= 0 && currentY < res)
                            {
                                sum += tempHeights[currentY, currentX];
                                count++;
                            }
                        }
                    }

                    float averageHeight = sum / count;
                    float difference = Mathf.Abs(tempHeights[y, x] - averageHeight);

                    if (difference > seuilTolerance)
                    {
                        heights[y, x] = Mathf.Lerp(tempHeights[y, x], averageHeight, forceLissage);
                    }
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
        terrain.Flush();
    }
}