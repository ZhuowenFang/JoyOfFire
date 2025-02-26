using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    private bool inventoryEnabled;

    public ItemSlot[] itemSlots;
    public Image itemDesciptionImage;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemNameText;
    public Sprite emptySlotSprite;
    public static InventoryManager instance;
    void Awake()
    {
        instance = this;
    }

    
    public void AddItem(string itemName, Sprite itemImage, string itemDescription)
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (itemSlot.isFull && itemSlot.itemName == itemName)
            {
                itemSlot.AddItem(itemName, itemImage, itemDescription);
                return;
            }
        }

        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (!itemSlot.isFull)
            {
                itemSlot.AddItem(itemName, itemImage, itemDescription);
                break;
            }
        }
    }

    public void DeselectAll()
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.isSelected = false;
            itemSlot.selectedShader.SetActive(false);
        }
        itemDesciptionImage.sprite = emptySlotSprite;
        itemDescriptionText.text = "";
        itemNameText.text = "";
    }
}
