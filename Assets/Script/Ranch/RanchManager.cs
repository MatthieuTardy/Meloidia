using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanchManager : MonoBehaviour
{
    // si des crocs Notes sont dans le ranch
    // on les ajoutes dans une listes -> liste utilisé par les conditions
    // 
    public static RanchManager instance;
    
    public RanchRessourcesGenerator RanchGenerator;
    
    public List<LegumeManager> CrocNotesInRanch;
    private void Start()
    {
        if (instance != null) 
        {
            Destroy(instance);
        }
        RanchManager.instance = this;
        CrocNotesInRanch = new List<LegumeManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7)//legume
        {
            AddingCrocNote(other.GetComponent<LegumeManager>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)//legume
        {
           DeleteCrocNote(other.GetComponent<LegumeManager>());
        }
    }


    void AddingCrocNote(LegumeManager CN)
    {
        CrocNotesInRanch.Add(CN);
        RanchGenerator.UpdateList(CN);
    }

    void DeleteCrocNote(LegumeManager CN)
    {
        CrocNotesInRanch.Remove(CN);
    }

    


}
