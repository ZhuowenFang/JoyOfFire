using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterConfigurator : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField skill1Input;
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
        intelligenceInput.onValueChanged.RemoveAllListeners();
        agilityInput.onValueChanged.RemoveAllListeners();
        strengthInput.onValueChanged.RemoveAllListeners();
        nameInput.text = character.characterName;
        skill1Input.text = character.characterSkill1Entry;
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

            // 计算派生属性
            CalculateDerivedAttributes();

            // 更新派生属性到UI
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

        character.characterName = nameInput.text;
        character.characterSkill1Entry = skill1Input.text;
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

        Debug.Log("Character attributes saved and calculated.");
        characterConfiguratorPanel.SetActive(false);
    }
    public bool AreAllInputsValid()
    {
        // 遍历所有需要检查的输入框
        TMP_InputField[] requiredFields = 
        {
            intelligenceInput, agilityInput, strengthInput,
            healthInput, physicalAttackInput, physicalDefenseInput,
            soulAttackInput, soulDefenseInput, speedInput,
            criticalRateInput, hitRateInput, tenacityRateInput, damageX1Input, damageX2Input
        };

        foreach (var field in requiredFields)
        {
            if (string.IsNullOrWhiteSpace(field.text) || !float.TryParse(field.text, out float value) || value <= 0)
            {
                Debug.LogError($"输入字段 {field.name} 无效，值：{field.text}");
                return false;  // 有任意一个无效则失败
            }
        }

        return true;  // 所有字段均有效
    }

    private void CalculateDerivedAttributes()
    {
        // 基础属性的衍生公式
        character.soulAttack = character.intelligence * 20.0f;
        character.soulDefense = character.intelligence * 5.0f;
        character.hitRate = character.intelligence * 1.0f;
        character.health = character.intelligence * 30.0f
                           + character.agility * 20.0f
                           + character.strength * 50.0f;
        character.physicalAttack = character.agility * 10.0f
                                   + character.strength * 5.0f;
        character.physicalDefense = character.strength * 15.0f;
        character.speed = character.agility * 1.0f;
        character.criticalRate = character.agility * 1.0f;
        character.tenacityRate = character.strength * 1.0f;
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
    public string characterName;
    public string characterSkill1Entry;
    public float intelligence;
    public float agility;
    public float strength;
    public float health;
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
    public CharacterAttributes Clone()
    {
        return new CharacterAttributes
        {
            characterName = this.characterName,
            characterSkill1Entry = this.characterSkill1Entry,
            intelligence = this.intelligence,
            agility = this.agility,
            strength = this.strength,
            health = this.health,
            physicalAttack = this.physicalAttack,
            physicalDefense = this.physicalDefense,
            soulAttack = this.soulAttack,
            soulDefense = this.soulDefense,
            speed = this.speed,
            criticalRate = this.criticalRate,
            hitRate = this.hitRate,
            tenacityRate = this.tenacityRate,
            damageX1 = this.damageX1,
            damageX2 = this.damageX2
        };
    }
}
