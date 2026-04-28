using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class NavMeshObstacleBatchTool
{
    [MenuItem("Tools/NavMesh/Add Carving Obstacles to Selected")]
    public static void AddCarvingObstacles()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Veuillez sélectionner au moins un GameObject dans la scène.");
            return;
        }

        int addedOrModifiedCount = 0;

        foreach (GameObject go in selectedObjects)
        {
            BoxCollider boxCollider = go.GetComponent<BoxCollider>();

            if (boxCollider == null)
            {
                continue;
            }

            NavMeshObstacle obstacle = go.GetComponent<NavMeshObstacle>();

            if (obstacle == null)
            {
                obstacle = Undo.AddComponent<NavMeshObstacle>(go);
            }
            else
            {
                Undo.RecordObject(obstacle, "Update NavMeshObstacle properties");
            }

            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = boxCollider.center;
            obstacle.size = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z + 1f);
            obstacle.carving = true;

            addedOrModifiedCount++;
        }

        Debug.Log($"Terminé : NavMesh Obstacle (Carve) appliqué et ajusté sur {addedOrModifiedCount} GameObject(s).");
    }
}