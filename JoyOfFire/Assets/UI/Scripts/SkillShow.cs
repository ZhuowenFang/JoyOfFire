using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip;
    public Text tooltipText;
    public string skillDescription = "技能效果：/n 能量消耗";

    private RectTransform tooltipRectTransform;
    private bool isTooltipVisible = false;

    void Start()
    {
        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltip.SetActive(false);
    }

    // 当鼠标进入按钮时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTooltipVisible) return;
        tooltipText.text = skillDescription;
        Vector3 mousePos = Input.mousePosition;
        tooltipRectTransform.position = new Vector3(mousePos.x, mousePos.y - 50f, mousePos.z);  // 使得弹窗显示在按钮下方
        tooltip.SetActive(true);
    }

    // 当鼠标离开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isTooltipVisible) return;
        tooltip.SetActive(false);
        isTooltipVisible = false;
    }
}
