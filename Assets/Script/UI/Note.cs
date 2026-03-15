using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Note : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    public int ID;
    public Sprite itemIcon;

    [Tooltip("La couleur de la note lorsqu'elle est survolée ou sélectionnée.")]
    public Color highlightColor = Color.black;

    [Header("Juice Settings")]
    [Tooltip("L'objet enfant qui contient les visuels à animer. Si laissé vide, essaiera de trouver un enfant nommé 'Visuals'.")]
    public Transform visualsTransform; // Référence à l'enfant contenant les visuels

    [Tooltip("Le facteur de grossissement de la note au clic.")]
    public float scaleFactor = 1.1f;

    [Tooltip("La durée de l'animation de grossissement.")]
    public float scaleDuration = 0.1f;

    
    private static Image itemImage;
    private static Note selectedItem;

    private Graphic graphic;
    private Color originalColor;
    private Coroutine highlightCoroutine;
    private Vector3 originalScale;

    void Awake()
    {
        
        graphic = GetComponent<Graphic>();
        if (graphic != null)
        {
            originalColor = graphic.color;
        }

        // Si visualsTransform n'est pas assigné, on cherche un enfant nommé "Visuals"
        if (visualsTransform == null)
        {
            visualsTransform = transform.Find("Visuals");
        }

        if (visualsTransform != null)
        {
            originalScale = visualsTransform.localScale;
        }
        else
        {
            // Fallback si on ne trouve pas d'enfant : on anime l'objet lui-même
            //Debug.LogWarning("Aucun 'visualsTransform' assigné ou enfant 'Visuals' trouvé. L'animation de scale pourrait ne pas fonctionner avec un Layout Group.", this);
            visualsTransform = transform;
            originalScale = visualsTransform.localScale;
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
        if (Input.GetButton("ValidateNote"))
        {
            GameManager.Instance.playerManager.noteSystem.PlayNoteFromPC(ID);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedItem != this)
        {
            StopHighlightImmediate();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Joue l'animation de "juice" à chaque clic
        StartCoroutine(ScaleJuiceCoroutine());
        if (selectedItem == this)
        {
            DeselectAll();
        }
        else
        {
            SelectItem();
            
        }
    }

    private void SelectItem()
    {
        if (selectedItem != null && selectedItem != this)
        {
            selectedItem.StopHighlightImmediate();
        }

        selectedItem = this;
        Highlight();
    }

    public static void DeselectAll()
    {
        if (selectedItem != null)
        {
            selectedItem.StopHighlightImmediate();
            selectedItem = null;
        }
    }

    public void Highlight()
    {
        if (this.isActiveAndEnabled)
        {
            if (highlightCoroutine != null) StopCoroutine(highlightCoroutine);
            if (graphic != null) graphic.color = originalColor;
            highlightCoroutine = StartCoroutine(HighlightCoroutine());
        }
    }

    private IEnumerator HighlightCoroutine()
    {
        if (graphic == null) yield break;
        graphic.color = highlightColor;
        yield return new WaitForSeconds(0.2f);
        graphic.color = originalColor;
        highlightCoroutine = null;
    }

    public void StopHighlightImmediate()
    {
        if (highlightCoroutine != null) StopCoroutine(highlightCoroutine);
        if (graphic != null) graphic.color = originalColor;
    }

    private IEnumerator ScaleJuiceCoroutine()
    {
        if (visualsTransform == null) yield break;

        Vector3 targetScale = originalScale * scaleFactor;
        float elapsedTime = 0f;

        // Grossir
        while (elapsedTime < scaleDuration)
        {
            visualsTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        visualsTransform.localScale = targetScale;

        elapsedTime = 0f;

        // Rétrécir
        while (elapsedTime < scaleDuration)
        {
            visualsTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        visualsTransform.localScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.Instance.playerManager.noteSystem.PlayNoteFromPC(ID);
    }
    
}