using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Text itemSlotName;
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
    public int itemCount;

    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
        itemNumber.text = "";
    }

    public void AddItem(string itemName, Sprite itemImage, string itemDescription, int count)
    {
        if (isFull && itemSlotName.text == itemName)
        {
            Debug.LogError("Item already exists in slot");
            itemCount += count;
            UpdateItemNumber();
        }
        else
        {
            this.itemName = itemName;
            itemSprite = itemImage;
            itemSlotName.text = itemName;
            this.itemDescription = itemDescription;
            isFull = true;
            itemCount = count;
            itemIcon.sprite = itemImage;
            Debug.Log(itemCount);
            UpdateItemNumber();
        }
    }

    // public void RemoveItem(int count = 1)
    // {
    //     itemCount -= count;
    //     if (itemCount <= 0)
    //     {
    //         ClearSlot();
    //     }
    //     else
    //     {
    //         UpdateItemNumber();
    //     }
    // }

    public int GetItemCount()
    {
        return itemCount;
    }

    public void UpdateItemNumber()
    {
        if (itemCount < 2)
        {
            itemNumber.text = "1";
        }
        else
        {
            itemNumber.text = itemCount.ToString();
        }
    }

    public void ClearSlot()
    {
        itemSlotName.text = "";
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
        itemNameText.text = itemSlotName.text;
        itemDescriptionText.text = itemDescription;
        itemDesciptionImage.sprite = itemSprite;
        itemDesciptionNumber.text = itemNumber.text;
        if (itemDesciptionImage.sprite == null)
        {
            itemDesciptionImage.sprite = emptySlotSprite;
        }
    }

    
}
