using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public int outils;
    public bool havingTools = false;

    public bool isBuildMode = false;

    public bool calme;
    int terre;
    [SerializeField] int terreMax = 1;
    int eau;
    [SerializeField] int eauMax = 1;
    public int essenceMagique; // La variable Max est supprimée

    public List<Transform> tuto;
    public int indexTuto = 0;
    public Transform tutoSelect;

    [Header("Outils")]
    public GameObject Gant;
    public GameObject Arrosoir;
    public GameObject Pelle;
    public GameObject Houe;
    public GameObject Marteau;

    public NoteSystem noteSystem;

    private Rigidbody body;
    public bool ragdoll = false;
    void Start()
    {
        body = GetComponent<Rigidbody>();
        essenceMagique = 0; // On commence à 0
    }

    public void Bump(Vector3 dir)
    {
        StartCoroutine(SetSingingStateRagdoll(0.5f));
        body.AddForce(new Vector3(dir.x, 0.2f, dir.z)*3, ForceMode.Impulse);
        
    }


    #region Use Get and Reload
    public void ReloadWater()
    {
        eau = eauMax;
    }

    public void UseWater()
    {
        eau--;
    }
    public int GetWater()
    {
        return eau;
    }

    public void ReloadDirt()
    {
        terre = terreMax;
    }

    public void UseDirt()
    {
        terre--;
    }
    public int GetDirt()
    {
        return terre;
    }

    #endregion
    [Button("test")] 
    public void Test()
    {
        StartCoroutine(SetSingingStateCalme(15));
    }
    public IEnumerator SetSingingStateCalme(float duration)
    {
        calme = true;
        yield return new WaitForSeconds(duration);
        calme = false;
    }
    public IEnumerator SetSingingStateRagdoll(float duration)
    {
        ragdoll = true;
        ///NOAHHHHHHHH ICI C'EST LA ICI ICI LA LA 
        yield return new WaitForSeconds(duration);
        ragdoll = false;
    }
}