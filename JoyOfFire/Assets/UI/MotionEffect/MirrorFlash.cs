using System.Collections;
using UnityEngine;

public class MirrorFlash : MonoBehaviour
{
    [Header("���ʺͲ���")]
    public Material flashMat;               // ��Ҫ���ƵĲ��ʣ�������ʵ�����ʣ�
    public float interval = 5f;             // �Զ�ɨ��ʱ�������룩
    public float sweepDuration = 0.6f;      // ɨ�Ӷ���ʱ�����룩

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

    // �Զ����ֶ�������ڣ������������������ɨ��Э��
    private void StartSweep()
    {
        if (sweepCoroutine != null)
        {
            StopCoroutine(sweepCoroutine);
            sweepCoroutine = null;
        }
        sweepCoroutine = StartCoroutine(SweepFlashCoroutine());
    }

    // ��������ɨ�Ӷ��������ü�ʱ��
    public void PlayImmediately()
    {
        timer = 0f;
        StartSweep();
    }

    // ֹͣ��ǰɨ�Ӷ����͹ر�Ч��
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
