using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DoubleClickListener : MonoBehaviour, IPointerClickHandler
{
    public Action onDoubleClick;  // 双击时回调

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f; // 两次点击间隔阈值

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            // 触发双击回调
            onDoubleClick?.Invoke();
            lastClickTime = 0;  // 重置
        }
        else
        {
            lastClickTime = Time.time;
        }
    }
}