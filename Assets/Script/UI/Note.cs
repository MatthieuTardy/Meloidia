using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Note : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string itemName;
    public Sprite itemIcon;

    private const string ItemNameTextObjectName = "Name";
    private const string ItemPreviewImageObjectName = "Selected";

    // Les éléments d'UI sont statiques pour être partagés entre toutes les instances de Note
    private static Text itemText;
    private static Image itemImage;
    private static Note selectedItem;

    private Graphic graphic;
    private Color originalColor;
    private Coroutine highlightCoroutine;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
        if (graphic != null)
        {
            originalColor = graphic.color; // Sauvegarde de la couleur initiale
        }
    }

    void Start()
    {
        // On cherche les références de l'UI une seule fois pour optimiser
        if (itemText == null)
        {
            GameObject textObject = GameObject.Find(ItemNameTextObjectName);
            if (textObject != null) itemText = textObject.GetComponent<Text>();
            else Debug.LogWarning("Objet UI 'Name' non trouvé.");
        }

        if (itemImage == null)
        {
            GameObject imageObject = GameObject.Find(ItemPreviewImageObjectName);
            if (imageObject != null) itemImage = imageObject.GetComponent<Image>();
            else Debug.LogWarning("Objet UI 'Selected' non trouvé.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Affiche les infos de la note survolée
        UpdateDisplay(itemName, itemIcon, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Si une note est "verrouillée", on affiche ses infos, sinon on efface tout
        if (selectedItem != null)
        {
            UpdateDisplay(selectedItem.itemName, selectedItem.itemIcon, true);
        }
        else
        {
            ClearDisplay();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Si on clique sur une note déjà sélectionnée, on la déverrouille
        if (selectedItem == this)
        {
            DeselectAll();
        }
        else // Sinon, on la sélectionne (verrouille)
        {
            SelectItem();
        }
    }

    private void SelectItem()
    {
        selectedItem = this;
        UpdateDisplay(itemName, itemIcon, true);
    }

    // Méthode publique statique pour pouvoir la vider depuis n'importe où (ex: NoteSystem)
    public static void DeselectAll()
    {
        selectedItem = null;
        ClearDisplay();
    }

    private static void UpdateDisplay(string name, Sprite icon, bool isEnabled)
    {
        if (itemText != null)
        {
            itemText.text = name;
        }
        if (itemImage != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = isEnabled;
        }
    }

    private static void ClearDisplay()
    {
        UpdateDisplay("", null, false);
    }

    // Gère l'animation de surbrillance sans rester bloqué
    public void Highlight()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        if (graphic != null)
        {
            graphic.color = originalColor;
        }

        highlightCoroutine = StartCoroutine(HighlightCoroutine());
    }

    private IEnumerator HighlightCoroutine()
    {
        if (graphic == null) yield break;

        graphic.color = Color.yellow; // Couleur de surbrillance
        yield return new WaitForSeconds(0.2f); // Durée
        graphic.color = originalColor; // Retour à la normale
        highlightCoroutine = null;
    }
}