using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttributeManager : MonoBehaviour
{
    public Slider strengthSlider;
    public Slider agilitySlider;
    public Slider intelligenceSlider;

    public TMP_Text strengthText;
    public TMP_Text agilityText;
    public TMP_Text intelligenceText;

    public TMP_Text totalPointsText;
    
    public TMP_Text currentStrengthText;
    public TMP_Text currentAgilityText;
    public TMP_Text currentIntelligenceText;

    private int totalPoints;
    private int allocatedPoints = 0;

    private float lastStrengthValue;
    private float lastAgilityValue;
    private float lastIntelligenceValue;
    private CharacterAttributes currentCharacter;

    private float strengthChangeValuePerPoint;
    private float agilityChangeValuePerPoint;
    private float intelligenceChangeValuePerPoint;
    public Button confirmButton;
    public GameObject ATResultPanel;
    public TMP_Text ATResultStrengthText;
    public TMP_Text ATResultAgilityText;
    public TMP_Text ATResultIntelligenceText;

    void Start()
    {
        Initial();

        
        strengthSlider.onValueChanged.AddListener(delegate { OnSliderChanged(strengthSlider, ref lastStrengthValue); });
        agilitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(agilitySlider, ref lastAgilityValue); });
        intelligenceSlider.onValueChanged.AddListener(delegate { OnSliderChanged(intelligenceSlider, ref lastIntelligenceValue); });
        
        confirmButton.onClick.AddListener(ConfirmAttributeAllocation);
        
    }

    public void Initial()
    {
        currentCharacter = CharacterDetail.instance.currentCharacter;
        currentStrengthText.text = currentCharacter.strength.ToString("0.0");
        currentAgilityText.text = currentCharacter.agility.ToString("0.0");
        currentIntelligenceText.text = currentCharacter.intelligence.ToString("0.0");
        
        
        totalPoints = currentCharacter.attributePoints;
        
        lastStrengthValue = strengthSlider.value;
        lastAgilityValue = agilitySlider.value;
        lastIntelligenceValue = intelligenceSlider.value;
        strengthChangeValuePerPoint = (currentCharacter.potentialStrength - currentCharacter.initialStrength) / 20;
        agilityChangeValuePerPoint = (currentCharacter.potentialAgility - currentCharacter.initialAgility) / 20;
        intelligenceChangeValuePerPoint = (currentCharacter.potentialIntelligence - currentCharacter.initialIntelligence) / 20;
        
        strengthSlider.maxValue = (currentCharacter.potentialStrength - currentCharacter.strength) / strengthChangeValuePerPoint;
        agilitySlider.maxValue = (currentCharacter.potentialAgility - currentCharacter.agility) / agilityChangeValuePerPoint;
        intelligenceSlider.maxValue = (currentCharacter.potentialIntelligence - currentCharacter.intelligence) / intelligenceChangeValuePerPoint;
        UpdateUI();

        
    }
    public void ReAssignSliders()
    {
        strengthSlider.value = 0;
        agilitySlider.value = 0;
        intelligenceSlider.value = 0;
        lastStrengthValue = strengthSlider.value;
        lastAgilityValue = agilitySlider.value;
        lastIntelligenceValue = intelligenceSlider.value;
    }

    private void Update()
    {
        if (allocatedPoints == 0)
        {
            confirmButton.interactable = false;
        }
        else
        {
            confirmButton.interactable = true;
        }
    }

    void OnSliderChanged(Slider slider, ref float lastValue)
    {
        int newAllocatedPoints = (int)(strengthSlider.value + agilitySlider.value + intelligenceSlider.value);
        int remainingPoints = totalPoints - newAllocatedPoints;

        if (remainingPoints < 0 && slider.value > lastValue)
        {
            slider.value = lastValue;
        }
        else
        {
            lastValue = slider.value;
        }
        UpdateUI();
    }


    void UpdateUI()
    {
        allocatedPoints = (int)(strengthSlider.value + agilitySlider.value + intelligenceSlider.value);

        strengthText.text = "+ " + (strengthSlider.value * strengthChangeValuePerPoint).ToString("0.0");
        agilityText.text = "+ " + (agilitySlider.value * agilityChangeValuePerPoint).ToString("0.0");
        intelligenceText.text = "+ " + (intelligenceSlider.value * intelligenceChangeValuePerPoint).ToString("0.0");

        totalPointsText.text = "共拥有" + totalPoints.ToString("0") + "个属性点，已分配" + allocatedPoints.ToString("0") + "个属性点。";

    }
    
    void ConfirmAttributeAllocation()
    {
        if (allocatedPoints == 0)
        {
            Debug.LogError("No attribute points allocated.");
            return;
        }
        float currentStrength = currentCharacter.strength;
        float currentAgility = currentCharacter.agility;
        float currentIntelligence = currentCharacter.intelligence;
        
        
        
        currentCharacter.strength += strengthSlider.value * strengthChangeValuePerPoint;
        Debug.Log($"Strength: {currentCharacter.strength}");
        currentCharacter.agility += agilitySlider.value * agilityChangeValuePerPoint;
        Debug.Log($"Agility: {currentCharacter.agility}");
        currentCharacter.intelligence += intelligenceSlider.value * intelligenceChangeValuePerPoint;
        Debug.Log($"Intelligence: {currentCharacter.intelligence}");
        currentCharacter.attributePoints -= allocatedPoints;
        totalPoints = currentCharacter.attributePoints;
        ReAssignSliders();

        CharacterDetail.instance.UpdateCharacterDetails(currentCharacter);
        
        ATResultPanel.SetActive(true);
        if (currentStrength == currentCharacter.strength)
        {
            ATResultStrengthText.text = "力量      " + currentStrength.ToString("0.0");
        }
        else
        {
            ATResultStrengthText.text = "力量      " + currentStrength.ToString("0.0") + "    >>>    " + currentCharacter.strength.ToString("0.0");
        }
        if (currentAgility == currentCharacter.agility)
        {
            ATResultAgilityText.text = "敏捷      " + currentAgility.ToString("0.0");
        }
        else
        {
            ATResultAgilityText.text = "敏捷      " + currentAgility.ToString("0.0") + "    >>>    " + currentCharacter.agility.ToString("0.0");
        }
        if (currentIntelligence == currentCharacter.intelligence)
        {
            ATResultIntelligenceText.text = "智力      " + currentIntelligence.ToString("0.0");
        }
        else
        {
            ATResultIntelligenceText.text = "智力      " + currentIntelligence.ToString("0.0") + "    >>>    " + currentCharacter.intelligence.ToString("0.0");
        }
        
    }

}
