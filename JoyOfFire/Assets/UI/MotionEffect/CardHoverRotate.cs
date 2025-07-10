using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float rotationAngle = 180f;
    public float rotateSpeed = 200f;
    public float rotationThreshold = 0.1f;

    public GameObject frontRoot;
    public GameObject backRoot;

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private bool isHover = false;
    private bool isFront = true;
    private float lastFlipTime = -10f;

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

    private void Update()
    {
        Quaternion target = isHover ? targetRotation : originalRotation;

        if (Quaternion.Angle(transform.localRotation, target) > rotationThreshold)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation, target, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.localRotation = target;
        }

        float y = transform.localEulerAngles.y;
        float normalizedY = (y > 180f) ? y - 360f : y;
        bool near90 = Mathf.Abs(normalizedY - 90f) < 0.5f || Mathf.Abs(normalizedY + 90f) < 0.5f;

        if (near90 && Time.time - lastFlipTime > 1f)
        {
            ToggleFace();
            lastFlipTime = Time.time;
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
