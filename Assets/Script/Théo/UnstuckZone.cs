using UnityEngine;

public class UnstuckZone : MonoBehaviour
{
    public string nomDuLayerJoueur = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(nomDuLayerJoueur))
        {
            PlayerUnstuck playerScript = other.GetComponent<PlayerUnstuck>();

            if (playerScript != null && transform.childCount > 0)
            {
                playerScript.pointDeSecoursParDefaut = transform.GetChild(0);
            }
        }
    }
}