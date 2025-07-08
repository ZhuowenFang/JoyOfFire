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
        // Kill 防止重复播放卡顿
        canvasGroup.DOKill();
        panelTransform.DOKill();

        // 重置状态
        canvasGroup.alpha = 0;
        panelTransform.localScale = startScale;

        // 播放动效 + SetUpdate(true) 防止被 TimeScale 影响
        canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
        panelTransform.DOScale(Vector3.one, scaleDuration).SetEase(scaleEase).SetUpdate(true);
    }

    private void OnEnable()
    {
        Play();
    }
}
