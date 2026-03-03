using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject MenuCanva;
    bool Open;
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
                Open = !Open;
                MenuCanva.SetActive(Open);
        }
    }
}
