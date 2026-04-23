using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonChangeColorText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color WhiteColor = Color.white;
    [SerializeField] Color BlackColor = Color.black;
    private void OnEnable()
    {
        text.color = BlackColor;
    }

    public void ChangeToWhite()
    {
        text.color = WhiteColor;
    }
}
