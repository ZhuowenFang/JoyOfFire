using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetail : MonoBehaviour
{
    
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
    public Image levelBar;
    public Image StrengthBar;
    public Image AgilityBar;
    public Image IntelligenceBar;
    public Text AttributePointText;
    public List<Image> skillIcons;
    public List<Text> skillNameTexts;
    public List<Text> skillDescriptionTexts;
    public int levelItemCount;
    public TMP_Text LevelItemAmountText;
    public TMP_Text levelItemIconAmountText;
    public Slider slider;
    public TMP_Text valueText;
    public Button confirmButton;
    
    public GameObject UPResultPanel;

    
    public CharacterAttributes currentCharacter;
    
    public TMP_Text UPResultText;

    public static CharacterDetail instance;
    
    public Button AssignAttributeButton;
    
    public Button UpdateButton;
    
    public int BreakThroughItemCount;
    public GameObject UpdatePanel;
    
    public List<Button> UpdateOptions;
    
    public Button confirmUpdateButton;
    
    public GameObject UpdateResultPanel;
    
    private Button selectedUpdateOption = null;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.green;
    
    private int SelectedIndex = 0;

    
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        slider.onValueChanged.AddListener(UpdateText);
        confirmButton.onClick.AddListener(LevelUp);
        UpdateButton.onClick.AddListener(() =>
        {
            UpdatePanel.SetActive(true);
        });
        confirmUpdateButton.onClick.AddListener(BreakThrough);
        confirmUpdateButton.interactable = false;
        foreach (Button option in UpdateOptions)
        {
            option.onClick.AddListener(() =>
            {
                if (selectedUpdateOption != null)
                {
                    selectedUpdateOption.image.color = defaultColor;
                }
                selectedUpdateOption = option;
                selectedUpdateOption.image.color = selectedColor;
                confirmUpdateButton.interactable = true;
                SelectedIndex = UpdateOptions.IndexOf(option);
            });
        }        
        
    }

    void Update()
    {
        if (currentCharacter!= null)
        {
            AssignAttributeButton.interactable = currentCharacter.attributePoints > 0;
            UpdateButton.interactable = BreakThroughItemCount > 0 && currentCharacter.level >= 5;
        }
    }

    public void BreakThrough()
    {
        String UpdateType = "";
        switch (SelectedIndex)
        {
            case 0:
                UpdateType = "A";
                break;
            case 1:
                UpdateType = "B";
                break;
            case 2:
                UpdateType = "C";
                break;
            case 3:
                UpdateType = "D";
                break;
        }
        
        ClassManager.CharacterUpdateData characterUpdateData = new ClassManager.CharacterUpdateData
        {
            user_id = currentCharacter.user_id,
            character_id = currentCharacter.character_id,
            Update = UpdateType
        };
        
        APIManager.instance.UpdateCharacter(
            JsonUtility.ToJson(characterUpdateData),
            onSuccess: (response) =>
            {
                Debug.Log($"Character Updated: {response}");
                UpdateCharacterSkills(response);
            },
            onError: (error) =>
            {
                Debug.LogError($"Error deleting character: {error}");
            }
        );
        
        UpdateResultPanel.SetActive(true);
        UpdatePanel.SetActive(false);
        foreach (Button option in UpdateOptions)
        {
            option.image.color = defaultColor;
            confirmUpdateButton.interactable = false;
        }
        selectedUpdateOption = null;
    }
    
    void UpdateText(float value)
    {
        if (value == 0)
        {
            confirmButton.interactable = false;
        }
        else
        {
            confirmButton.interactable = true;
        }
        
        String currentLevel = (currentCharacter.level + value).ToString();
        valueText.text = "消耗" + value.ToString("0") + "个升级道具，调查员升级到" + currentLevel + "级，获得" + value.ToString("0") + "点属性点"; 
    }
    
    private void LevelUp()
    {
        if (currentCharacter == null)
        {
            Debug.LogError("currentCharacter is null");
            return;
        }
        if (slider.value == 0)
        {
            Debug.LogError("slider value is 0");
            return;
        }
        int currentLevel = currentCharacter.level;
        currentCharacter.level += (int)slider.value;
        currentCharacter.attributePoints += (int)slider.value;
        UPResultPanel.SetActive(true);
        UPResultText.text = "等级提升：         " + currentLevel + "  >>>  " + currentCharacter.level;
        InventoryManager.instance.RemoveItem("Level_boost", (int)slider.value);
        UpdateCharacterDetails(currentCharacter);
        slider.value = 0;

        
    }
    
    public void UpdateLevelBoostItemCount()
    {
        levelItemCount = InventoryManager.instance.GetItemCount("Level_boost");
        LevelItemAmountText.text = levelItemCount.ToString();
        slider.maxValue = levelItemCount;
        levelItemIconAmountText.text = LevelItemAmountText.text;
        BreakThroughItemCount = InventoryManager.instance.GetItemCount("Break_through");
    }
    
    public void UpdateBreakThroughOptionButtonStatus()
    {
        int skillCount = currentCharacter.skills.Count;
        int buttonCountForSkills = UpdateOptions.Count - 1;

        for (int i = 0; i < buttonCountForSkills; i++)
        {
            if (i < skillCount)
            {
                UpdateOptions[i].interactable = !string.IsNullOrEmpty(currentCharacter.skills[i].skillName);
            }
            else
            {
                UpdateOptions[i].interactable = false;
            }
        }

        bool anySkillEmpty = currentCharacter.skills.Count < 3;
        UpdateOptions[UpdateOptions.Count - 1].interactable = anySkillEmpty;
    }

    public void ShowCharacterDetails(CharacterAttributes character)
    {
        if (character.characterName == "狂风医生")
        {
            UpdateButton.gameObject.SetActive(false);
        }
        else
        {
            UpdateButton.gameObject.SetActive(true);
        }
        levelItemCount = InventoryManager.instance.GetItemCount("Level_boost");
        LevelItemAmountText.text = levelItemCount.ToString();
        slider.maxValue = levelItemCount;
        levelItemIconAmountText.text = LevelItemAmountText.text;
        currentCharacter = character;
        characterNameText.text = character.basic_information.name;
        characterStoryText.text = "背景故事：" + character.basic_information.story;
        experienceText.text = "经历：" + character.experience;
        levelText.text = character.level.ToString();
        levelBar.fillAmount = character.level / 25f;
        strengthText.text = character.strength.ToString("0.0");;
        agilityText.text = character.agility.ToString("0.0");;
        intelligenceText.text = character.intelligence.ToString("0.0");;
        healthText.text = character.health.ToString();
        physicalAttackText.text = character.physicalAttack.ToString();
        physicalDefenseText.text = character.physicalDefense.ToString();
        soulAttackText.text = character.soulAttack.ToString();
        soulDefenseText.text = character.soulDefense.ToString();
        speedText.text = character.speed.ToString();
        criticalRateText.text = character.criticalRate * 100 + "%";
        hitRateText.text = character.hitRate * 100 + "%";
        tenacityRateText.text = character.tenacityRate * 100 + "%";
        StrengthBar.fillAmount = character.strength / character.potentialStrength;
        AgilityBar.fillAmount = character.agility / character.potentialAgility;
        IntelligenceBar.fillAmount = character.intelligence / character.potentialIntelligence;
        Gender.text = character.basic_information.gender;
        AttributePointText.text = character.attributePoints.ToString();
 
        UpdateBreakThroughOptionButtonStatus();
        // StartCoroutine(APIManager.instance.LoadImage(character.character_picture, characterImage));
        StartCoroutine(ImageCache.GetTexture(character.character_picture, (Texture2D texture) =>
        {
            if (texture != null)
            {
                characterImage.sprite = Sprite.Create(texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(0.5f, 0.5f));
            }
        }));
        // Role.text = character.basic_information.profession;
        
        for (int i = 0; i < character.skills.Count; i++)
        {
            int index = i; // 捕获当前循环变量的副本
            if (!string.IsNullOrEmpty(character.skills[index].skillName))
            {
                skillNameTexts[index].text = character.skills[index].skillName;
                skillDescriptionTexts[index].text = character.skills[index].skillDescription;
                if (!string.IsNullOrEmpty(character.skills[index].skillIcon))
                {
                    StartCoroutine(ImageCache.GetTexture(character.skills[index].skillIcon, (Texture2D texture) =>
                    {
                        if (texture != null)
                        {
                            Debug.LogError(index);
                            skillIcons[index].sprite = Sprite.Create(texture,
                                new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f));
                        }
                    }));
                }
            }
        }
    }
    

     public void UpdateCharacterDetails(CharacterAttributes character)
    {
        if (character.characterName == "狂风医生")
        {
            UpdateButton.gameObject.SetActive(false);
        }
        else
        {
            UpdateButton.gameObject.SetActive(true);
        }
        currentCharacter = character;
        characterNameText.text = character.basic_information.name;
        characterStoryText.text = "背景故事：" + character.basic_information.story;
        experienceText.text = "经历：" + character.experience;
        levelText.text = character.level.ToString();
        levelBar.fillAmount = character.level / 25f;

        strengthText.text = character.strength.ToString("0.0");
        agilityText.text = character.agility.ToString("0.0");
        intelligenceText.text = character.intelligence.ToString("0.0");


        character.soulAttack  = character.intelligence * 20f;
        character.soulDefense = character.intelligence * 5f;
        character.hitRate     = character.intelligence * 0.01f;
        float healthFromInt   = character.intelligence * 30f;

        float physicalAttackFromAgi = character.agility * 10f;
        float criticalRateFromAgi   = character.agility * 0.01f;
        float speedFromAgi          = character.agility;
        float healthFromAgi         = character.agility * 20f;

        float physicalAttackFromStr = character.strength * 5f;
        character.physicalDefense   = character.strength * 15f;
        character.tenacityRate      = character.strength * 0.01f;
        float healthFromStr         = character.strength * 50f;

        character.physicalAttack = physicalAttackFromAgi + physicalAttackFromStr;
        character.criticalRate = criticalRateFromAgi;
        character.speed        = speedFromAgi + 100;

        character.health = healthFromInt + healthFromAgi + healthFromStr + character.additionalHealth;

        healthText.text = character.health.ToString("0");
        physicalAttackText.text = character.physicalAttack.ToString("0");
        physicalDefenseText.text = character.physicalDefense.ToString("0");
        soulAttackText.text = character.soulAttack.ToString("0");
        soulDefenseText.text = character.soulDefense.ToString("0");
        speedText.text = character.speed.ToString("0");
        criticalRateText.text = (character.criticalRate * 100).ToString("F0") + "%";
        hitRateText.text = (character.hitRate * 100).ToString("F0") + "%";
        tenacityRateText.text = (character.tenacityRate * 100).ToString("F0") + "%";

        StrengthBar.fillAmount = character.strength / character.potentialStrength;
        AgilityBar.fillAmount = character.agility / character.potentialAgility;
        IntelligenceBar.fillAmount = character.intelligence / character.potentialIntelligence;
        Gender.text = character.basic_information.gender;
        AttributePointText.text = character.attributePoints.ToString();
        levelItemCount = InventoryManager.instance.GetItemCount("Level_boost");
        LevelItemAmountText.text = levelItemCount.ToString();
        slider.maxValue = levelItemCount;
        levelItemIconAmountText.text = LevelItemAmountText.text;
        for (int i = 0; i < character.skills.Count; i++)
        {
            if (!String.IsNullOrEmpty(character.skills[i].skillName))
            {
                skillNameTexts[i].text = character.skills[i].skillName;
                skillDescriptionTexts[i].text = character.skills[i].skillDescription;
                if (!String.IsNullOrEmpty(character.skills[i].skillIcon))
                {
                    // StartCoroutine(APIManager.instance.LoadImage(character.skills[i].skillIcon, skillIcons[i]));
                    StartCoroutine(ImageCache.GetTexture(character.skills[i].skillIcon, (Texture2D texture) =>
                    {
                        if (texture != null)
                        {
                            skillIcons[i].sprite = Sprite.Create(texture, 
                                new Rect(0, 0, texture.width, texture.height), 
                                new Vector2(0.5f, 0.5f));
                        }
                    }));
                }

            }
        }
        UpdateBreakThroughOptionButtonStatus();
    }

    public void UpdateCharacterSkills(string response)
    {
        var characterResponse = JsonConvert.DeserializeObject<CharacterCreation.CharacterAttributesResponse>(response);
        var updatedCharacterData = characterResponse.data;
        string updatedCharacterId = updatedCharacterData.character_id;
    
        CharacterAttributes existingCharacter = NewCharacterManager.instance.allCharacters
            .OfType<CharacterAttributes>()
            .FirstOrDefault(c => c.character_id == updatedCharacterId);
        if (existingCharacter == null)
        {
            Debug.LogWarning($"未找到 character_id 为 {updatedCharacterId} 的角色，技能更新失败。");
            return;
        }
    
        List<SkillAttributes> updatedSkills = NewCharacterManager.ConvertToSkillAttributes(updatedCharacterData);
    
        existingCharacter.skills = updatedSkills;
    
        UpdateCharacterDetails(existingCharacter);
    
    }

    
    
}
