using UnityEngine;

public class VortexController : MonoBehaviour
{
    public Material swirlMaterial;
    public float twistSpeed = 1.5f;        // ˳ʱ���ٶ�
    public float strength = 1f;            // Ť��ǿ��
    public float fadeRadius = 0.4f;        // ͸��

    void Update()
    {
        swirlMaterial.SetFloat("_TwistSpeed", twistSpeed);
        swirlMaterial.SetFloat("_Strength", strength);
        swirlMaterial.SetFloat("_FadeRadius", fadeRadius);
    }
}