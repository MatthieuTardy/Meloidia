using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocNoteProximity : MonoBehaviour
{
    public List<LegumeManager> CrocNoteInProximity;

    private void Awake()
    {
        CrocNoteInProximity = new List<LegumeManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            CrocNoteInProximity.Add(other.gameObject.GetComponent<LegumeManager>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            CrocNoteInProximity.Remove(other.gameObject.GetComponent<LegumeManager>());
        }
    }
}