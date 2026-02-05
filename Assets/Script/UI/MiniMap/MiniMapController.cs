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
            if (Input.GetKey("m"))
            {
                largeMap.SetActive(true);
            }
            else
                largeMap.SetActive(false);
    }
}
