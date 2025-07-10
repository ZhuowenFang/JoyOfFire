using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UICardFlip : MonoBehaviour, IPointerClickHandler
{
    public RectTransform front;
    public RectTransform back;
    public float flipDuration = 0.5f;

    private bool isFront = true;
    private bool isFlipping = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFlipping) return;

        isFlipping = true;

        // ����ǰ������ת+180��
        transform.DOLocalRotate(new Vector3(0, 180, 0), flipDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                // ��鵱ǰ��ת�Ƕȣ�����պù���90�㣬�л�����
                float yAngle = transform.localEulerAngles.y % 360;
                if ((isFront && yAngle > 90 && yAngle < 270) || (!isFront && (yAngle < 90 || yAngle > 270)))
                {
                    front.gameObject.SetActive(!isFront);
                    back.gameObject.SetActive(isFront);
                }
            })
            .OnComplete(() =>
            {
                isFront = !isFront;
                isFlipping = false;
            });
    }
}
