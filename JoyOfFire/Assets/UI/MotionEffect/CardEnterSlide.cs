using UnityEngine;
using DG.Tweening;

public class CardEnterSlide : MonoBehaviour
{
    public Vector3 targetLocalPosition;
    public Vector3 enterOffset = new Vector3(0, 300f, 0);
    public float enterDuration = 0.5f;
    public Ease enterEase = Ease.OutCubic;

    private Tween moveTween;

    public void ResetAndPlay()
    {
        Debug.Log("ResetAndPlay called on " + gameObject.name);

        transform.localPosition = targetLocalPosition + enterOffset;

        moveTween?.Kill();
        moveTween = transform.DOLocalMove(targetLocalPosition, enterDuration)
            .SetEase(enterEase)
            .SetUpdate(true);
    }

    private void OnDisable()
    {
        moveTween?.Kill();
    }
}
