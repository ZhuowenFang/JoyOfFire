using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class WindChimeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tween swingTween;
    private Quaternion originalRotation;

    [Header("动效参数")]
    public float swingAngle = 10f;
    public float swingDuration = 1f;

    void Awake()
    {
        originalRotation = transform.localRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 每次鼠标移入都杀掉旧的 tween，并重新创建
        swingTween?.Kill();

        swingTween = DOTween.Sequence()
            .Append(transform.DOLocalRotate(new Vector3(0, swingAngle, 0), swingDuration / 2).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(new Vector3(0, -swingAngle, 0), swingDuration).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(Vector3.zero, swingDuration / 2).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true); // 防止 TimeScale=0 时也能播放
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        swingTween?.Kill();
        swingTween = null;
        transform.localRotation = originalRotation;
    }
}