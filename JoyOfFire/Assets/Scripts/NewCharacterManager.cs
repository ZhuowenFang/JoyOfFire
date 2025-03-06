using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterManager : MonoBehaviour
{
    [SerializeReference]
    public List<ICharacter> allCharacters = new List<ICharacter>();
    public List<Button> characterIndexButtons;
    public GameObject characterDetailPanel;
    public GameObject characterCreationPanel;
    public GameObject waitPanel;
    public GameObject characterFeaturePanel;
    
    public static NewCharacterManager instance;
    public bool creatingCharacter = false;
    
    public GameObject RewardPanel;
    
    public Button ReplaceButton;
    public Button ReplaceCancelButton;
    public GameObject ReplacePanel;
    public GameObject CharacterRepalcementLayout;
    public GameObject replaceCharacterPrefab;
    public Button replaceConfirmButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        createInitialCharacter();
        InitializeButtons();
        ReplaceButton.onClick.AddListener(replaceAndCreateCharacter);
                
    }
    
    
    void Update()
    {
        ReplaceButton.interactable = allCharacters.Count == 3;
    }
    public void replaceAndCreateCharacter()
    {
        ReplaceButton.gameObject.SetActive(false);
        ReplacePanel.SetActive(true);
        foreach (Transform child in CharacterRepalcementLayout.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (CharacterAttributes character in allCharacters)
        {
            GameObject newCharacter = Instantiate(replaceCharacterPrefab, CharacterRepalcementLayout.transform);
            newCharacter.transform.localScale = new Vector3(3f, 3f, 3f);
            // StartCoroutine(APIManager.instance.LoadImage(character.character_picture, newCharacter.GetComponent<Image>()));
            StartCoroutine(ImageCache.GetTexture(character.character_picture, (Texture2D texture) =>
            {
                if (texture != null)
                {
                    newCharacter.GetComponent<Image>().sprite = Sprite.Create(texture, 
                        new Rect(0, 0, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f));
                }
            }));
            newCharacter.transform.Find("Name").GetComponent<Text>().text = character.characterName;
            newCharacter.transform.Find("Level").GetComponent<Text>().text = "等级：" + character.level.ToString();
            ReplaceButtonGroup.instance.buttons.Add(newCharacter.GetComponent<Button>());
            ReplaceButtonGroup.instance.InitializeButtons();
            
        }

        replaceConfirmButton.onClick.AddListener(() =>
        {
            int index = ReplaceButtonGroup.instance.buttons.IndexOf(ReplaceButtonGroup.instance.selectedButton);
            characterCreationPanel.SetActive(true);
            characterFeaturePanel.SetActive(false);
            characterDetailPanel.SetActive(false);
            ReplacePanel.SetActive(false);
            CharacterAttributes removedCharacter = allCharacters[index] as CharacterAttributes;
            DeleteCharacter(removedCharacter);
        });
        
        ReplaceCancelButton.onClick.AddListener(() =>
        {
            ReplaceButton.gameObject.SetActive(true);
            ReplacePanel.SetActive(false);
            
        });


    }
    
    public void DeleteCharacter(ICharacter character)
    {
        
        allCharacters.Remove(character);
        InitializeButtons();
        characterCreationPanel.SetActive(true);
        characterFeaturePanel.SetActive(false);
        characterDetailPanel.SetActive(false);
    }

    public void createInitialCharacter()
    {
        string mockResponse = @"{
            ""basic_information"": {
                ""appearance"": ""身穿白大褂，手持听诊器，周围狂风呼啸"",
                ""fighting_ability"": ""狂风之力，可辅助可攻击"",
                ""gender"": ""男"",
                ""name"": ""狂风医生"",
                ""story"": ""拥有狂风之力的医生，在克苏鲁世界救死扶伤""
            },
            ""character_picture"": ""https://s.coze.cn/t/CnQK0oHlBLj415s2/"",
            ""current_ability"": [""- 智力：3"", ""- 力量：6"", ""- 敏捷：1""],
            ""potential_ability"": [""- 智力：25"", ""- 力量：23"", ""- 敏捷：15""],
            ""talent1"": {
                ""abilitydescription"": ""以风之力进行诊疗，造成 148.14% 的物理伤害，同时给自己增加 51.04 的护盾。"",
                ""cost"": ""2"",
                ""description"": [{
                    ""talent_description"": ""以风之力治疗与攻击"",
                    ""talent_name"": ""风暴诊疗""
                }],
                ""icon"": ""https://s.coze.cn/t/ClmrB2ygjB0IqGk8/""
            },
            ""talent_count1"": [""1.4814"", ""0.0000"", ""1"", ""1"", ""1"", ""51.0398"", ""0.0000"", ""0.0000""],
            ""talent2"": null,
            ""talent_count2"": null,
            ""talent3"": null,
            ""talent_count3"": null,
            ""experience"": null,
        }";
        
        var characterResponse = JsonConvert.DeserializeObject<ClassManager.CharacterData>(mockResponse);
        
        var character = ConvertToCharacterAttributes(characterResponse);
        character.user_id = "1";
        
        character.id = "1";
        character.character_id = "10";
        AddCharacter(character);
        
        CharacterDetail.instance.Role.text = "时光";        
    }
    public void InitializeButtons()
    {
        foreach (var button in characterIndexButtons)
        {
            int index = characterIndexButtons.IndexOf(button);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCharacterButtonClicked(index));
        }
        if (allCharacters.Count > 0)
        {
            characterCreationPanel.SetActive(false);
            characterFeaturePanel.SetActive(false);
            characterDetailPanel.SetActive(true);
            waitPanel.SetActive(false);
            CharacterDetail.instance.ShowCharacterDetails(allCharacters[0] as CharacterAttributes);
        }
        else
        {
            characterCreationPanel.SetActive(true);
            characterFeaturePanel.SetActive(false);
            characterDetailPanel.SetActive(false);
            // waitPanel.SetActive(false);

        }
    }

    private void OnCharacterButtonClicked(int index)
    {
        if (index < allCharacters.Count && allCharacters[index] != null)
        {
            characterCreationPanel.SetActive(false);
            characterDetailPanel.SetActive(true);
            characterFeaturePanel.SetActive(false);

            waitPanel.SetActive(false);
            CharacterDetail.instance.ShowCharacterDetails(allCharacters[index] as CharacterAttributes);
        }
        else
        {
            characterCreationPanel.SetActive(true);
            characterDetailPanel.SetActive(false);
            characterFeaturePanel.SetActive(false);

            // waitPanel.SetActive(false);

        }
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
            user_id = characterData.user_id,
            id = characterData.id,
            character_id = characterData.character_id,
            characterName = characterData.basic_information.name,
            intelligence = ExtractAttributeValue(characterData.current_ability, "智力"),
            initialIntelligence = ExtractAttributeValue(characterData.current_ability, "智力"),
            potentialIntelligence = ExtractAttributeValue(characterData.potential_ability, "智力"),
            agility = ExtractAttributeValue(characterData.current_ability, "敏捷"),
            initialAgility = ExtractAttributeValue(characterData.current_ability, "敏捷"),
            potentialAgility = ExtractAttributeValue(characterData.potential_ability, "敏捷"),
            strength = ExtractAttributeValue(characterData.current_ability, "力量"),
            initialStrength = ExtractAttributeValue(characterData.current_ability, "力量"),
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
            damageX1 = -20f,
            damageX2 = 300f,
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
            experience = characterData.experience,
            attributePoints = 0,
            sanValue = 0f,
            additionalHealth = 0f,
            star = 1,
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


    public static List<SkillAttributes> ConvertToSkillAttributes(ClassManager.CharacterData characterData)
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
            skillDescription = talent.description != null && talent.description.Count > 0 ? talent.abilitydescription : "无描述",
            skillCost = int.TryParse(talent.cost, out var cost) ? cost : 0,
            skillIcon = talent.icon,
            skillVector = talentCount?.ConvertAll(float.Parse) ?? new List<float>(),

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
            criticalRate = monsterData.critical_strike_rate / 100,
            hitRate = monsterData.hit_rate / 100,
            tenacityRate = monsterData.tenacity_rate,
            skills = new List<MonsterSkillAttributes>(),
            timePoint = 0f,
            energy = 0,
            maxEnergy = 10,
            damageX1 = -20f,
            damageX2 = 300f,
            currentHealth = monsterData.hp,
            base_gold_value = monsterData.base_gold_value,
            
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
            silenceChance = TryParseFloat(skillVector[16]),
            bindChance = TryParseFloat(skillVector[17]),
            bleedStacks = TryParseFloat(skillVector[18]),
            bleedChance = TryParseFloat(skillVector[19]),
            burnStacks = TryParseFloat(skillVector[20]),
            burnChance = TryParseFloat(skillVector[21]),
            poisonStacks = TryParseFloat(skillVector[22]),
            drunkStacks = TryParseFloat(skillVector[23]),
            gazeStacks = TryParseFloat(skillVector[24]),
            executeTarget = TryParseFloat(skillVector[25]) > 0,
            specialDish = TryParseFloat(skillVector[26]) > 0,
            knifeLimit = TryParseFloat(skillVector[27]),
            knifeHpCost = TryParseFloat(skillVector[28]),
            knifeConsume = TryParseFloat(skillVector[29]),
            damageBoost = TryParseFloat(skillVector[30]),
            taunt = TryParseFloat(skillVector[31]) > 0,
            stealth = TryParseFloat(skillVector[32]) > 0,
            skipTurn = TryParseFloat(skillVector[33]) > 0,
            reflectDamage = TryParseFloat(skillVector[34]) > 0,
            dispelDebuff = TryParseFloat(skillVector[35]) > 0,
        };

    }



}