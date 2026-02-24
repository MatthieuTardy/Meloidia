using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocNoteProximity : MonoBehaviour
{
    public List<GameObject> CrocNoteInProximity;

    private void Awake()
    {
        CrocNoteInProximity = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            CrocNoteInProximity.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            CrocNoteInProximity.Remove(other.gameObject);
        }
    }
}