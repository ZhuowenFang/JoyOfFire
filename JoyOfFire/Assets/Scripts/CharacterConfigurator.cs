using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public void LoadCharacter(CharacterAttributes charAttr)
    {
        character = charAttr;
        if (character.skillAttributes == null)
        {
            character.skillAttributes = new SkillAttributes();
        }
        intelligenceInput.onValueChanged.RemoveAllListeners();
        agilityInput.onValueChanged.RemoveAllListeners();
        strengthInput.onValueChanged.RemoveAllListeners();
        nameInput.text = character.characterName;
        costInput.text = character.skillAttributes.cost.ToString();
        // skill1Input.text = character.characterSkill1Entry;
        physicalDamageInput.text = character.skillAttributes.physicalDamage.ToString("F2");
        soulDamageInput.text = character.skillAttributes.soulDamage.ToString("F2");
        stunChanceInput.text = character.skillAttributes.stunChance.ToString("F2");
        silenceChanceInput.text = character.skillAttributes.silenceChance.ToString("F2");
        criticalBoostInput.text = character.skillAttributes.criticalBoost.ToString("F2");
        shieldAmountInput.text = character.skillAttributes.shieldAmount.ToString("F2");
        blockChanceInput.text = character.skillAttributes.blockChance.ToString("F2");
        healAmountInput.text = character.skillAttributes.healAmount.ToString("F2");
        intelligenceInput.text = character.intelligence.ToString("F2");
        agilityInput.text = character.agility.ToString("F2");
        strengthInput.text = character.strength.ToString("F2");
        healthInput.text = character.health.ToString("F2");
        physicalAttackInput.text = character.physicalAttack.ToString("F2");
        physicalDefenseInput.text = character.physicalDefense.ToString("F2");
        soulAttackInput.text = character.soulAttack.ToString("F2");
        soulDefenseInput.text = character.soulDefense.ToString("F2");
        speedInput.text = character.speed.ToString("F2");
        criticalRateInput.text = character.criticalRate.ToString("F2");
        hitRateInput.text = character.hitRate.ToString("F2");
        tenacityRateInput.text = character.tenacityRate.ToString("F2");
        damageX1Input.text = character.damageX1.ToString("F2");
        damageX2Input.text = character.damageX2.ToString("F2");
        levelInput.text = character.level.ToString();
        intelligenceInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
        agilityInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
        strengthInput.onValueChanged.AddListener(_ => UpdateDerivedAttributes());
    }
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

    public void SaveCharacter()
    {
        
        if (!AreAllInputsValid())
        {
            Debug.LogError("所有属性必须填写，且不能为零！");
            return;
        }

        character.skillAttributes = new SkillAttributes
        {
            cost = int.Parse(costInput.text),
            physicalDamage = float.Parse(physicalDamageInput.text),
            soulDamage = float.Parse(soulDamageInput.text),
            stunChance = float.Parse(stunChanceInput.text),
            silenceChance = float.Parse(silenceChanceInput.text),
            criticalBoost = float.Parse(criticalBoostInput.text),
            shieldAmount = float.Parse(shieldAmountInput.text),
            blockChance = float.Parse(blockChanceInput.text),
            healAmount = float.Parse(healAmountInput.text)
        };
        character.characterName = nameInput.text;
        character.intelligence = float.Parse(intelligenceInput.text);
        character.agility = float.Parse(agilityInput.text);
        character.strength = float.Parse(strengthInput.text);
        character.health = float.Parse(healthInput.text);
        character.physicalAttack = float.Parse(physicalAttackInput.text);
        character.physicalDefense = float.Parse(physicalDefenseInput.text);
        character.soulAttack = float.Parse(soulAttackInput.text);
        character.soulDefense = float.Parse(soulDefenseInput.text);
        character.speed = float.Parse(speedInput.text);
        character.criticalRate = float.Parse(criticalRateInput.text);
        character.hitRate = float.Parse(hitRateInput.text);
        character.tenacityRate = float.Parse(tenacityRateInput.text);
        character.damageX1 = float.Parse(damageX1Input.text);
        character.damageX2 = float.Parse(damageX2Input.text);
        character.level = int.Parse(levelInput.text);
        character.timePoint = 0f;
        character.currentHealth = character.health;
        character.energy = 0;
        character.maxEnergy = 10;

        Debug.Log("Character attributes saved and calculated.");
        characterConfiguratorPanel.SetActive(false);
    }
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
public class CharacterAttributes
{
    public int index; 
    public string characterName;
    public SkillAttributes skillAttributes; 
    public float intelligence;
    public float potentialIntelligence;
    public float agility;
    public float potentialAgility;
    public float strength;
    public float potentialStrength;
    public float health;
    public float currentHealth;
    public float physicalAttack;
    public float physicalDefense;
    public float soulAttack;
    public float soulDefense;
    public float speed;
    public float criticalRate;
    public float hitRate;
    public float tenacityRate;
    public float damageX1;
    public float damageX2;
    public float timePoint;
    public int level;
    public float shieldAmount;
    public int energy;
    public int maxEnergy;
    public string id;
    public string user_id;
    public ClassManager.BasicInformation basic_information;
    public string character_picture;
    public List<string> current_ability;
    public List<string> potential_ability;
    public ClassManager.Talent talent1;
    public List<string> talent_count1;
    public ClassManager.Talent talent2;
    public List<string> talent_count2;
    public ClassManager.Talent talent3;
    public List<string> talent_count3;
    public string experience;
    public CharacterAttributes Clone()
    {
        return new CharacterAttributes
        {
            index = this.index,
            characterName = this.characterName,
            skillAttributes = this.skillAttributes,
            intelligence = this.intelligence,
            potentialIntelligence = this.potentialIntelligence,
            agility = this.agility,
            potentialAgility = this.potentialAgility,
            strength = this.strength,
            potentialStrength = this.potentialStrength,
            health = this.health,
            currentHealth = this.health,
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
            timePoint = 0f,
            level = this.level,
            shieldAmount = this.shieldAmount,
            energy = this.energy,
            maxEnergy = this.maxEnergy,
            id = this.id,
            user_id = this.user_id,
            basic_information = this.basic_information,
            character_picture = this.character_picture,
            current_ability = this.current_ability,
            potential_ability = this.potential_ability,
            talent1 = this.talent1,
            talent_count1 = this.talent_count1,
            talent2 = this.talent2,
            talent_count2 = this.talent_count2,
            talent3 = this.talent3,
            talent_count3 = this.talent_count3,
            experience = this.experience
            
        };
    }
}

[System.Serializable]
public class SkillAttributes
{
    public int cost;
    public float physicalDamage;
    public float soulDamage;
    public float stunChance;
    public float silenceChance;
    public float criticalBoost;
    public float shieldAmount;
    public float blockChance;
    public float healAmount;  
}



    

