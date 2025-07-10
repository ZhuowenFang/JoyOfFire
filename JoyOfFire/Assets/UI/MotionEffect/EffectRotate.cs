using UnityEngine;
using DG.Tweening;

public class EffectRotate : MonoBehaviour
{
    public Transform panelTransform;

    [Header("��ת����")]
    public Vector3 rotateBy = new Vector3(90, 90, 90); // ÿ�������ת�ĽǶ�
    public float rotateDuration = 5f;
    public int rotateLoops = 100;
    public LoopType loopType = LoopType.Incremental;
    public RotateMode rotateMode = RotateMode.FastBeyond360;
    public Ease rotateEase = Ease.OutQuad;

    private Tween rotateTween;

    public void Play()
    {
        // ��ֹ�ظ�����
        panelTransform.DOKill();
        rotateTween?.Kill();

        // ������ת
        panelTransform.localRotation = Quaternion.identity;

        // ������ת
        rotateTween = panelTransform.DOLocalRotate(rotateBy, rotateDuration, rotateMode)
            .SetEase(rotateEase)
            .SetRelative(true)
            .SetLoops(rotateLoops, loopType)
            .SetUpdate(true); // ��ֹ�� TimeScale Ӱ��
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
