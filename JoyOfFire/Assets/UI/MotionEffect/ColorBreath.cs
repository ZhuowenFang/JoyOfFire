using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ColorBreath : MonoBehaviour
{
    private Graphic graphic;        // Image / Text / TMPText �ȶ��̳��� Graphic
    private Tween colorTween;

    [Header("��Ч����")]
    public Color fromColor = new Color(0.8f, 0.4f, 0.4f, 1f); // ��FROM�����ɫ
    public float duration = 1f;
    public Ease easeType = Ease.OutQuad;
    public int loops = 100;
    public LoopType loopType = LoopType.Yoyo;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        if (graphic == null)
        {
            Debug.LogError("��Ҫ���ڴ� Graphic��Image/Text/TMPText���Ķ����ϣ�");
        }
    }

    private void OnEnable()
    {
        PlayBreathEffect();
    }

    private void OnDisable()
    {
        colorTween?.Kill();  // ֹͣ��������ֹ����
    }

    public void PlayBreathEffect()
    {
        if (graphic == null) return;

        // ������ɫ
        graphic.color = fromColor;

        // ֹͣ�ɶ�Ч
        colorTween?.Kill();

        // �����¶�Ч
        colorTween = graphic.DOColor(Color.white, duration)
            .SetEase(easeType)
            .SetLoops(loops, loopType)
            .SetUpdate(true); // �����Ҫ���� TimeScale���ɸĳ� .SetUpdate(true)
    }
}
