using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UICardFlip : MonoBehaviour, IPointerClickHandler
{
    public RectTransform front;
    public RectTransform back;
    public float flipDuration = 0.5f;

    private bool isFront = true;
    private bool isFlipping = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFlipping) return;

        isFlipping = true;

        // 朝当前方向旋转+180°
        transform.DOLocalRotate(new Vector3(0, 180, 0), flipDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                // 检查当前旋转角度，如果刚好过了90°，切换显隐
                float yAngle = transform.localEulerAngles.y % 360;
                if ((isFront && yAngle > 90 && yAngle < 270) || (!isFront && (yAngle < 90 || yAngle > 270)))
                {
                    front.gameObject.SetActive(!isFront);
                    back.gameObject.SetActive(isFront);
                }
            })
            .OnComplete(() =>
            {
                isFront = !isFront;
                isFlipping = false;
            });
    }
}
