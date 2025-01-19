using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterManager : MonoBehaviour
{
    public List<CharacterAttributes> allCharacters = new List<CharacterAttributes>(); // 所有角色
    public List<Button> characterIndexButtons; // 角色按钮
    public GameObject characterDetailPanel; // 角色详情页面
    public GameObject characterCreationPanel; // 角色创建页面
    public Text characterNameText;
    public Text characterStoryText;
    public Text experienceText;
    public Text levelText;
    public Text strengthText;
    public Text agilityText;
    public Text intelligenceText;
    public Text healthText;
    public Text physicalAttackText;
    public Text physicalDefenseText;
    public Text soulAttackText;
    public Text soulDefenseText;
    public Text speedText;
    public Text criticalRateText;
    public Text hitRateText;
    public Text tenacityRateText;
    public Text Role;
    public Text Gender;
    public Image characterImage;
    public static NewCharacterManager instance;
    public Image StrengthBar;
    public Image AgilityBar;
    public Image IntelligenceBar;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        foreach (var button in characterIndexButtons)
        {
            int index = characterIndexButtons.IndexOf(button); // 避免闭包问题
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCharacterButtonClicked(index));
        }
    }

    private void OnCharacterButtonClicked(int index)
    {
        if (index < allCharacters.Count && allCharacters[index] != null)
        {
            ShowCharacterDetails(allCharacters[index]);
        }
        else
        {
            characterCreationPanel.SetActive(true);
            characterDetailPanel.SetActive(false);
        }
    }

    private void ShowCharacterDetails(CharacterAttributes character)
    {
        characterCreationPanel.SetActive(false);
        characterDetailPanel.SetActive(true);

        characterNameText.text = character.basic_information.name;
        characterStoryText.text = "背景故事：" + character.basic_information.story;
        experienceText.text = "经历：" + character.experience;
        levelText.text = character.level.ToString();
        strengthText.text = character.strength.ToString();
        agilityText.text = character.agility.ToString();
        intelligenceText.text = character.intelligence.ToString();
        healthText.text = character.health.ToString();
        physicalAttackText.text = character.physicalAttack.ToString();
        physicalDefenseText.text = character.physicalDefense.ToString();
        soulAttackText.text = character.soulAttack.ToString();
        soulDefenseText.text = character.soulDefense.ToString();
        speedText.text = character.speed.ToString();
        criticalRateText.text = character.criticalRate.ToString();
        hitRateText.text = character.hitRate.ToString();
        tenacityRateText.text = character.tenacityRate.ToString();
        StrengthBar.fillAmount = character.strength / character.potentialStrength;
        AgilityBar.fillAmount = character.agility / character.potentialAgility;
        IntelligenceBar.fillAmount = character.intelligence / character.potentialIntelligence;
        Gender.text = character.basic_information.gender;

        StartCoroutine(APIManager.instance.LoadImage(character.character_picture, characterImage));
    }

    public void AddCharacter(CharacterAttributes character)
    {
        allCharacters.Add(character);
        Debug.Log($"角色 {character.characterName} 已添加！");
    }
    
    public static CharacterAttributes ConvertToCharacterAttributes(ClassManager.CharacterData characterData)
    {
        var characterAttributes = new CharacterAttributes
        {
            characterName = characterData.basic_information.name,
            intelligence = ExtractAttributeValue(characterData.current_ability, "智力"),
            potentialIntelligence = ExtractAttributeValue(characterData.potential_ability, "智力"),
            agility = ExtractAttributeValue(characterData.current_ability, "敏捷"),
            potentialAgility = ExtractAttributeValue(characterData.potential_ability, "敏捷"),
            strength = ExtractAttributeValue(characterData.current_ability, "力量"),
            potentialStrength = ExtractAttributeValue(characterData.potential_ability, "力量"),
            health = CalculateHealth(characterData),
            physicalAttack = CalculatePhysicalAttack(characterData),
            physicalDefense = CalculatePhysicalDefense(characterData),
            soulAttack = CalculateSoulAttack(characterData),
            soulDefense = CalculateSoulDefense(characterData),
            speed = CalculateSpeed(characterData),
            criticalRate = CalculateCriticalRate(characterData),
            hitRate = CalculateHitRate(characterData),
            tenacityRate = CalculateTenacityRate(characterData),
            damageX1 = 1f,
            damageX2 = 1f,
            currentHealth = CalculateHealth(characterData),
            shieldAmount = 0f,
            energy = 0,
            maxEnergy = 10,
            level = 1,
            timePoint = 0f,
            skillAttributes = ConvertToSkillAttributes(characterData),
            basic_information = characterData.basic_information,
            character_picture = characterData.character_picture,
            current_ability = characterData.current_ability,
            potential_ability = characterData.potential_ability,
            experience = characterData.experience,
            talent1 = characterData.talent1,
            talent_count1 = characterData.talent_count1,
            talent2 = characterData.talent2,
            talent_count2 = characterData.talent_count2,
            talent3 = characterData.talent3,
            talent_count3 = characterData.talent_count3,
        };

        return characterAttributes;
    }

    private static float ExtractAttributeValue(List<string> abilityList, string attributeName)
    {
        foreach (var ability in abilityList)
        {
            if (ability.StartsWith($"- {attributeName}："))
            {
                var valueStr = ability.Replace($"- {attributeName}：", "");
                if (float.TryParse(valueStr, out float value))
                {
                    return value;
                }
            }
        }
        return 0f;
    }

    private static SkillAttributes ConvertToSkillAttributes(ClassManager.CharacterData characterData)
    {
        return new SkillAttributes
        {
            cost = int.TryParse(characterData.talent1?.cost, out var cost) ? cost : 0,
            physicalDamage = characterData.talent_count1 != null && characterData.talent_count1.Count > 0
                ? float.Parse(characterData.talent_count1[0])
                : 0f,
            soulDamage = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[1])
                : 0f,
            stunChance = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[2])
                : 0f,
            silenceChance = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[3])
                : 0f,
            criticalBoost = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[4])
                : 0f,
            shieldAmount = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[5])
                : 0f,
            blockChance = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[6])
                : 0f,
            healAmount = characterData.talent_count1 != null && characterData.talent_count1.Count > 1
                ? float.Parse(characterData.talent_count1[7])
                : 0f
        };
    }

    private static float CalculateHealth(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "力量") * 50f + ExtractAttributeValue(characterData.current_ability, "智力") * 20f + ExtractAttributeValue(characterData.current_ability, "敏捷") * 30f;
    }
    private static float CalculatePhysicalAttack(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "力量") * 10f;
    }

    private static float CalculatePhysicalDefense(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "力量") * 5f;
    }

    private static float CalculateSoulAttack(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "智力") * 20f;
    }

    private static float CalculateSoulDefense(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "智力") * 10f;
    }

    private static float CalculateSpeed(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "敏捷") * 1f + 100f;
    }

    private static float CalculateCriticalRate(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "敏捷") * 0.01f;
    }

    private static float CalculateHitRate(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "智力") * 0.01f;
    }

    private static float CalculateTenacityRate(ClassManager.CharacterData characterData)
    {
        return ExtractAttributeValue(characterData.current_ability, "力量") * 0.01f;
    }
    

}