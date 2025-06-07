using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    [Header("�ƶ�����")]
    public List<Sprite> cloudSprites;          // �ƶ���ͼ�б�
    public GameObject cloudPrefab;             // �ƶ�Ԥ����
    public Transform player;                   // ��ɫ Transform

    [Header("����������")]
    public Vector2 areaSize = new Vector2(20f, 20f);  // ���������С
    public int cloudCount = 100;                         // �ƶ�����

    [Header("�ƶ����")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.5f);  // �ƶ�������ŷ�Χ

    [Header("�ƶ���")]
    public float minDistance = 1.5f;  // �ƶ�֮�����С���

    void Start()
    {
        SpawnCloudsRandom();
    }

    void SpawnCloudsRandom()
    {
        if (cloudPrefab == null || cloudSprites.Count == 0 || player == null)
        {
            Debug.LogWarning("�ƶ��������ò�������");
            return;
        }

        List<Vector3> positions = new List<Vector3>();

        int attempts = 0;     // ���Դ�����������ѭ��
        int maxAttempts = cloudCount * 10;

        int spawnedCount = 0;

        while (spawnedCount < cloudCount && attempts < maxAttempts)
        {
            attempts++;

            // �����������λ��
            float posX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float posY = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
            Vector3 spawnPos = new Vector3(posX, posY, 0);

            // �����λ���������ƶ���
            bool tooClose = false;
            foreach (var pos in positions)
            {
                if (Vector3.Distance(pos, spawnPos) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;  // ����̫�����������λ��

            positions.Add(spawnPos);

            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity, transform);

            SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
            sr.sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];

            // ������ź����������ת
            float scale = Random.Range(scaleRange.x, scaleRange.y);
            int flipScaleX = Random.value < 0.5f ? 1 : -1;
            cloud.transform.localScale = new Vector3(scale * flipScaleX, scale, 1);

            CloudBehavior behavior = cloud.GetComponent<CloudBehavior>();
            if (behavior != null)
                behavior.player = player;

            spawnedCount++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"���Դﵽ���ޣ�ʵ�������ƶ�������{spawnedCount}");
        }
    }
}