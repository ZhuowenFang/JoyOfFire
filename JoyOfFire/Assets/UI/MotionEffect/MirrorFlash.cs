using System.Collections;
using UnityEngine;

public class MirrorFlash : MonoBehaviour
{
    [Header("材质和参数")]
    public Material flashMat;               // 建议用实例材质
    public float interval = 5f;             // 自动扫掠时间间隔
    public float sweepDuration = 0.6f;      // 扫掠动画时长

    private float timer = 0f;
    private bool sweeping = false;
    private Coroutine sweepCoroutine = null;

    void Update()
    {
        timer += Time.unscaledDeltaTime; 

        if (!sweeping && timer >= interval)
        {
            StartSweep();
            timer = 0f;
        }
    }

    private void StartSweep()
    {
        if (sweepCoroutine != null)
        {
            StopCoroutine(sweepCoroutine);
            sweepCoroutine = null;
        }
        sweepCoroutine = StartCoroutine(SweepFlashCoroutine());
    }

    public void PlayImmediately()
    {
        timer = 0f;
        StartSweep();
    }


    public void StopEffect()
    {
        if (sweepCoroutine != null)
        {
            StopCoroutine(sweepCoroutine);
            sweepCoroutine = null;
        }
        sweeping = false;
        flashMat.SetFloat("_FlashStrength", 0f);
    }

    private IEnumerator SweepFlashCoroutine()
    {
        sweeping = true;
        float elapsed = 0f;
        flashMat.SetFloat("_FlashStrength", 1f);

        while (elapsed < sweepDuration)
        {
            float pos = Mathf.Clamp01(elapsed / sweepDuration);
            flashMat.SetFloat("_FlashPos", pos);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        flashMat.SetFloat("_FlashStrength", 0f);
        sweeping = false;
        sweepCoroutine = null;
    }
}
