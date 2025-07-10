using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("��������")]
    public float floatAmplitudeY = 20f;   // ���¸������ȣ����أ�
    public float floatAmplitudeX = 10f;   // ���Ҹ������ȣ����أ�
    public float floatSpeed = 1f;         // �����ٶȣ�Ƶ�ʣ�

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;
    private float startTimeOffset;
    private bool isFloating = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        RestartFloating();
    }

    void Update()
    {
        if (!isFloating) return;

        float elapsed = Time.time - startTimeOffset;

        float offsetY = Mathf.Sin(elapsed * floatSpeed) * floatAmplitudeY;
        float offsetX = Mathf.Cos(elapsed * floatSpeed * 0.8f) * floatAmplitudeX;

        rectTransform.anchoredPosition = startAnchoredPos + new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// ������������Ч��������λ�úͼ�ʱ��
    /// </summary>
    public void RestartFloating()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        startAnchoredPos = rectTransform.anchoredPosition;
        startTimeOffset = Time.time;
        isFloating = true;
    }

    /// <summary>
    /// ֹͣ����Ч�����ָ���ʼλ��
    /// </summary>
    public void StopFloating()
    {
        isFloating = false;
        rectTransform.anchoredPosition = startAnchoredPos;
    }
}

