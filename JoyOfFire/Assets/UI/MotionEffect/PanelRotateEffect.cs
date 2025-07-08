using DG.Tweening;
using UnityEngine;

public class PanelRotateEffect : MonoBehaviour
{

    public Transform panelTransform; 
    private Tween rotateTween;

    public void Play()
    {
        // ��ɱ��֮ǰ�Ķ�������ֹ����
        panelTransform.DOKill();

        // ������ת�������������
        panelTransform.localRotation = Quaternion.identity;

        // ���´�����ת����
        rotateTween = transform.DOLocalRotate(
            new Vector3(90, 90, 90),   // Ŀ����ת�Ƕ�
            5f,                       // ����ʱ�䣨��ͼ��Duration��
            RotateMode.FastBeyond360   // ��תģʽ���ͽ�ͼһ��
        )
        .SetRelative(true)           // �����ת
        .SetEase(Ease.OutQuad)       // ������ʽ
        .SetLoops(100, LoopType.Incremental);
    }

    private void OnEnable()
    {
        Play();
    }
}