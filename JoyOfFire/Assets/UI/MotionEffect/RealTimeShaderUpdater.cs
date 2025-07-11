using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RealTimeShaderUpdater : MonoBehaviour
{
    private Material instanceMaterial;

    void Start()
    {
        Image image = GetComponent<Image>();
        if (image.material != null)
        {
            // ��¡һ�ݲ��ʣ���ֹ�޸Ĺ������
            instanceMaterial = new Material(image.material);
            image.material = instanceMaterial;
        }
        else
        {
            Debug.LogWarning("RealTimeShaderUpdaterSingle: Image has no material assigned.");
        }
    }

    void Update()
    {
        if (instanceMaterial != null)
        {
            float realtime = Time.realtimeSinceStartup;
            instanceMaterial.SetFloat("_RealTime", realtime);
        }
    }
}

