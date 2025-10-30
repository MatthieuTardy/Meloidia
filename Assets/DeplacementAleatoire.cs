using System.Collections;
using UnityEngine;

// Assurez-vous d'avoir un Rigidbody sur ce GameObject
[RequireComponent(typeof(Rigidbody))]
public class DeplacementAleatoireRigidbody : MonoBehaviour
{
    // Durée du déplacement
    public float dureeDeplacement = 1f;
    // Intervalle entre les déplacements
    public float intervalleAttente = 5f;
    // Vitesse du personnage
    public float vitesse = 5f;
    // Vitesse de rotation (pour adoucir le changement de direction)
    public float vitesseRotation = 10f;

    public Rigidbody rb; // Référence au Rigidbody
    private Vector3 directionAleatoire;

    void Start()
    {
        rb.freezeRotation = true;

        // Lancement de la Coroutine
        StartCoroutine(GererDeplacement());
    }

    IEnumerator GererDeplacement()
    {
        while (true) // Boucle infinie pour répéter l'action
        {
            // 1. **Phase d'Attente (5 secondes)**
            // Arrêter tout mouvement du Rigidbody pendant l'attente
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(intervalleAttente);

            // 2. **Définition de la Direction Aléatoire**
            directionAleatoire = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

            if (directionAleatoire == Vector3.zero)
            {
                continue;
            }

            float tempsEcoule = 0f;

            // 3. **Phase de Déplacement (1 seconde)**
            while (tempsEcoule < dureeDeplacement)
            {
                // A. Déplacer le personnage en utilisant Rigidbody.MovePosition (si Is Kinematic est Vrai)
                // OU Rigidbody.velocity (si Is Kinematic est Faux et vous utilisez la physique)

                // Mouvement Kinematic recommandé pour le contrôle précis :
                Vector3 deplacement = directionAleatoire * vitesse * Time.deltaTime;
                rb.MovePosition(rb.position + deplacement);

                // B. Faire pivoter le personnage pour regarder dans sa direction

                // Calculer la rotation cible vers la nouvelle direction
                Quaternion rotationCible = Quaternion.LookRotation(directionAleatoire);

                // Appliquer la rotation progressivement avec Slerp
                rb.rotation = Quaternion.Slerp(
                    rb.rotation, // Utiliser rb.rotation pour l'état du Rigidbody
                    rotationCible,
                    Time.deltaTime * vitesseRotation
                );

                tempsEcoule += Time.deltaTime;
                yield return null; // Attendre la prochaine frame
            }
        }
    }
}