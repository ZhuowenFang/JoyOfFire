using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider slider;  // 拖拽绑定Slider
    public TMP_Text valueText; // 拖拽绑定Text

    void Start()
    {
        // 初始化文本
        UpdateText(slider.value);

        // 监听Slider的值变化
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float value)
    {
        valueText.text = "消耗" + value.ToString("0") + "个升级道具，调查员升级到  级，获得  属性点。"; 
    }
}
