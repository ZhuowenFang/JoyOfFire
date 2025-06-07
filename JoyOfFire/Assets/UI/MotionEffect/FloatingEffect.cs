using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("��������")]
    public float floatAmplitudeY = 20f;     // ���¸��������ط���
    public float floatAmplitudeX = 10f;     // ���Ұڶ������ط���
    public float floatSpeed = 1f;           // �����ٶ�

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