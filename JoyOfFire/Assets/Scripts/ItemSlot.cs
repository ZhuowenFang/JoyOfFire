using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySlotSprite;

    public Text itemNumber;
    [SerializeField] private Image itemIcon;

    public Image itemDesciptionImage;
    public Text itemDescriptionText;
    public Text itemNameText;
    public Text itemDesciptionNumber;
    public GameObject selectedShader;
    public bool isSelected;

    private InventoryManager inventoryManager;
    private int itemCount;

    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
        itemNumber.text = "";
    }

    public void AddItem(string itemName, Sprite itemImage, string itemDescription)
    {
        if (isFull && this.itemName == itemName)
        {
            itemCount++;
            UpdateItemNumber();
        }
        else
        {
            this.itemSprite = itemImage;
            this.itemName = itemName;
            this.itemDescription = itemDescription;
            isFull = true;
            itemCount = 1;
            itemIcon.sprite = itemImage;
            UpdateItemNumber();
        }
    }

    public void RemoveItem()
    {
        itemCount--;
        if (itemCount <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateItemNumber();
        }
    }

    private void UpdateItemNumber()
    {
        if (itemCount < 2)
        {
            itemNumber.text = "";
        }
        else
        {
            itemNumber.text = itemCount.ToString();
        }
    }

    public void ClearSlot()
    {
        itemName = "";
        itemSprite = null;
        itemDescription = "";
        isFull = false;
        itemCount = 0;
        itemIcon.sprite = emptySlotSprite;
        itemNumber.text = "";
        InventoryManager.instance.DeselectAll();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        InventoryManager.instance.DeselectAll();
        selectedShader.SetActive(true);
        isSelected = true;
        itemNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDesciptionImage.sprite = itemSprite;
        itemDesciptionNumber.text = itemNumber.text;
        if (itemDesciptionImage.sprite == null)
        {
            itemDesciptionImage.sprite = emptySlotSprite;
        }
    }

    
}
