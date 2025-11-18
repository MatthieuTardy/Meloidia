using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInterraction : MonoBehaviour
{
    [Header("Debug")]


    [SerializeField] bool PlayerIsBuildMode;
    bool previewInstantiate;
    [Range(1,10)][SerializeField] int Ratio;

    [SerializeField] ConstructionScriptable constructChosen;
    GameObject construct;
    [SerializeField] Transform constructionSpawn;
    Transform Spawn;
    private void Update()
    {
        if (PlayerIsBuildMode)
        {
            InstantiatePreViewBuild();
            PrevisualizeConstruct();
        }
    }

    void PrevisualizeConstruct()
    {
        construct.transform.position = SetOnGrid(Ratio);
        construct.transform.rotation = Quaternion.identity;
    }

    public void InstantiatePreViewBuild()
    {
        if (!previewInstantiate)
        {
            Spawn = constructionSpawn;
            construct = Instantiate(constructChosen.MeshPrefab, constructionSpawn.position, Quaternion.identity, Spawn);
            previewInstantiate = true;
        }
    }


    Vector3Int SetOnGrid(float gridSize)
    {
        Vector3 pos = constructionSpawn.position;

        return new Vector3Int(
            Mathf.RoundToInt(pos.x / gridSize) * (int)gridSize,
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z / gridSize) * (int)gridSize
        );
    }
}
