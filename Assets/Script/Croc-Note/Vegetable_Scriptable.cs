using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public abstract class Vegetable_Scriptable : ScriptableObject
{
    [SerializeField]private string nom;

    [SerializeField] private int price;


    public string Nom => nom;
    public int Price => price;

}
