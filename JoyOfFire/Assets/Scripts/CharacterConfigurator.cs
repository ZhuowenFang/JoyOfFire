using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CharacterConfigurator : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField costInput;
    public TMP_InputField physicalDamageInput;
    public TMP_InputField soulDamageInput;
    public TMP_InputField stunChanceInput;
    public TMP_InputField silenceChanceInput;
    public TMP_InputField criticalBoostInput;
    public TMP_InputField shieldAmountInput;
    public TMP_InputField blockChanceInput;
    public TMP_InputField healAmountInput;
    public TMP_InputField intelligenceInput;
    public TMP_InputField agilityInput;
    public TMP_InputField strengthInput;
    public TMP_InputField healthInput;
    public TMP_InputField physicalAttackInput;
    public TMP_InputField physicalDefenseInput;
    public TMP_InputField soulAttackInput;
    public TMP_InputField soulDefenseInput;
    public TMP_InputField speedInput;
    public TMP_InputField criticalRateInput;
    public TMP_InputField hitRateInput;
    public TMP_InputField tenacityRateInput;
    public TMP_InputField damageX1Input;
    public TMP_InputField damageX2Input;
    public TMP_InputField levelInput;
    
    public GameObject characterConfiguratorPanel;
    

    private CharacterAttributes character;
    
    private void Start()
    {
        intelligenceInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
        agilityInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
        strengthInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
    }
    // public void LoadCharacter(CharacterAttributes charAttr)
    // {
    //     character = charAttr;
    //     if (character.skillAttributes == null)
    //     {
    //         character.skillAttributes = new SkillAttributes();
    //     }
    //     intelligenceInput.onValueChanged.RemoveAllListeners();
    //     agilityInput.onValueChanged.RemoveAllListeners();
    //     strengthInput.onValueChanged.RemoveAllListeners();
    //     nameInput.text = character.characterName;
    //     costInput.text = character.skillAttributes.cost.ToString();
    //     // skill1Input.text = character.characterSkill1Entry;
    //     physicalDamageInput.text = character.skillAttributes.physicalDamage.ToString("F2");
    //     soulDamageInput.text = character.skillAttributes.soulDamage.ToString("F2");
    //     stunChanceInput.text = character.skillAttributes.stunChance.ToString("F2");
    //     silenceChanceInput.text = character.skillAttributes.silenceChance.ToString("F2");
    //     criticalBoostInput.text = character.skillAttributes.criticalBoost.ToString("F2");
    //     shieldAmountInput.text = character.skillAttributes.shieldAmount.ToString("F2");
    //     blockChanceInput.text = character.skillAttributes.blockChance.ToString("F2");
    //     healAmountInput.text = character.skillAttributes.healAmount.ToString("F2");
    //     intelligenceInput.text = character.intelligence.ToString("F2");
    //     agilityInput.text = character.agility.ToString("F2");
    //     strengthInput.text = character.strength.ToString("F2");
    //     healthInput.text = character.health.ToString("F2");
    //     physicalAttackInput.text = character.physicalAttack.ToString("F2");
    //     physicalDefenseInput.text = character.physicalDefense.ToString("F2");
    //     soulAttackInput.text = character.soulAttack.ToString("F2");
    //     soulDefenseInput.text = character.soulDefense.ToString("F2");
    //     speedInput.text = character.speed.ToString("F2");
    //     criticalRateInput.text = character.criticalRate.ToString("F2");
    //     hitRateInput.text = character.hitRate.ToString("F2");
    //     tenacityRateInput.text = character.tenacityRate.ToString("F2");
    //     damageX1Input.text = character.damageX1.ToString("F2");
    //     damageX2Input.text = character.damageX2.ToString("F2");
    //     levelInput.text = character.level.ToString();
    //     intelligenceInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
    //     agilityInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
    //     strengthInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
    // }
    private void UpdateDerivedAttributes()
    {
        if (float.TryParse(intelligenceInput.text, out float intelligence) &&
            float.TryParse(agilityInput.text, out float agility) &&
            float.TryParse(strengthInput.text, out float strength))
        {
            character.intelligence = intelligence;
            character.agility = agility;
            character.strength = strength;

            CalculateDerivedAttributes();

            UpdateDerivedAttributesToUI();
        }
    }

    // public void SaveCharacter()
    // {
    //     
    //     if (!AreAllInputsValid())
    //     {
    //         Debug.LogError("所有属性必须填写，且不能为零！");
    //         return;
    //     }
    //
    //     character.skillAttributes = new SkillAttributes
    //     {
    //         cost = int.Parse(costInput.text),
    //         physicalDamage = float.Parse(physicalDamageInput.text),
    //         soulDamage = float.Parse(soulDamageInput.text),
    //         stunChance = float.Parse(stunChanceInput.text),
    //         silenceChance = float.Parse(silenceChanceInput.text),
    //         criticalBoost = float.Parse(criticalBoostInput.text),
    //         shieldAmount = float.Parse(shieldAmountInput.text),
    //         blockChance = float.Parse(blockChanceInput.text),
    //         healAmount = float.Parse(healAmountInput.text)
    //     };
    //     character.characterName = nameInput.text;
    //     character.intelligence = float.Parse(intelligenceInput.text);
    //     character.agility = float.Parse(agilityInput.text);
    //     character.strength = float.Parse(strengthInput.text);
    //     character.health = float.Parse(healthInput.text);
    //     character.physicalAttack = float.Parse(physicalAttackInput.text);
    //     character.physicalDefense = float.Parse(physicalDefenseInput.text);
    //     character.soulAttack = float.Parse(soulAttackInput.text);
    //     character.soulDefense = float.Parse(soulDefenseInput.text);
    //     character.speed = float.Parse(speedInput.text);
    //     character.criticalRate = float.Parse(criticalRateInput.text);
    //     character.hitRate = float.Parse(hitRateInput.text);
    //     character.tenacityRate = float.Parse(tenacityRateInput.text);
    //     character.damageX1 = float.Parse(damageX1Input.text);
    //     character.damageX2 = float.Parse(damageX2Input.text);
    //     character.level = int.Parse(levelInput.text);
    //     character.timePoint = 0f;
    //     character.currentHealth = character.health;
    //     character.energy = 0;
    //     character.maxEnergy = 10;
    //
    //     Debug.Log("Character attributes saved and calculated.");
    //     characterConfiguratorPanel.SetActive(false);
    // }
    public bool AreAllInputsValid()
    {
        TMP_InputField[] requiredFields = 
        {
            intelligenceInput, agilityInput, strengthInput,
            healthInput, physicalAttackInput, physicalDefenseInput,
            soulAttackInput, soulDefenseInput, speedInput,
            criticalRateInput, hitRateInput, tenacityRateInput, damageX1Input, damageX2Input, levelInput
        };

        foreach (var field in requiredFields)
        {
            if (string.IsNullOrWhiteSpace(field.text) || !float.TryParse(field.text, out float value) || value <= 0)
            {
                Debug.LogError($"输入字段 {field.name} 无效，值：{field.text}");
                return false;
            }
        }

        return true;
    }

    private void CalculateDerivedAttributes()
    {
        character.soulAttack = character.intelligence * 20.0f;
        character.soulDefense = character.intelligence * 5.0f;
        character.health = character.intelligence * 30.0f
                           + character.agility * 20.0f
                           + character.strength * 50.0f;
        character.physicalAttack = character.agility * 10.0f
                                   + character.strength * 5.0f;
        character.physicalDefense = character.strength * 15.0f;
        character.speed = character.agility * 1.0f + 100f;
        character.criticalRate = character.agility * 1.0f / 100f;
        character.hitRate = character.intelligence * 1.0f / 100f;
        character.tenacityRate = character.strength * 1.0f / 100f; 
    }

    private void UpdateDerivedAttributesToUI()
    {
        healthInput.text = character.health.ToString("F2");
        physicalAttackInput.text = character.physicalAttack.ToString("F2");
        physicalDefenseInput.text = character.physicalDefense.ToString("F2");
        soulAttackInput.text = character.soulAttack.ToString("F2");
        soulDefenseInput.text = character.soulDefense.ToString("F2");
        speedInput.text = character.speed.ToString("F2");
        criticalRateInput.text = character.criticalRate.ToString("F2");
        hitRateInput.text = character.hitRate.ToString("F2");
        tenacityRateInput.text = character.tenacityRate.ToString("F2");
    }
    public CharacterAttributes GetCharacter()
    {
        return character;
    }
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
    public ClassManager.BasicInformation basic_information;
    public string character_picture;
    public List<string> current_ability;
    public List<string> potential_ability;
    public string experience;
    
    public int attributePoints = 0;
    public float additionalHealth = 0f;
    public float sanValue = 0f;

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
            sanValue = this.sanValue
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

    public BuffEffect(ICharacter Attacker, string name, BuffType type, int turns, float stackVal, bool debuff, int maxStackVal)
    {
        attacker = Attacker;
        buffName = name;
        buffType = type;
        duration = turns;
        stack = stackVal;
        isDebuff = debuff;
        maxStack = maxStackVal;
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


    

