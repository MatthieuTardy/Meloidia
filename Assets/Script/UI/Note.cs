using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class Note : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public string itemName;
    public Sprite itemIcon;

    private string itemNameText = "Name";
    private string itemPreview = "Selected";

    private Text itemText;
    private Image itemImage;

    private static Note selectedItem;
    // Start is called before the first frame update
    void Start()
    {

        if (!string.IsNullOrEmpty(itemNameText))
        {
            GameObject textObject = GameObject.Find(itemNameText);

            if (textObject != null)
            {                 
                itemText = textObject.GetComponent<Text>();
            }
            else
            {
                Debug.Log("Item Name Text not found with name: " + itemNameText);
            }

        }

        if (!string.IsNullOrEmpty(itemPreview))
        {
            GameObject imageObject = GameObject.Find(itemPreview);
            if (imageObject != null)
            {
                itemImage = imageObject.GetComponent<Image>();
            }
            else
            {
                Debug.Log("No Preview Image found");
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemText != null)
        {
            itemText.text = itemName;
        }

        if(itemImage != null)
        {
            itemImage.sprite = itemIcon;
            itemImage.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedItem != null)
        {
            if (itemText != null)
            {
                itemText.text = selectedItem.itemName;
            }
            if (itemImage != null)
            {
                itemImage.sprite = selectedItem.itemIcon;
                itemImage.enabled = true;

            }
            else
            {
                if (itemText != null)
                {
                    itemText.text = "";
                }

                if (itemImage != null)
                {
                    itemImage.enabled = false;
                }

            }


        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectItem();
    }

    private void SelectItem()
    {
        if(selectedItem != null)
        {
            selectedItem.DeselectItem();
        }

        selectedItem = this;

        if(itemText != null)
        {
            itemText.text = itemName;
        }
        if(itemImage != null)
        {
            itemImage.sprite = itemIcon;
            itemImage.enabled = true;
        }
    }

    private void DeselectItem()
    {
        if(selectedItem == this)
        {
            if(itemText != null)
            {
                itemText.text = "";
            }
            if(itemImage != null)
            {
                itemImage.enabled = false;
            }
        }
    }

}
