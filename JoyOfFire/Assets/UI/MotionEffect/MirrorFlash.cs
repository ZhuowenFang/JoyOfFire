using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorFlash : MonoBehaviour
{
    public Material flashMat;
    public float interval = 5f;
    public float sweepDuration = 0.6f;

    private float timer = 0f;
    private bool sweeping = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (!sweeping && timer >= interval)
        {
            StartCoroutine(SweepFlash());
            timer = 0f;
        }
    }

    System.Collections.IEnumerator SweepFlash()
    {
        sweeping = true;
        float time = 0f;
        flashMat.SetFloat("_FlashStrength", 1f);

        while (time < sweepDuration)
        {
            float pos = time / sweepDuration; // ´Ó0É¨µ½1
            flashMat.SetFloat("_FlashPos", pos);
            time += Time.deltaTime;
            yield return null;
        }

        flashMat.SetFloat("_FlashStrength", 0f);
        sweeping = false;
    }
}
