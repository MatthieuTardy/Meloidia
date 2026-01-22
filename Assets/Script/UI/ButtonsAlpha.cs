using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonsAlpha : MonoBehaviour
{
    [SerializeField] Image[] images;

    void Start()
    {
        for(int i = 0; i < images.Length; i++)
        {
            images[i].alphaHitTestMinimumThreshold = 1f;
        }
    }

}
