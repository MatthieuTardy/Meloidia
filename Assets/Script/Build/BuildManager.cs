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
        interraction.constructChosen = Construction[index];
        interraction.PlayerIsBuildMode = true;
    }

    public void EndBuild()
    {
        interraction.PlayerIsBuildMode = false;
        isBuilding = false;
    }
}
