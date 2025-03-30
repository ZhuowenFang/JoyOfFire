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

    private int totalPoints = 20;  // �����Ե���
    private int allocatedPoints = 0;  // �ѷ���ĵ���

    private float lastStrengthValue;
    private float lastAgilityValue;
    private float lastIntelligenceValue;

    void Start()
    {
        // ��ʼ���ı�
        lastStrengthValue = strengthSlider.value;
        lastAgilityValue = agilitySlider.value;
        lastIntelligenceValue = intelligenceSlider.value;
        UpdateUI();

        // ���� Slider �仯
        strengthSlider.onValueChanged.AddListener(delegate { OnSliderChanged(strengthSlider, ref lastStrengthValue, strengthText, "����"); });
        agilitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(agilitySlider, ref lastAgilityValue, agilityText, "����"); });
        intelligenceSlider.onValueChanged.AddListener(delegate { OnSliderChanged(intelligenceSlider, ref lastIntelligenceValue, intelligenceText, "����"); });
    }

    void OnSliderChanged(Slider slider, ref float lastValue, TMP_Text text, string attributeName)
    {
        // �����ѷ���ĵ���
        int newAllocatedPoints = (int)(strengthSlider.value + agilitySlider.value + intelligenceSlider.value);
        int remainingPoints = totalPoints - newAllocatedPoints;

        // ���ʣ��������㣬���������ͼ����ֵ����ָ��ϴε�ֵ
        if (remainingPoints < 0 && slider.value > lastValue)
        {
            slider.value = lastValue;  // ��ԭΪ��һ�ε�ֵ
        }
        else
        {
            lastValue = slider.value;  // ��¼��ֵ
        }
        UpdateUI();
    }


    void UpdateUI()
    {
        // �����ѷ���ĵ���
        allocatedPoints = (int)(strengthSlider.value + agilitySlider.value + intelligenceSlider.value);
        int remainingPoints = totalPoints - allocatedPoints;

        // ��������ֵ�ı�
        strengthText.text = "+ " + strengthSlider.value.ToString("0");
        agilityText.text = "+ " + agilitySlider.value.ToString("0");
        intelligenceText.text = "+ " + intelligenceSlider.value.ToString("0");

        // ���������Ե���ѷ������
        totalPointsText.text = "��ӵ��" + totalPoints.ToString("0") + "�����Ե㣬�ѷ���" + allocatedPoints.ToString("0") + "�����Ե㡣";

    }

}
