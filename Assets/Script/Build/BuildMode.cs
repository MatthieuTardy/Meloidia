using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : MonoBehaviour
{
    [SerializeField] GameObject Wheel;
    GameObject Clone;

    [SerializeField] BuildInterraction interraction;
    ConstructionScriptable[] Construction;
    
    /// <summary>
    ///  a faire :  mettre ce script en manager + referencé sur le GameManager
    ///             afficher la preview
    ///             + faire les trucs a faire dans la preview
    /// </summary>

    private void Start()
    {
        FindAnyObjectByType<PlayerController>().OnBuildMode += WhenBuildModeStarts;
    }

    void WhenBuildModeStarts()
    {
        Debug.Log("Build Started");
        InstantiateWheel();
    }

    void InstantiateWheel()
    {
        Clone = Instantiate(Wheel);

    }

    public void ChangeSelectedBuild(int index)
    {
        interraction.constructChosen = Construction[0];
       // interraction.constructChosen = Construction[index];
    }
}
