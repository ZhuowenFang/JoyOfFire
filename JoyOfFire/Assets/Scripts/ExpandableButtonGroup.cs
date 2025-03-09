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
            var skillDescription = button.transform.Find("SkillDescription").GetComponent<Text>();

            if (button == clickedButton)
            {
                StartCoroutine(AnimateButtonSize(rectTransform, expandedWidth, true, textComponent,skillDescription));
                expandedButton = rectTransform;
            }
            else
            {
                StartCoroutine(AnimateButtonSize(rectTransform, collapsedWidth, false, textComponent,skillDescription));
            }
        }
    }

    System.Collections.IEnumerator AnimateButtonSize(RectTransform target, float targetWidth, bool showText, Text textComponent, Text skillDescription)
    {
        float elapsedTime = 0f;
        float startWidth = target.sizeDelta.x;

        if (!showText && textComponent != null)
        {
            textComponent.enabled = false;
            skillDescription.enabled = false;
        }

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; 
            float newWidth = Mathf.Lerp(startWidth, targetWidth, elapsedTime / animationDuration);
            target.sizeDelta = new Vector2(newWidth, target.sizeDelta.y);
            yield return null;
        }

        target.sizeDelta = new Vector2(targetWidth, target.sizeDelta.y);

        if (showText && textComponent != null)
        {
            textComponent.enabled = true;
            skillDescription.enabled = true;
        }
    }

}
