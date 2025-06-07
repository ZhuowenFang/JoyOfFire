using UnityEngine;

[ExecuteInEditMode]
public class VignetteEffect : MonoBehaviour
{
    public Material vignetteMaterial;
    [Range(0, 1)] public float intensity = 0.5f;
    [Range(0, 1)] public float smoothness = 0.5f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_Intensity", intensity);
            vignetteMaterial.SetFloat("_Smoothness", smoothness);
            Graphics.Blit(src, dest, vignetteMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}