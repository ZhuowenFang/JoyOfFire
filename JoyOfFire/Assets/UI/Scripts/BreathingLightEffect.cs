using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingLightEffect : MonoBehaviour
{
    public Image image;  
    public float cycleTime = 2f;
    private Color startColor;
    private float timer = 0f;
    void Start()
    {
        image = GetComponent<Image>();
        startColor = image.color;
    }
    void Update()
    {
        // 计算一个周期内的变化值（从0到1之间）
        float lerpValue = Mathf.PingPong(Time.time / cycleTime, 1);
        // 通过Lerp计算渐变的Alpha值，模拟呼吸灯的效果
        Color currentColor = startColor;
        currentColor.a = Mathf.Lerp(0f, 1f, lerpValue);
        image.color = currentColor;
    }
}
