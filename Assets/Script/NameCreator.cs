
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameCreator : MonoBehaviour
{
    

    public static string NewName()
    {
        string name = "";
        string[] syllabes = { "po", "mi", "ri", "mo","la","ba","si","zo","phy","tchi","mni","pi","chi","tcho", "ro","to","pe","re","le","a","o","poo","so","ma","ta"};


        int longeur = Random.Range(1, 4);
        for (int i = 0; i < longeur; i++)
        {
            int index = Random.Range(0, syllabes.Length);
            name += syllabes[index];
            name = name.ToUpper();
        }
            return name;

    }


}
