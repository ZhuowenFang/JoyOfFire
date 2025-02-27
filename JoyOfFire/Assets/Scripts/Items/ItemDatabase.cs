using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    private Dictionary<string, ItemData> itemLookup;

    public void Initialize()
    {
        itemLookup = new Dictionary<string, ItemData>();
        foreach (var item in items)
        {
            itemLookup[item.itemName] = item;
        }
    }

    public ItemData GetItemByName(string itemName)
    {
        if (itemLookup == null)
            Initialize();
            
        return itemLookup.TryGetValue(itemName, out ItemData item) ? item : null;
    }
}