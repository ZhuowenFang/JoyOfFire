using System.Collections;
using UnityEngine;

public class MirrorFlash : MonoBehaviour
{
    [Header("材质和参数")]
    public Material flashMat;               // 需要控制的材质（建议是实例材质）
    public float interval = 5f;             // 自动扫掠时间间隔（秒）
    public float sweepDuration = 0.6f;      // 扫掠动画时长（秒）

    private float timer = 0f;
    private bool sweeping = false;
    private Coroutine sweepCoroutine = null;

    void Update()
    {
        timer += Time.deltaTime;

        if (!sweeping && timer >= interval)
        {
            StartSweep();
            timer = 0f;
        }
    }

    // 自动和手动播放入口，都调用这个方法启动扫掠协程
    private void StartSweep()
    {
        if (sweepCoroutine != null)
        {
            StopCoroutine(sweepCoroutine);
            sweepCoroutine = null;
        }
        sweepCoroutine = StartCoroutine(SweepFlashCoroutine());
    }

    // 立即播放扫掠动画，重置计时器
    public void PlayImmediately()
    {
        timer = 0f;
        StartSweep();
    }

    // 停止当前扫掠动画和关闭效果
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
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashMat.SetFloat("_FlashStrength", 0f);
        sweeping = false;
        sweepCoroutine = null;
    }
}
