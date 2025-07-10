using UnityEngine;

public class EffectController : MonoBehaviour
{
    public CardHoverRotate[] cards; // 拖进需要旋转的卡牌
    public CardEnterSlide[] slides; // 拖进需要滑动的卡牌
    public FloatingEffect[] floatingEffects;
    public MirrorFlash[] mirrorFlashes;

    private void OnEnable()
    {
        Debug.Log("父面板 OnEnable: 初始化所有子卡片");


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



