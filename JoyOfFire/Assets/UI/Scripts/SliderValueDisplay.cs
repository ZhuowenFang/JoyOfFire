using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider slider;  // ��ק��Slider
    public TMP_Text valueText; // ��ק��Text

    void Start()
    {
        // ��ʼ���ı�
        UpdateText(slider.value);

        // ����Slider��ֵ�仯
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float value)
    {
        valueText.text = "����" + value.ToString("0") + "���������ߣ�����Ա������  �������  ���Ե㡣"; 
    }
}
