using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public int outils;
    public bool havingTools = false;

    public int terre;
    public int terreMax = 1;
    public int eau;
    public int eauMax = 1;

    public List<Transform> tuto;
    public int indexTuto=0;
    public Transform tutoSelect;

    [Header("Outils")]
    public GameObject Gant;
    public GameObject Arrosoir;
    public GameObject Pelle;
    public GameObject Houe;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
