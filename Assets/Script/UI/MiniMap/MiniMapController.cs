using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] GameObject largeMap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ActiveMap();


    }

    void ActiveMap()
    {
        if (Input.GetButtonDown("Carte"))
        {
            largeMap.SetActive(!largeMap.activeSelf);
        }
    }
}
