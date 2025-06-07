using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class WindChimeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tween swingTween;
    private Quaternion originalRotation;

    public float swingAngle = 10f;
    public float swingDuration = 1f;

    void Awake()
    {
        originalRotation = transform.localRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        swingTween?.Kill();
        swingTween = DOTween.Sequence()
            .Append(transform.DOLocalRotate(new Vector3(0, swingAngle, 0), swingDuration / 2).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(new Vector3(0, -swingAngle, 0), swingDuration).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(new Vector3(0, 0, 0), swingDuration / 2).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        swingTween?.Kill();
        transform.localRotation = originalRotation;
    }
}