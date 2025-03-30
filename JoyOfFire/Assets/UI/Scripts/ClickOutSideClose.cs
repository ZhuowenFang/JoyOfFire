using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutSideClose : MonoBehaviour, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _mouseIsInside = false;
    private void OnEnable()
    {
        //Debug.LogError($"�¼������ע��{transform}");
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.LogError("ȡ������");
        if (!_mouseIsInside)
            gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.LogError("����");
        _mouseIsInside = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogError("�Ƴ�");
        _mouseIsInside = false;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

