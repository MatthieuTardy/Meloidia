
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInterraction : MonoBehaviour
{
    public bool PlayerIsBuildMode;
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
        else
        {
            if(construct != null)
            {
                Destroy(construct);
            }
        }
    }
    private void Start()
    {
        
    }
    #region Build Preview
    void PrevisualizeConstruct()
    {
        if (construct != null)
        {
            construct.transform.position = SetOnGrid(Ratio);
            construct.transform.rotation = Quaternion.identity;
        }

    }

    public void InstantiatePreViewBuild()
    {
        if (construct == null && PlayerIsBuildMode)
        {
            construct = Instantiate(constructChosen.MeshPrefab, constructionSpawn.position, Quaternion.identity, constructionSpawn);
            previewInstantiate = true;
        }
        else if(construct != null && !PlayerIsBuildMode)
        {
            previewInstantiate = false;
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
        if (construct != null)
        {
            Vector3 halfExtents = (construct.transform.localScale * 0.5f);
            bool blocked = Physics.CheckBox(construct.transform.position, halfExtents, construct.transform.rotation);
            if (blocked)
            {
                return false;
            }
        }
            return true;


    }

    void Build()
    {
        Vector3 buildPos = new Vector3(construct.transform.position.x, floorPosition, construct.transform.position.z);
        Instantiate(construct, buildPos,construct.transform.rotation);
        GameManager.Instance.buildManager.EndBuild();
    }

    void ChangeMat(bool good)
    {
        if (construct != null)
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
}
