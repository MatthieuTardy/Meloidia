
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInterraction : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] public bool PlayerIsBuildMode;
    [SerializeField] KeyCode buildKey;

    bool previewInstantiate;

    [Header("Construction Setting")]
    [Range(1,10)][SerializeField] int Ratio;
    [SerializeField] float floorPosition;

    public ConstructionScriptable constructChosen;
    GameObject construct;
    [SerializeField] Transform constructionSpawn;
    Transform Spawn;

    [SerializeField] Material goodMat;
    [SerializeField] Material wrongMat;


    /// <summary>
    /// A faire:    
    ///             Changer le material de la preview selon "CanBuild()".
    ///             mettre le bon material pour le build
    /// </summary>
    /// 
    private void Update()
    {
        if (PlayerIsBuildMode)
        {
            InstantiatePreViewBuild();
            PrevisualizeConstruct();
            ChangeMat(CanBuild());
            if (Input.GetButtonDown("Fire1"))
            {
                TryToBuild();
            }
        }
    }
    private void Start()
    {
        
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
            construct = Instantiate(constructChosen.MeshPrefab, constructionSpawn.position, Quaternion.identity, constructionSpawn);
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
            Build();
        }
    }

    bool CanBuild()
    {
        Vector3 halfExtents = (construct.transform.localScale * 0.5f);
        bool blocked = Physics.CheckBox(construct.transform.position,halfExtents,construct.transform.rotation);
        if (blocked)
        {
            return false;
        }
        return true;
    }

    void Build()
    {
        Vector3 buildPos = new Vector3(construct.transform.position.x, floorPosition, construct.transform.position.z);
        Instantiate(construct, buildPos,construct.transform.rotation);
        
    }

    void ChangeMat(bool good)
    {
        Debug.Log(good);
        if (good)
        {
            construct.GetComponent<MeshRenderer>().material = goodMat;
        }
        else
        {
            construct.GetComponent<MeshRenderer>().material = wrongMat;
        }
    }
}
