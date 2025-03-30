using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterManager : MonoBehaviour
{
    [SerializeReference]
    public List<ICharacter> allCharacters = new List<ICharacter>();
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
            int index = characterIndexButtons.IndexOf(button);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCharacterButtonClicked(index));
        }
        if (allCharacters.Count > 0)
        {
            ShowCharacterDetails(allCharacters[0] as CharacterAttributes);
        }
        else
        {
            characterCreationPanel.SetActive(true);
            characterDetailPanel.SetActive(false);
        }
    }

    private void OnCharacterButtonClicked(int index)
    {
        if (index < allCharacters.Count && allCharacters[index] != null)
        {
            ShowCharacterDetails(allCharacters[index] as CharacterAttributes);
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

    public void AddCharacter(ICharacter character)
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
            skills = ConvertToSkillAttributes(characterData),
            basic_information = characterData.basic_information,
            character_picture = characterData.character_picture,
            current_ability = characterData.current_ability,
            potential_ability = characterData.potential_ability,
            experience = characterData.experience
        };

        return characterAttributes;
    }


    private static float ExtractAttributeValue(List<string> abilityList, string attributeName)
    {
        foreach (var ability in abilityList)
        {
            if (ability.StartsWith($"- {attributeName}："))
            {
                var valueStr = ability.Replace($"- {attributeName}：", "").Trim();
                if (float.TryParse(valueStr, out float value))
                {
                    return value;
                }
            }
        }
        return 0f;
    }


    private static List<SkillAttributes> ConvertToSkillAttributes(ClassManager.CharacterData characterData)
    {
        var skills = new List<SkillAttributes>();

        if (characterData.talent1 != null)
        {
            skills.Add(CreateSkillFromTalent(characterData.talent1, characterData.talent_count1));
        }
        if (characterData.talent2 != null)
        {
            skills.Add(CreateSkillFromTalent(characterData.talent2, characterData.talent_count2));
        }
        if (characterData.talent3 != null)
        {
            skills.Add(CreateSkillFromTalent(characterData.talent3, characterData.talent_count3));
        }

        return skills;
    }

    private static SkillAttributes CreateSkillFromTalent(ClassManager.Talent talent, List<string> talentCount)
    {
        return new SkillAttributes
        {
            skillName = talent.description != null && talent.description.Count > 0 ? talent.description[0].talent_name : "未知技能",
            skillDescription = talent.description != null && talent.description.Count > 0 ? talent.description[0].talent_description : "无描述",
            skillCost = int.TryParse(talent.cost, out var cost) ? cost : 0,
            skillIcon = talent.icon1,
            skillVector = talentCount?.ConvertAll(float.Parse) ?? new List<float>(),

            // 解析技能的各项属性
            physicalDamage = talentCount != null && talentCount.Count > 0 ? float.Parse(talentCount[0]) : 0f,
            soulDamage = talentCount != null && talentCount.Count > 1 ? float.Parse(talentCount[1]) : 0f,
            stunChance = talentCount != null && talentCount.Count > 2 ? float.Parse(talentCount[2]) : 0f,
            silenceChance = talentCount != null && talentCount.Count > 3 ? float.Parse(talentCount[3]) : 0f,
            criticalBoost = talentCount != null && talentCount.Count > 4 ? float.Parse(talentCount[4]) : 0f,
            shieldAmount = talentCount != null && talentCount.Count > 5 ? float.Parse(talentCount[5]) : 0f,
            blockChance = talentCount != null && talentCount.Count > 6 ? float.Parse(talentCount[6]) : 0f,
            healAmount = talentCount != null && talentCount.Count > 7 ? float.Parse(talentCount[7]) : 0f
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
    
    
    public static MonsterAttributes ConvertToMonsterAttributes(ClassManager.MonsterData monsterData)
    {
        var monsterAttributes = new MonsterAttributes
        {
            id = monsterData.id,
            characterName = monsterData.enemy,
            monsterId = monsterData.monsterId,
            level = monsterData.level,
            health = monsterData.hp,
            sanValue = monsterData.san_value,
            physicalAttack = monsterData.physical_attack,
            physicalDefense = monsterData.physical_defense,
            soulAttack = monsterData.soul_attack,
            soulDefense = monsterData.soul_defense,
            speed = monsterData.speed + 100f,
            criticalRate = monsterData.critical_strike_rate,
            hitRate = monsterData.hit_rate,
            tenacityRate = monsterData.tenacity_rate,
            skills = new List<MonsterSkillAttributes>(),
            timePoint = 0f,
            energy = 0,
            maxEnergy = 10,
            damageX1 = 1f,
            damageX2 = 1f,
            currentHealth = monsterData.hp,
            
        };

        if (!string.IsNullOrEmpty(monsterData.skill1) && monsterData.skill1_vector != null)
        {
            monsterAttributes.skills.Add(ParseSkill(monsterData.skill1, monsterData.skill1_desc, monsterData.skill1_vector, monsterData.skill1_cost, monsterData.skill1_icon));
        }

        if (!string.IsNullOrEmpty(monsterData.skill2) && monsterData.skill2_vector != null)
        {
            monsterAttributes.skills.Add(ParseSkill(monsterData.skill2, monsterData.skill2_desc, monsterData.skill2_vector, monsterData.skill2_cost, monsterData.skill2_icon));
        }

        if (!string.IsNullOrEmpty(monsterData.skill3) && monsterData.skill3_vector != null)
        {
            monsterAttributes.skills.Add(ParseSkill(monsterData.skill3, monsterData.skill3_desc, monsterData.skill3_vector, monsterData.skill3_cost, monsterData.skill3_icon));
        }

        if (!string.IsNullOrEmpty(monsterData.skill4) && monsterData.skill4_vector != null)
        {
            monsterAttributes.skills.Add(ParseSkill(monsterData.skill4, monsterData.skill4_desc, monsterData.skill4_vector, monsterData.skill4_cost, monsterData.skill4_icon));
        }

        return monsterAttributes;
    }

    private static MonsterSkillAttributes ParseSkill(string skillName, string skillDescription, List<string> skillVector, int? skillCost, string skillIcon)
    {
        if (skillVector == null || skillVector.Count < 17)
        {
            Debug.LogError($"技能 {skillName} 数据不完整！");
            return null;
        }

        float TryParseFloat(string value, float defaultValue = 0f)
        {
            return float.TryParse(value, out float result) ? result : defaultValue;
        }

        int TryParseInt(string value, int defaultValue = 0)
        {
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        return new MonsterSkillAttributes
        {
            skillName = skillName,
            skillDescription = skillDescription,
            skillCost = skillCost ?? 0,
            skillIcon = skillIcon,
            skillValues = skillVector.ConvertAll(v => TryParseFloat(v)),

            physicalDamage = TryParseFloat(skillVector[0]),
            physicalAttackCount = TryParseFloat(skillVector[1]),
            soulDamage = TryParseFloat(skillVector[2]),
            soulAttackCount = TryParseFloat(skillVector[3]),
            sanityLoss = TryParseFloat(skillVector[4]),
            targetAlly = TryParseFloat(skillVector[5]) > 0,
            targetSelf = TryParseFloat(skillVector[6]) > 0,
            targetCount = TryParseFloat(skillVector[7]),
            criticalChance = TryParseFloat(skillVector[8]),
            shieldValue = TryParseFloat(skillVector[9]),
            blockChance = TryParseFloat(skillVector[10]),
            fixedLifesteal = TryParseFloat(skillVector[11]),
            percentLifesteal = TryParseFloat(skillVector[12]),
            fixedHeal = TryParseFloat(skillVector[13]),
            percentHeal = TryParseFloat(skillVector[14]),
            stunChance = TryParseFloat(skillVector[15]),
            silenceChance = TryParseFloat(skillVector[16])
        };
    }



}