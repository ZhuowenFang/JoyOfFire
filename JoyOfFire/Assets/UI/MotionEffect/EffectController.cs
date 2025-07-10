using UnityEngine;

public class EffectController : MonoBehaviour
{
    public CardHoverRotate[] cards; // �Ͻ���Ҫ��ת�Ŀ���
    public CardEnterSlide[] slides; // �Ͻ���Ҫ�����Ŀ���
    public FloatingEffect[] floatingEffects;
    public MirrorFlash[] mirrorFlashes;

    private void OnEnable()
    {
        Debug.Log("����� OnEnable: ��ʼ�������ӿ�Ƭ");


        foreach (var card in cards)
        {
            card.InitRotation();
            card.ResetFace();
        }



    }
    public void PlayAllEffects()
    {
        Debug.Log("PlayAllEffects triggered");

        foreach (var mirror in mirrorFlashes)
        {
            mirror.PlayImmediately();
        }

        foreach (var effect in floatingEffects)
        {
            effect.RestartFloating();
        }

        foreach (var slide in slides)
        {
            slide.ResetAndPlay();
        }

    }

    public void StopAllEffects()
    {
        Debug.Log("StopAllEffects triggered");
        foreach (var mirror in mirrorFlashes)
        {
            mirror.StopEffect();
        }

        foreach (var effect in floatingEffects)
        {
            effect.StopFloating();
        }
    }
}



