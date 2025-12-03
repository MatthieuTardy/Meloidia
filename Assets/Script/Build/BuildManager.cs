using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [SerializeField] GameObject Wheel;
    GameObject Clone;

    [SerializeField] BuildInterraction interraction;
    [SerializeField] ConstructionScriptable[] Construction;

    public bool isBuilding = false;
    private void Start()
    {
        FindAnyObjectByType<PlayerController>().OnBuildMode += WhenBuildModeStarts;
    }

    void WhenBuildModeStarts()
    {
        if (!isBuilding)
        {
            Debug.Log("Build Started");
            InstantiateWheel();
            isBuilding = true;
        }
        else
        {
            if(Clone != null)
            {
                Destroy(Clone);
            }
            EndBuild();
            isBuilding = false;
        }
    }

    void InstantiateWheel()
    {
        Clone = Instantiate(Wheel);

    }

    public void ChangeSelectedBuild(int index)
    {
        interraction.constructChosen = Construction[0];
       // interraction.constructChosen = Construction[index]; // a mettre plus tard
        interraction.PlayerIsBuildMode = true;
    }

    public void EndBuild()
    {
        interraction.PlayerIsBuildMode = false;
        isBuilding = false;
    }
}
