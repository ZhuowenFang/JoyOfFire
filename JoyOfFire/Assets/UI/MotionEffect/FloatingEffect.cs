using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("浮动参数")]
    public float floatAmplitudeY = 20f;   // 上下浮动幅度
    public float floatAmplitudeX = 10f;   // 左右浮动幅度
    public float floatSpeed = 1f;         // 浮动速度（频率）

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

        float elapsed = Time.realtimeSinceStartup - startTimeOffset;

        float offsetY = Mathf.Sin(elapsed * floatSpeed) * floatAmplitudeY;
        float offsetX = Mathf.Cos(elapsed * floatSpeed * 0.8f) * floatAmplitudeX;

        rectTransform.anchoredPosition = startAnchoredPos + new Vector2(offsetX, offsetY);
    }

    public void RestartFloating()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        startAnchoredPos = rectTransform.anchoredPosition;
        startTimeOffset = Time.realtimeSinceStartup;
        isFloating = true;
    }

    public void StopFloating()
    {
        isFloating = false;
        rectTransform.anchoredPosition = startAnchoredPos;
    }
}

