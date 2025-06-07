using UnityEngine;

public class VortexController : MonoBehaviour
{
    public Material swirlMaterial;
    public float twistSpeed = 1.5f;        // 顺时针速度
    public float strength = 1f;            // 扭曲强度
    public float fadeRadius = 0.4f;        // 透明

    void Update()
    {
        swirlMaterial.SetFloat("_TwistSpeed", twistSpeed);
        swirlMaterial.SetFloat("_Strength", strength);
        swirlMaterial.SetFloat("_FadeRadius", fadeRadius);
    }
}