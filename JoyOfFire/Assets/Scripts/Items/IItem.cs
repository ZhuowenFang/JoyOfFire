using System;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    string ItemName { get; }
    string Description { get; }
    ItemType Type { get; }
    bool IsConsumable { get; }
    ItemEffectType EffectType { get; }
    void OnAcquire();    // 获得道具时调用，用于注册被动效果
    void OnRemove();     // 失去道具时调用，用于注销被动效果
    void OnUse();        // 主动使用道具时调用（仅主动道具需要实现）
}

public enum ItemType
{
    Combat,
    Exploration,
    Mission,
    Upgrade,
    Supply,
    Gold
}

public enum ItemEffectType
{
    Passive,    // 被动效果
    Active      // 主动效果
}