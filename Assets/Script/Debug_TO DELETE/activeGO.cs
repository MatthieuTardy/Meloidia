using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeGO : MonoBehaviour
{
    [SerializeField] GameObject mailBar;
    [SerializeField] GameObject mailButton;
    // Start is called before the first frame update
    void Start()
    {
        mailBar.SetActive(false);
        mailButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {

                mailBar.SetActive(!mailBar.activeSelf);
                mailButton.SetActive(!mailButton.activeSelf);

        }
    }
}
