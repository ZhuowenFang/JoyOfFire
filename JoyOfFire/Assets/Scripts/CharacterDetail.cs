using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // UpdateText(0);

        slider.onValueChanged.AddListener(UpdateText);
        confirmButton.onClick.AddListener(LevelUp);
        
        
    }

    void Update()
    {
        if (currentCharacter!= null)
        {
            AssignAttributeButton.interactable = currentCharacter.attributePoints > 0;
        }
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
    }
    
    public void ShowCharacterDetails(CharacterAttributes character)
    {
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
 
    
        StartCoroutine(APIManager.instance.LoadImage(character.character_picture, characterImage));
        // Role.text = character.basic_information.profession;
        
        for (int i = 0; i < character.skills.Count; i++)
        {
            if (!String.IsNullOrEmpty(character.skills[i].skillIcon))
            {
                StartCoroutine(APIManager.instance.LoadImage(character.skills[i].skillIcon, skillIcons[i]));
                skillNameTexts[i].text = character.skills[i].skillName;
                skillDescriptionTexts[i].text = character.skills[i].skillDescription;
            }
        }
    }
    

     public void UpdateCharacterDetails(CharacterAttributes character)
    {
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
    }

    
    
}
