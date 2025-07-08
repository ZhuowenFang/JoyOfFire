using UnityEngine;
using DG.Tweening;

public class PanelEnterEffect : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform panelTransform;
    public float fadeDuration = 0.3f;
    public float scaleDuration = 0.3f;
    public Vector3 startScale = new Vector3(0.8f, 0.8f, 0.8f);
    public Ease scaleEase = Ease.OutBack;

    public void Play()
    {
        // Kill ��ֹ�ظ����ſ���
        canvasGroup.DOKill();
        panelTransform.DOKill();

        // ����״̬
        canvasGroup.alpha = 0;
        panelTransform.localScale = startScale;

        // ���Ŷ�Ч + SetUpdate(true) ��ֹ�� TimeScale Ӱ��
        canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
        panelTransform.DOScale(Vector3.one, scaleDuration).SetEase(scaleEase).SetUpdate(true);
    }

    private void OnEnable()
    {
        Play();
    }
}
