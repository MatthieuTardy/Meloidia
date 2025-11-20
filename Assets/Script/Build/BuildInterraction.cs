using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInterraction : MonoBehaviour
{
    [Header("Debug")]


    [SerializeField] bool PlayerIsBuildMode;
    [SerializeField] KeyCode buildKey;
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
            if (Input.GetKeyDown(buildKey))
            {
                TryToBuild();
            }
        }
    }

    #region Build Preview
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
    #endregion

    void TryToBuild()
    {
        if (CanBuild())
        {
            Debug.Log("BUILDED");
        }
    }

        bool m_HitDetect;
        RaycastHit m_Hit;
    bool CanBuild()
    {

        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data

        m_HitDetect = Physics.BoxCast(Spawn.position,(Spawn.localScale*Ratio)*0.5f,Spawn.position,out m_Hit,Spawn.rotation,1);
        if (m_HitDetect)
        {
            //Output the name of the Collider your Box hit
            Debug.Log("Hit : " + m_Hit.collider.name);
        }
        return true;
    }
}
