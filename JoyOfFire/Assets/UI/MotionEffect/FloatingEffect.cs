using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("浮动参数")]
    public float floatAmplitudeY = 20f;   // 上下浮动幅度（像素）
    public float floatAmplitudeX = 10f;   // 左右浮动幅度（像素）
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

        float elapsed = Time.time - startTimeOffset;

        float offsetY = Mathf.Sin(elapsed * floatSpeed) * floatAmplitudeY;
        float offsetX = Mathf.Cos(elapsed * floatSpeed * 0.8f) * floatAmplitudeX;

        rectTransform.anchoredPosition = startAnchoredPos + new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// 重新启动浮动效果（重置位置和计时）
    /// </summary>
    public void RestartFloating()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        startAnchoredPos = rectTransform.anchoredPosition;
        startTimeOffset = Time.time;
        isFloating = true;
    }

    /// <summary>
    /// 停止浮动效果，恢复起始位置
    /// </summary>
    public void StopFloating()
    {
        isFloating = false;
        rectTransform.anchoredPosition = startAnchoredPos;
    }
}

