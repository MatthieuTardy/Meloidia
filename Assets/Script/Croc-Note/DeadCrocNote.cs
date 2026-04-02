using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DeadCrocNote : MonoBehaviour
{
    [SerializeField] GameObject NameBoard;
    public void Init(string name)
    {
        Debug.Log(name);
        NameBoard.GetComponent<TextMeshPro>().text = name;
    }

    private void Start()
    {
        NameBoard.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            NameBoard.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            NameBoard.SetActive(false);
        }
    }

    private void Update()
    {
        if (NameBoard.activeInHierarchy) 
        {
            NameBoard.transform.LookAt(GameManager.Instance.playerManager.Camera);
        }
    }
}
