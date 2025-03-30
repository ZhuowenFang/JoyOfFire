using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableButtonGroup : MonoBehaviour
{
    public List<Button> buttons; // 所有按钮
    public RectTransform expandedButton; // 当前展开的按钮
    public float expandedWidth = 300f; // 展开状态的宽度
    public float collapsedWidth = 100f; // 折叠状态的宽度
    public float animationDuration = 0.2f; // 动画时长

    void Start()
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    void OnButtonClicked(Button clickedButton)
    {
        foreach (var button in buttons)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            var textComponent = button.GetComponentInChildren<Text>();

            if (button == clickedButton)
            {
                // 展开点击的按钮
                StartCoroutine(AnimateButtonSize(rectTransform, expandedWidth, true, textComponent));
                expandedButton = rectTransform;
            }
            else
            {
                // 收缩其他按钮
                StartCoroutine(AnimateButtonSize(rectTransform, collapsedWidth, false, textComponent));
            }
        }
    }

    System.Collections.IEnumerator AnimateButtonSize(RectTransform target, float targetWidth, bool showText, Text textComponent)
    {
        float elapsedTime = 0f;
        float startWidth = target.sizeDelta.x;

        // 隐藏文字（如果是收缩）
        if (!showText && textComponent != null)
        {
            textComponent.enabled = false;
        }

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, targetWidth, elapsedTime / animationDuration);
            target.sizeDelta = new Vector2(newWidth, target.sizeDelta.y);
            yield return null;
        }

        target.sizeDelta = new Vector2(targetWidth, target.sizeDelta.y);

        // 显示文字（如果是展开）
        if (showText && textComponent != null)
        {
            textComponent.enabled = true;
        }
    }
}
