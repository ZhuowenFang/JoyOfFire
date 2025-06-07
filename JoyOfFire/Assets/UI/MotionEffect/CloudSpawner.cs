using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    [Header("云朵设置")]
    public List<Sprite> cloudSprites;          // 云朵贴图列表
    public GameObject cloudPrefab;             // 云朵预制体
    public Transform player;                   // 角色 Transform

    [Header("区域与数量")]
    public Vector2 areaSize = new Vector2(20f, 20f);  // 生成区域大小
    public int cloudCount = 100;                         // 云朵总数

    [Header("云朵外观")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.5f);  // 云朵随机缩放范围

    [Header("云朵间距")]
    public float minDistance = 1.5f;  // 云朵之间的最小间距

    void Start()
    {
        SpawnCloudsRandom();
    }

    void SpawnCloudsRandom()
    {
        if (cloudPrefab == null || cloudSprites.Count == 0 || player == null)
        {
            Debug.LogWarning("云朵生成配置不完整！");
            return;
        }

        List<Vector3> positions = new List<Vector3>();

        int attempts = 0;     // 尝试次数，避免死循环
        int maxAttempts = cloudCount * 10;

        int spawnedCount = 0;

        while (spawnedCount < cloudCount && attempts < maxAttempts)
        {
            attempts++;

            // 在区域内随机位置
            float posX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float posY = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
            Vector3 spawnPos = new Vector3(posX, posY, 0);

            // 检查新位置与已有云朵间距
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
                continue;  // 距离太近，重新随机位置

            positions.Add(spawnPos);

            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity, transform);

            SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
            sr.sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];

            // 随机缩放和左右随机翻转
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
            Debug.LogWarning($"尝试达到上限，实际生成云朵数量：{spawnedCount}");
        }
    }
}