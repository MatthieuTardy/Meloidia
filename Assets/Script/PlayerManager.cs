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

    void Start()
    {
        essenceMagique = 0; // On commence à 0
    }

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
}