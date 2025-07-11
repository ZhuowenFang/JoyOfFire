using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("旋转参数")]
    public float rotationAngle = 180f;
    public float rotateSpeed = 200f;            // 每秒旋转多少度
    public float rotationThreshold = 0.1f;      // 接近目标时自动对齐

    [Header("卡牌正反面")]
    public GameObject frontRoot;
    public GameObject backRoot;

    private Quaternion originalRotation;
    private Quaternion targetRotation;

    private bool isHover = false;
    private bool isFront = true;
    private float lastFlipTime = -10f;

    void Start()
    {
        InitRotation();   
    }

    public void InitRotation()
    {
        originalRotation = transform.localRotation;
        targetRotation = originalRotation * Quaternion.Euler(0, rotationAngle, 0);
        Debug.Log("InitRotation called on " + gameObject.name);
    }

    public void ResetFace()
    {
        isFront = true;
        frontRoot?.SetActive(true);
        backRoot?.SetActive(false);
    }

    void Update()
    {
        Quaternion target = isHover ? targetRotation : originalRotation;

        float step = rotateSpeed * Time.unscaledDeltaTime;

        if (Quaternion.Angle(transform.localRotation, target) > rotationThreshold)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, step);
        }
        else
        {
            transform.localRotation = target;
        }

        // 当旋转到接近 ±90° 时，触发翻面
        float y = transform.localEulerAngles.y;
        float normalizedY = (y > 180f) ? y - 360f : y;
        bool near90 = Mathf.Abs(normalizedY - 90f) < 0.5f || Mathf.Abs(normalizedY + 90f) < 0.5f;

        if (near90 && Time.realtimeSinceStartup - lastFlipTime > 0.3f) // 用真实时间防止重复翻面
        {
            ToggleFace();
            lastFlipTime = Time.realtimeSinceStartup;
        }
    }

    private void ToggleFace()
    {
        isFront = !isFront;
        frontRoot?.SetActive(isFront);
        backRoot?.SetActive(!isFront);
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