using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ColorBreath : MonoBehaviour
{
    private Graphic graphic;        // Image / Text / TMPText 等都继承自 Graphic
    private Tween colorTween;

    [Header("动效参数")]
    public Color fromColor = new Color(0.8f, 0.4f, 0.4f, 1f); // 你FROM里的颜色
    public float duration = 1f;
    public Ease easeType = Ease.OutQuad;
    public int loops = 100;
    public LoopType loopType = LoopType.Yoyo;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        if (graphic == null)
        {
            Debug.LogError("需要挂在带 Graphic（Image/Text/TMPText）的对象上！");
        }
    }

    private void OnEnable()
    {
        PlayBreathEffect();
    }

    private void OnDisable()
    {
        colorTween?.Kill();  // 停止动画，防止残留
    }

    public void PlayBreathEffect()
    {
        if (graphic == null) return;

        // 重置颜色
        graphic.color = fromColor;

        // 停止旧动效
        colorTween?.Kill();

        // 播放新动效
        colorTween = graphic.DOColor(Color.white, duration)
            .SetEase(easeType)
            .SetLoops(loops, loopType)
            .SetUpdate(true); // 如果需要忽略 TimeScale，可改成 .SetUpdate(true)
    }
}
