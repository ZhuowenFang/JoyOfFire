using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip;
    public Text tooltipText;
    public string skillDescription = "����Ч����/n ��������";

    private RectTransform tooltipRectTransform;
    private bool isTooltipVisible = false;

    void Start()
    {
        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltip.SetActive(false);
    }

    // �������밴ťʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTooltipVisible) return;
        tooltipText.text = skillDescription;
        Vector3 mousePos = Input.mousePosition;
        tooltipRectTransform.position = new Vector3(mousePos.x, mousePos.y - 50f, mousePos.z);  // ʹ�õ�����ʾ�ڰ�ť�·�
        tooltip.SetActive(true);
    }

    // ������뿪��ťʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isTooltipVisible) return;
        tooltip.SetActive(false);
        isTooltipVisible = false;
    }
}
