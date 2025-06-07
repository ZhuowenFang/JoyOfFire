using UnityEngine;
using UnityEngine.EventSystems;

public class CardRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float rotationAngle = 180f;     // 翻转最大角度
    public float rotateSpeed = 200f;       // 旋转速度
    public float rotationThreshold = 0.1f; // 抖动消除的角度阈值

    public GameObject frontRoot;           // 正面
    public GameObject backObject;          // 背面

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private bool isHover = false;

    private float lastFlipTime = -10f;     // 上次切换卡面的时间
    private bool isFront = true;           // 当前是否为正面

    void Start()
    {
        originalRotation = transform.localRotation;
        targetRotation = originalRotation * Quaternion.Euler(0, rotationAngle, 0);

        if (frontRoot == null || backObject == null)
        {
            Debug.LogError("请赋值 frontRoot 和 backObject 对象");
        }

        frontRoot.SetActive(true);
        backObject.SetActive(false);
    }

    void Update()
    {
        Quaternion target = isHover ? targetRotation : originalRotation;

        // 如果当前角度非常接近目标角度，则直接锁定目标角度，防止颤动
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

        // 当前 Y 轴角度（-180 ~ 180）
        float currentY = transform.localEulerAngles.y;
        float normalizedY = (currentY > 180f) ? currentY - 360f : currentY;

        // 检测是否在 ±90° 附近
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