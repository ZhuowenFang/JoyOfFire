using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string chineseName;
    public string description;
    public bool IsConsumable;
    public ItemType type;
    public ItemEffectType effectType;
    public Sprite icon;
    public bool useCustomScript = false;  // 新增：是否使用自定义脚本
    public string itemScriptName;         // 仅当 useCustomScript 为 true 时使用
}