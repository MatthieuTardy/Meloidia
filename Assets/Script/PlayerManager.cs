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
    public int terre;
    public int terreMax = 1;
    public int eau;
    public int eauMax = 1;
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

    // Start is called before the first frame update
    void Start()
    {
        essenceMagique = 0; // On commence à 0
    }

    // Update is called once per frame
    void Update()
    {

    }
}