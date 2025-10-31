using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UiSelection : MonoBehaviour
{
    public Button[] buttons;
    public float radius = 200f;
    public Vector2 centerOffset = Vector2.zero;

    // Référence au root de la roue (par défaut le GameObject contenant ce script)
    // Si tu veux que la roue soit un enfant séparé, assigne-le ici dans l'inspecteur.
    public GameObject wheelRoot;

    // Seuil d'activation (même logique que NoteSystem pour détecter qu'on "joue" une note)
    public float activationThreshold = 0.5f;

    void Start()
    {
        if (wheelRoot == null)
            wheelRoot = this.gameObject;

       
        if (Application.isPlaying)
            UpdateWheelVisibility();
    }

    void Update()
    {
        UpdateWheelVisibility();
    }

   
   

    // Affiche la roue uniquement quand le joueur "joue" les notes (même conditions d'axes que dans NoteSystem)
    private void UpdateWheelVisibility()
    {
        if (wheelRoot == null) return;

        bool isPlayingInput = false;

        float sx = Input.GetAxis("SongX_Xbox");
        float sy = Input.GetAxis("SongY_Xbox");

        if (Mathf.Abs(sx) > activationThreshold || Mathf.Abs(sy) > activationThreshold)
        {
            isPlayingInput = true;
        }

        // Quand on passe de true -> false, on veut s'assurer de réinitialiser les notes du canvas
        bool wasActive = wheelRoot.activeSelf;
        if (wasActive != isPlayingInput)
        {
            wheelRoot.SetActive(isPlayingInput);

            if (!isPlayingInput)
            {
                // Le joueur a arrêté de "jouer" : reset des highlights et de la sélection des notes
                ResetAllNotesOnCanvas();
            }
        }
    }

    // Trouve toutes les Notes dans le canvas (ou sous le wheelRoot) et remet leur couleur d'origine.
    // On appelle aussi la méthode statique DeselectAll pour vider l'affichage.
    private void ResetAllNotesOnCanvas()
    {
        // Si wheelRoot contient des boutons Note, on les cherche sous wheelRoot.
        Note[] notes = wheelRoot.GetComponentsInChildren<Note>(true);

        // Si rien trouvé sous wheelRoot, on tente de trouver globalement (sécurité)
        if (notes == null || notes.Length == 0)
        {
            notes = FindObjectsOfType<Note>(true);
        }

        foreach (var note in notes)
        {
            if (note != null)
            {
                note.StopHighlightImmediate();
            }
        }

        // Efface la sélection verrouillée et l'affichage partagé
        Note.DeselectAll();
    }
}