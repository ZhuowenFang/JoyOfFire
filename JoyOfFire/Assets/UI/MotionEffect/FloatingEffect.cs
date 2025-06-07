using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("浮动参数")]
    public float floatAmplitudeY = 20f;     // 上下浮动的像素幅度
    public float floatAmplitudeX = 10f;     // 左右摆动的像素幅度
    public float floatSpeed = 1f;           // 浮动速度

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitudeY;
        float offsetX = Mathf.Cos(Time.time * floatSpeed * 0.8f) * floatAmplitudeX;
        rectTransform.anchoredPosition = new Vector2(startAnchoredPos.x + offsetX, startAnchoredPos.y + offsetY);
    }
}