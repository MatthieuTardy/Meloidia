using UnityEngine;

public class InstantTeleportZone : MonoBehaviour
{
    public string nomDuLayerJoueur = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(nomDuLayerJoueur))
        {
            PlayerUnstuck playerScript = other.GetComponent<PlayerUnstuck>();

            if (playerScript != null)
            {
                playerScript.DeBloquerJoueur();
            }
        }
    }
}