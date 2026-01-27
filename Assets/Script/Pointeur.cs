using UnityEngine;

public class PointeurVersCible : MonoBehaviour
{

    public GameObject fleche;


    // La méthode Update est appelée à chaque image.
    void Update()
    {
        // 1. Vérifie si une cible a été assignée.
        if (GameManager.Instance.playerManager.tutoSelect != null)
        {
            transform.LookAt(GameManager.Instance.playerManager.tutoSelect);
        }
        if (GameManager.Instance.playerManager.indexTuto == 1)
        {
            fleche.SetActive(false);
        }
    }
}