using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public Text itemDescriptionText;
    public Text itemNameText;
    public Sprite emptySlotSprite;
    public static InventoryManager instance;
    [SerializeField] private ItemDatabase itemDatabase;
    public Dictionary<string, (BaseItem item, int count)> activeItems = new Dictionary<string, (BaseItem, int)>();    
    void Awake()
    {
        instance = this;
        instance = this;
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase not assigned in InventoryManager!");
            return;
        }
        itemDatabase.Initialize();
    }

    // public void AddItem(string itemName, Sprite itemImage, string itemDescription)
    // {
    //     foreach (ItemSlot itemSlot in itemSlots)
    //     {
    //         if (itemSlot.isFull && itemSlot.itemSlotName.text == itemName)
    //         {
    //             itemSlot.AddItem(itemName, itemImage, itemDescription);
    //             return;
    //         }
    //     }
    //
    //     foreach (ItemSlot itemSlot in itemSlots)
    //     {
    //         if (!itemSlot.isFull)
    //         {
    //             itemSlot.AddItem(itemName, itemImage, itemDescription);
    //             break;
    //         }
    //     }
    // }

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
    public void ObtainItem(string itemName, int count = 1)
    {
        if (activeItems.ContainsKey(itemName))
        {
            var (existingItem, currentCount) = activeItems[itemName];
            activeItems[itemName] = (existingItem, currentCount + count);
            foreach (ItemSlot itemSlot in itemSlots)
            {
                if (itemSlot.isFull && itemSlot.itemName == existingItem.data.chineseName)
                {
                    itemSlot.AddItem(existingItem.data.chineseName, existingItem.data.icon, existingItem.data.description, count);
                    return;
                }
            }
            return;
        }

        ItemData itemData = itemDatabase.GetItemByName(itemName);
        if (itemData == null)
        {
            Debug.LogError($"找不到道具: {itemName}");
            return;
        }

        GameObject itemObj = new GameObject(itemName);
        itemObj.transform.SetParent(transform);
        
        BaseItem newItem;
        if (itemData.useCustomScript)
        {
            System.Type itemType = System.Type.GetType(itemData.itemScriptName);
            newItem = itemObj.AddComponent(itemType) as BaseItem;
        }
        else
        {
            newItem = itemObj.AddComponent<BasicItem>();
        }
        
        newItem.Initialize(itemData);
        activeItems.Add(itemName, (newItem, count));
        newItem.OnAcquire();
 
        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (!itemSlot.isFull)
            {
                itemSlot.AddItem(newItem.data.chineseName, newItem.data.icon, newItem.data.description, count);
                break;
            }
        }
        
        
        // UpdateUI();
    }
    
    public void initialNumbers()
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.UpdateItemNumber();
        }
    }

    public void UpdateUI()
    {
        
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.ClearSlot();
        }
        
        foreach (var (itemName, itemData) in activeItems)
        {
            var (item, count) = itemData;
            foreach (ItemSlot itemSlot in itemSlots)
            {
                if (itemSlot.isFull && itemSlot.itemSlotName.text == itemName)
                {
                    itemSlot.AddItem(item.data.chineseName, item.data.icon, item.data.description, count);
                    return;
                }
            }
            
            foreach (ItemSlot itemSlot in itemSlots)
            {
                if (!itemSlot.isFull)
                {
                    itemSlot.AddItem(item.data.chineseName, item.data.icon, item.data.description, count);
                    break;
                }
            }
        }
    }

    public void RemoveItem(string itemName, int count = 1)
    {
        if (activeItems.TryGetValue(itemName, out var itemData))
        {
            var (item, currentCount) = itemData;
            if (currentCount <= count)
            {
                // 移除所有
                item.OnRemove();
                Destroy(item.gameObject);
                activeItems.Remove(itemName);
            }
            else
            {
                // 减少数量
                activeItems[itemName] = (item, currentCount - count);
            }
            UpdateUI();
        }
    }
}
