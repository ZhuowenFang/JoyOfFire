using UnityEngine;
using DG.Tweening;

public class EffectRotate : MonoBehaviour
{
    public Transform panelTransform;

    [Header("旋转参数")]
    public Vector3 rotateBy = new Vector3(90, 90, 90); // 每次相对旋转的角度
    public float rotateDuration = 5f;
    public int rotateLoops = 100;
    public LoopType loopType = LoopType.Incremental;
    public RotateMode rotateMode = RotateMode.FastBeyond360;
    public Ease rotateEase = Ease.OutQuad;

    private Tween rotateTween;

    public void Play()
    {
        // 防止重复播放
        panelTransform.DOKill();
        rotateTween?.Kill();

        // 重置旋转
        panelTransform.localRotation = Quaternion.identity;

        // 播放旋转
        rotateTween = panelTransform.DOLocalRotate(rotateBy, rotateDuration, rotateMode)
            .SetEase(rotateEase)
            .SetRelative(true)
            .SetLoops(rotateLoops, loopType)
            .SetUpdate(true); // 防止被 TimeScale 影响
    }

    private void OnEnable()
    {
        Play();
    }

    private void OnDisable()
    {
        rotateTween?.Kill();
    }
}
