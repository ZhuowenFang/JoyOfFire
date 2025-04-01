using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CharacterConfigurator : MonoBehaviour
{
    
    

   
}



[System.Serializable]
public class CharacterAttributes : ICharacter
{
    public List<SkillAttributes> skills = new List<SkillAttributes>();
    public float initialIntelligence;
    public float initialAgility;
    public float initialStrength;
    public float intelligence;
    public float potentialIntelligence;
    public float agility;
    public float potentialAgility;
    public float strength;
    public float potentialStrength;

    public string id;
    public string user_id;
    public string character_id;
    public ClassManager.BasicInformation basic_information;
    public string character_picture;
    public List<string> current_ability;
    public List<string> potential_ability;
    public List<string> experience;
    
    public int attributePoints = 0;
    public float additionalHealth = 0f;
    public float sanValue = 0f;
    public string role;
    public int star = 0;

    public CharacterAttributes Clone()
    {
        return new CharacterAttributes
        {
            index = this.index,
            characterName = this.characterName,
            skills = new List<SkillAttributes>(this.skills),
            intelligence = this.intelligence,
            potentialIntelligence = this.potentialIntelligence,
            agility = this.agility,
            potentialAgility = this.potentialAgility,
            strength = this.strength,
            potentialStrength = this.potentialStrength,
            health = this.health,
            currentHealth = this.currentHealth,
            physicalAttack = this.physicalAttack,
            physicalDefense = this.physicalDefense,
            soulAttack = this.soulAttack,
            soulDefense = this.soulDefense,
            speed = this.speed,
            criticalRate = this.criticalRate,
            hitRate = this.hitRate,
            tenacityRate = this.tenacityRate,
            damageX1 = this.damageX1,
            damageX2 = this.damageX2,
            timePoint = this.timePoint,
            shieldAmount = this.shieldAmount,
            energy = this.energy,
            maxEnergy = this.maxEnergy,
            level = this.level,
            id = this.id,
            user_id = this.user_id,
            character_id = this.character_id,
            basic_information = this.basic_information,
            character_picture = this.character_picture,
            current_ability = this.current_ability,
            potential_ability = this.potential_ability,
            experience = this.experience,
            attributePoints = this.attributePoints,
            initialAgility = this.initialAgility,
            initialIntelligence = this.initialIntelligence,
            initialStrength = this.initialStrength,
            additionalHealth = this.additionalHealth,
            sanValue = this.sanValue,
            star = this.star,
            role = this.role,
        };
    }
}

[System.Serializable]
public class SkillAttributes
{
    public string skillName;
    public string skillDescription;
    public int skillCost;
    public string skillIcon;
    public List<float> skillVector; // 技能属性数值

    public float physicalDamage;
    public float soulDamage;
    public float stunChance;
    public float silenceChance;
    public float criticalBoost;
    public float shieldAmount;
    public float blockChance;
    public float healAmount;
}


[System.Serializable]
public class MonsterAttributes : ICharacter
{
    public string id;
    public string monsterId;
    public float sanValue;
    public List<MonsterSkillAttributes> skills = new List<MonsterSkillAttributes>();
    public float base_gold_value;

    public MonsterAttributes Clone()
    {
        return new MonsterAttributes
        {
            index = this.index,
            characterName = this.characterName,
            health = this.health,
            currentHealth = this.currentHealth,
            physicalAttack = this.physicalAttack,
            physicalDefense = this.physicalDefense,
            soulAttack = this.soulAttack,
            soulDefense = this.soulDefense,
            speed = this.speed,
            criticalRate = this.criticalRate,
            hitRate = this.hitRate,
            tenacityRate = this.tenacityRate,
            damageX1 = this.damageX1,
            damageX2 = this.damageX2,
            timePoint = this.timePoint,
            shieldAmount = this.shieldAmount,
            energy = this.energy,
            maxEnergy = this.maxEnergy,
            level = this.level,
            id = this.id,
            monsterId = this.monsterId,
            sanValue = this.sanValue,
            base_gold_value = this.base_gold_value,
            skills = CloneSkills(this.skills)
        };
    }
    private List<MonsterSkillAttributes> CloneSkills(List<MonsterSkillAttributes> originalSkills)
    {
        List<MonsterSkillAttributes> clonedSkills = new List<MonsterSkillAttributes>();
        foreach (var skill in originalSkills)
        {
            clonedSkills.Add(new MonsterSkillAttributes
            {
                skillName = skill.skillName,
                skillDescription = skill.skillDescription,
                skillCost = skill.skillCost,
                skillIcon = skill.skillIcon,
                skillValues = new List<float>(skill.skillValues),

                physicalDamage = skill.physicalDamage,
                physicalAttackCount = skill.physicalAttackCount,
                soulDamage = skill.soulDamage,
                soulAttackCount = skill.soulAttackCount,
                sanityLoss = skill.sanityLoss,
                targetAlly = skill.targetAlly,
                targetSelf = skill.targetSelf,
                targetCount = skill.targetCount,
                criticalChance = skill.criticalChance,
                shieldValue = skill.shieldValue,
                blockChance = skill.blockChance,
                fixedLifesteal = skill.fixedLifesteal,
                percentLifesteal = skill.percentLifesteal,
                fixedHeal = skill.fixedHeal,
                percentHeal = skill.percentHeal,
                stunChance = skill.stunChance,
                silenceChance = skill.silenceChance,
                bindChance = skill.bindChance,
                bleedStacks = skill.bleedStacks,
                bleedChance = skill.bleedChance,
                burnStacks = skill.burnStacks,
                burnChance = skill.burnChance,
                poisonStacks = skill.poisonStacks,
                drunkStacks = skill.drunkStacks,
                gazeStacks = skill.gazeStacks,
                executeTarget = skill.executeTarget,
                specialDish = skill.specialDish,
                knifeLimit = skill.knifeLimit,
                knifeHpCost = skill.knifeHpCost,
                knifeConsume = skill.knifeConsume,
                damageBoost = skill.damageBoost,
                taunt = skill.taunt,
                stealth = skill.stealth,
                skipTurn = skill.skipTurn,
                reflectDamage = skill.reflectDamage,
                dispelDebuff = skill.dispelDebuff
            });
        }
        return clonedSkills;
    }
}

[System.Serializable]
public class MonsterSkillAttributes
{
    public string skillName;
    public string skillDescription;
    public int skillCost;
    public string skillIcon;
    public List<float> skillValues;

    public float physicalDamage;
    public float physicalAttackCount;
    public float soulDamage;
    public float soulAttackCount;
    public float sanityLoss;
    public bool targetAlly;
    public bool targetSelf;
    public float targetCount;
    public float criticalChance;
    public float shieldValue;
    public float blockChance;
    public float fixedLifesteal;
    public float percentLifesteal;
    public float fixedHeal;
    public float percentHeal;
    public float stunChance;
    public float silenceChance;
    public float bindChance;    // 束缚概率

    // **持续伤害状态**
    public float bleedStacks;   // 流血层数
    public float bleedChance;   // 流血概率
    public float burnStacks;    // 灼烧层数
    public float burnChance;    // 灼烧概率
    public float poisonStacks;  // 中毒层数
    public float drunkStacks;   // 醉酒层数
    public float gazeStacks;    // 凝视层数

    // **特殊效果**
    public bool executeTarget;  // 宰杀目标（是否触发斩杀机制）
    public bool specialDish;    // 特色菜肴（可能和餐刀机制有关）

    // **餐刀机制**
    public float knifeLimit;    // 餐刀生成上限
    public float knifeHpCost;   // 释放技能消耗生命值
    public float knifeConsume;  // 消耗餐刀数

    // **其他**
    public float damageBoost;    // 伤害强化
    public bool taunt;           // 嘲讽
    public bool stealth;         // 潜行
    public bool skipTurn;        // 跳过回合
    public bool reflectDamage;   // 反伤
    public bool dispelDebuff;    // 驱散异常状态
}
[System.Serializable]
public abstract class ICharacter
{
    public int index { get; set; }
    public string characterName { get; set; }
    public float health { get; set; }
    public float currentHealth { get; set; }
    public float physicalAttack { get; set; }
    public float physicalDefense { get; set; }
    public float soulAttack { get; set; }
    public float soulDefense { get; set; }
    public float speed { get; set; }
    public float criticalRate { get; set; }
    public float hitRate { get; set; }
    public float tenacityRate { get; set; }
    public float damageX1 { get; set; }
    public float damageX2 { get; set; }
    public float timePoint { get; set; }
    public float shieldAmount { get; set; }
    public int energy { get; set; }
    public int maxEnergy { get; set; }
    public int level { get; set; }
    public List<BuffEffect> activeBuffs { get; set; } = new List<BuffEffect>();
}
[System.Serializable]
public class BuffEffect
{
    public ICharacter attacker; // 施加者
    public string buffName;  // Buff 名称
    public BuffType buffType; // Buff 类型
    public int duration;  // 持续回合数
    public float stack;   // 数值（伤害、加成等）
    public bool isDebuff;
    public int maxStack;
    public string description; // Buff 描述

    public BuffEffect(ICharacter Attacker, string name, BuffType type, int turns, float stackVal, bool debuff, int maxStackVal)
    {
        attacker = Attacker;
        buffName = name;
        buffType = type;
        duration = turns;
        stack = stackVal;
        isDebuff = debuff;
        maxStack = maxStackVal;
        switch (type)
        {
            case BuffType.Bind:
                description = "速度减半";
                break;
            case BuffType.Bleed:
                description = "被施加者行动时流失生命值，流失值为被施加者最大生命的1%";
                break;
            case BuffType.Burn:
                description = "回合结束时受到物理伤害，伤害值为施加者攻击的1%";
                break;
            case BuffType.DamageBoost:
                description = "提升所造成的伤害（伤害强化层数*10%）";
                break;
            case BuffType.Dispel:
                break;
            case BuffType.Drunk:
                description = "被施加了醉酒的角色在释放指向性的攻击和伤害类技能时，有(醉酒层数）*20%的概率转而随机指定场上任意一位友方和敌方单位。";
                break;
            case BuffType.Execute:
                description = "被施加了宰杀目标的角色会受到具有“美梦成真”状态的角色的额外伤害，额外伤害值为20%（加算）";
                break;
            case BuffType.Gaze:
                description = "被施加者每回合结束时受到等同于凝视层数的精神伤害。";
                break;
            case BuffType.HealOverTime:
                break;
            case BuffType.Poison:
                description = "被施加者行动时流失生命值，流失值为中毒层数";
                break;
            case BuffType.Silence:
                description = "被沉默的角色不能释放技能，持续到下一回合";
                break;
            case BuffType.Shield:
                description = "受攻击时，优先结算护盾，护盾为0后结算生命值";
                break;
            case BuffType.SkipTurn:
                description = "被跳过回合的角色本回合无法使用任何普攻或技能";
                break;
            case BuffType.Stun:
                description = "被眩晕的角色不能释放技能，持续到下一回合";
                break;
            case BuffType.Taunt:
                description = "敌人必须优先指定带有嘲讽的单位";
                break;
            case BuffType.Reflect:
                description = "被攻击时会反弹50%的伤害";
                break;
            case BuffType.Stealth:
                description = "无法被指定为攻击目标，但攻击和技能将会打破这个状态";
                break;
            case BuffType.Block:
                description = "提升物理防御和灵魂防御，以及韧性率";
                break;
            case BuffType.Cuisine:
                description = "开发中";
                break;
            case BuffType.Knife:
                description = "开发中";
                break;
            case BuffType.Dream:
                description = "开发中";
                break;
            
                
                
        }
    }
}

// Buff 类型
public enum BuffType
{
    Stun,         // 眩晕
    Silence,      // 沉默
    Burn,         // 灼烧
    Poison,       // 中毒
    Bleed,        // 流血
    Drunk,        // 醉酒
    Taunt,        // 嘲讽
    Shield,       // 护盾
    HealOverTime, // 持续恢复
    DamageBoost,  // 伤害提升
    Block, // 防御提升
    Stealth,      // 潜行
    SkipTurn,     // 跳过回合
    Reflect,      // 反伤
    Dispel,       // 驱散异常状态
    Gaze,        // 凝视
    Cuisine,    // 特色菜肴
    Knife,      // 餐刀
    Dream,      // 梦境
    Execute,
    Bind
}


    

