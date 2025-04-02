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
        // ����һ�������ڵı仯ֵ����0��1֮�䣩
        float lerpValue = Mathf.PingPong(Time.time / cycleTime, 1);
        // ͨ��Lerp���㽥���Alphaֵ��ģ������Ƶ�Ч��
        Color currentColor = startColor;
        currentColor.a = Mathf.Lerp(0f, 1f, lerpValue);
        image.color = currentColor;
    }
}
