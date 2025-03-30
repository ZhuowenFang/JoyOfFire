using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagMove : MonoBehaviour
{
    public GameObject backpackPanel;
    public Image arrowIcon;
    public float moveSpeed = 10f;
    public Vector2 hiddenPosition = new Vector2(0, -400);
    public Vector2 visiblePosition = new Vector2(0, 0);

    private bool isOpen = false;

    void Start()
    {
        backpackPanel.GetComponent<RectTransform>().anchoredPosition = hiddenPosition;
    }

    // 点击背包按钮时触发
    public void ToggleBackpack()
    {
        if (isOpen)
        {
            StartCoroutine(MoveBackpack(hiddenPosition));
            StartCoroutine(RotateArrow(0f));
        }
        else
        {
            StartCoroutine(MoveBackpack(visiblePosition));
            StartCoroutine(RotateArrow(180f));
        }

        isOpen = !isOpen;
    }

    // 控制背包栏的移动
    private System.Collections.IEnumerator MoveBackpack(Vector2 targetPosition)
    {
        RectTransform rectTransform = backpackPanel.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;

        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime * moveSpeed;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    // 控制箭头图标的旋转
    private System.Collections.IEnumerator RotateArrow(float targetRotation)
    {
        float startRotation = arrowIcon.transform.eulerAngles.z;
        float timeElapsed = 0f;

        while (timeElapsed < 1f)
        {
            float rotation = Mathf.LerpAngle(startRotation, targetRotation, timeElapsed);
            arrowIcon.transform.rotation = Quaternion.Euler(0, 0, rotation);
            timeElapsed += Time.deltaTime * moveSpeed;
            yield return null;
        }

        arrowIcon.transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }
}
