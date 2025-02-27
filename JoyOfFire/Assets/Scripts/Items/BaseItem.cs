using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour, IItem
{
    [SerializeField] protected string itemName;
    [SerializeField] protected string description;
    [SerializeField] protected ItemType type;
    [SerializeField] protected ItemEffectType effectType;
    [SerializeField] protected Sprite icon;

    public ItemData data;

    public string ItemName => data != null ? data.itemName : itemName;
    public string Description => data != null ? data.description : description;
    public ItemType Type => data != null ? data.type : type;
    public ItemEffectType EffectType => data != null ? data.effectType : effectType;
    public Sprite Icon => data != null ? data.icon : icon;

    public void Initialize(ItemData itemData)
    {
        data = itemData;
        itemName = data.itemName;
        description = data.description;
        type = data.type;
        effectType = data.effectType;
        icon = data.icon;
    }

    public virtual void OnAcquire()
    {
        Debug.Log($"获得道具: {ItemName}");
    }

    public virtual void OnRemove()
    {
        Debug.Log($"失去道具: {ItemName}");
    }

    public virtual void OnUse()
    {
        if (effectType != ItemEffectType.Active)
        {
            Debug.LogWarning($"尝试使用被动道具 {itemName}");
            return;
        }
        Debug.Log($"使用道具: {ItemName}");
    }

    protected virtual void OnDestroy()
    {
        OnRemove();
    }
}