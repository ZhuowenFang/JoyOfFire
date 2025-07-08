using DG.Tweening;
using UnityEngine;

public class PanelRotateEffect : MonoBehaviour
{

    public Transform panelTransform; 
    private Tween rotateTween;

    public void Play()
    {
        // 先杀掉之前的动画，防止叠加
        panelTransform.DOKill();

        // 重置旋转（根据你的需求）
        panelTransform.localRotation = Quaternion.identity;

        // 重新创建旋转动画
        rotateTween = transform.DOLocalRotate(
            new Vector3(90, 90, 90),   // 目标旋转角度
            5f,                       // 持续时间（截图里Duration）
            RotateMode.FastBeyond360   // 旋转模式，和截图一致
        )
        .SetRelative(true)           // 相对旋转
        .SetEase(Ease.OutQuad)       // 缓动方式
        .SetLoops(100, LoopType.Incremental);
    }

    private void OnEnable()
    {
        Play();
    }
}