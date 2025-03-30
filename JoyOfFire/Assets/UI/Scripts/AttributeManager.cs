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

    private int totalPoints = 20;  // 总属性点数
    private int allocatedPoints = 0;  // 已分配的点数

    private float lastStrengthValue;
    private float lastAgilityValue;
    private float lastIntelligenceValue;

    void Start()
    {
        // 初始化文本
        lastStrengthValue = strengthSlider.value;
        lastAgilityValue = agilitySlider.value;
        lastIntelligenceValue = intelligenceSlider.value;
        UpdateUI();

        // 监听 Slider 变化
        strengthSlider.onValueChanged.AddListener(delegate { OnSliderChanged(strengthSlider, ref lastStrengthValue, strengthText, "力量"); });
        agilitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(agilitySlider, ref lastAgilityValue, agilityText, "敏捷"); });
        intelligenceSlider.onValueChanged.AddListener(delegate { OnSliderChanged(intelligenceSlider, ref lastIntelligenceValue, intelligenceText, "智力"); });
    }

    void OnSliderChanged(Slider slider, ref float lastValue, TMP_Text text, string attributeName)
    {
        // 计算已分配的点数
        int newAllocatedPoints = (int)(strengthSlider.value + agilitySlider.value + intelligenceSlider.value);
        int remainingPoints = totalPoints - newAllocatedPoints;

        // 如果剩余点数不足，并且玩家试图增加值，则恢复上次的值
        if (remainingPoints < 0 && slider.value > lastValue)
        {
            slider.value = lastValue;  // 还原为上一次的值
        }
        else
        {
            lastValue = slider.value;  // 记录新值
        }
        UpdateUI();
    }


    void UpdateUI()
    {
        // 计算已分配的点数
        allocatedPoints = (int)(strengthSlider.value + agilitySlider.value + intelligenceSlider.value);
        int remainingPoints = totalPoints - allocatedPoints;

        // 更新属性值文本
        strengthText.text = "+ " + strengthSlider.value.ToString("0");
        agilityText.text = "+ " + agilitySlider.value.ToString("0");
        intelligenceText.text = "+ " + intelligenceSlider.value.ToString("0");

        // 更新总属性点和已分配点数
        totalPointsText.text = "共拥有" + totalPoints.ToString("0") + "个属性点，已分配" + allocatedPoints.ToString("0") + "个属性点。";

    }

}
