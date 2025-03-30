using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutSideClose : MonoBehaviour, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _mouseIsInside = false;
    private void OnEnable()
    {
        //Debug.LogError($"事件焦点关注于{transform}");
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.LogError("取消焦点");
        if (!_mouseIsInside)
            gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.LogError("进入");
        _mouseIsInside = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogError("移出");
        _mouseIsInside = false;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

