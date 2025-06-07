using UnityEngine;
using UnityEngine.EventSystems;

public class CardRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float rotationAngle = 180f;     // ��ת���Ƕ�
    public float rotateSpeed = 200f;       // ��ת�ٶ�
    public float rotationThreshold = 0.1f; // ���������ĽǶ���ֵ

    public GameObject frontRoot;           // ����
    public GameObject backObject;          // ����

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private bool isHover = false;

    private float lastFlipTime = -10f;     // �ϴ��л������ʱ��
    private bool isFront = true;           // ��ǰ�Ƿ�Ϊ����

    void Start()
    {
        originalRotation = transform.localRotation;
        targetRotation = originalRotation * Quaternion.Euler(0, rotationAngle, 0);

        if (frontRoot == null || backObject == null)
        {
            Debug.LogError("�븳ֵ frontRoot �� backObject ����");
        }

        frontRoot.SetActive(true);
        backObject.SetActive(false);
    }

    void Update()
    {
        Quaternion target = isHover ? targetRotation : originalRotation;

        // �����ǰ�Ƕȷǳ��ӽ�Ŀ��Ƕȣ���ֱ������Ŀ��Ƕȣ���ֹ����
        if (Quaternion.Angle(transform.localRotation, target) > rotationThreshold)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                target,
                rotateSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.localRotation = target;
        }

        // ��ǰ Y ��Ƕȣ�-180 ~ 180��
        float currentY = transform.localEulerAngles.y;
        float normalizedY = (currentY > 180f) ? currentY - 360f : currentY;

        // ����Ƿ��� ��90�� ����
        bool nearPositive90 = Mathf.Abs(normalizedY - 90f) < 0.5f;
        bool nearNegative90 = Mathf.Abs(normalizedY + 90f) < 0.5f;

        if ((nearPositive90 || nearNegative90) && Time.time - lastFlipTime > 1f)
        {
            ToggleFace();
            lastFlipTime = Time.time;
        }
    }

    private void ToggleFace()
    {
        isFront = !isFront;
        frontRoot.SetActive(isFront);
        backObject.SetActive(!isFront);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }
}