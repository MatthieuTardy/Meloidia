using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelogumeSinging : MonoBehaviour
{
    // AVERTISSEMENT : Ces champs doivent être assignés dans l'Inspecteur d'Unity !
    public FMODUnity.StudioEventEmitter DO;
    public FMODUnity.StudioEventEmitter RE;
    public FMODUnity.StudioEventEmitter MI;

    private bool _isGameManagerReady = false;

    void Start()
    {
        // 1. Vérification des références essentielles au démarrage
        if (DO == null || RE == null || MI == null)
        {
            Debug.LogError("Attention : Les émetteurs FMOD (DO, RE, MI) ne sont pas assignés dans l'Inspecteur du GameObject " + gameObject.name + ". La chanson ne démarrera pas.");
            return; // Sortir si les émetteurs ne sont pas là
        }

        // 2. Vérification de la structure GameManager pour la sécurité
        if (GameManager.Instance != null && GameManager.Instance.deplacementAleatoire != null)
        {
            _isGameManagerReady = true;
        }

        // Démarrer la coroutine de la chanson
        StartCoroutine(SongOfHealing());
    }

    // Arrête tous les sons (vérification ajoutée)
    void StopChant()
    {
        // On n'appelle Stop() que si la référence n'est pas nulle
        if (DO != null) DO.Stop();
        if (RE != null) RE.Stop();
        if (MI != null) MI.Stop();
    }

    IEnumerator SongOfHealing()
    {
        // Gérer la vitesse uniquement si la référence GameManager est prête
        if (_isGameManagerReady)
        {
            GameManager.Instance.deplacementAleatoire.vitesse = 0;
        }

        // --- Séquence musicale ---

        DO.Play();
        yield return new WaitForSeconds(1);
        StopChant();

        RE.Play();
        yield return new WaitForSeconds(1);
        StopChant();

        DO.Play();
        yield return new WaitForSeconds(1);
        StopChant();

        MI.Play();
        yield return new WaitForSeconds(1);
        StopChant();

        // Rétablir la vitesse de déplacement (si la référence est toujours là)
        if (_isGameManagerReady)
        {
            GameManager.Instance.deplacementAleatoire.vitesse = 5;
        }

        // Attente aléatoire avant de répéter la chanson
        yield return new WaitForSeconds(Random.Range(3f, 10.0f));

        // Répéter la chanson
        StartCoroutine(SongOfHealing());
    }
}