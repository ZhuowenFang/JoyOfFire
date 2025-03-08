using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MapTransitionManager : MonoBehaviour
{
    [Header("地图设置")]
    public List<GameObject> maps;
    public List<Collider2D> mapBounds;
    public List<Transform> playerSpawnPoints;

    [Header("摄像机设置")]
    public CinemachineConfiner cinemachineConfiner;
    public CinemachineVirtualCamera virtualCamera;

    public static MapTransitionManager instance;
    public Image transitionPanel;

    
    private void Awake()
    {
        instance = this;
    }
    public void TransitionMapWithFade(int mapIndex, int respawnPointIndex)
    {
        Debug.Log("开始地图切换");
        StartCoroutine(TransitionRoutine(mapIndex, respawnPointIndex));
    }
    private IEnumerator FadeInPanel(float duration)
    {
        if(transitionPanel == null)
        {
            Debug.LogWarning("transitionPanel 未指定");
            yield break;
        }
        transitionPanel.gameObject.SetActive(true);
        Color color = transitionPanel.color;
        float t = 0f;
        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / duration);
            transitionPanel.color = color;
            yield return null;
        }
        color.a = 1f;
        transitionPanel.color = color;
    }

    /// <summary>
    /// 将 transitionPanel 从不透明淡出至透明
    /// </summary>
    private IEnumerator FadeOutPanel(float duration)
    {
        if(transitionPanel == null)
        {
            Debug.LogWarning("transitionPanel 未指定");
            yield break;
        }
        Color color = transitionPanel.color;
        float t = 0f;
        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / duration);
            transitionPanel.color = color;
            yield return null;
        }
        color.a = 0f;
        transitionPanel.color = color;
        transitionPanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// 触发地图切换，完成禁用旧地图、启用新地图、修改玩家位置以及更新虚拟摄像机边界的操作
    /// </summary>
    private IEnumerator TransitionRoutine(int mapIndex, int respawnPointIndex)
    {
        Debug.Log("开始地图切换协程");

        yield return StartCoroutine(FadeInPanel(1f));

        if (mapIndex < 0 || mapIndex >= maps.Count)
        {
            Debug.LogError("无效的地图索引");
            yield break;
        }
        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
        maps[mapIndex].SetActive(true);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
            agent.Warp(playerSpawnPoints[respawnPointIndex].position);
        }
        else
        {
            Debug.LogWarning("找不到带有 'Player' 标签的玩家");
        }

        if (cinemachineConfiner != null && mapBounds != null && mapBounds.Count > mapIndex)
        {
            Collider2D newBounds = mapBounds[mapIndex];
            if (newBounds != null)
            {
                cinemachineConfiner.m_BoundingShape2D = newBounds;
                cinemachineConfiner.InvalidatePathCache();
                
                if (mapIndex > 3)
                {
                    virtualCamera.m_Lens.OrthographicSize = 10;
                    player.transform.localScale = new Vector3(2, 2, 2);
                }
                else
                {
                    virtualCamera.m_Lens.OrthographicSize = 6;
                    player.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                Debug.LogWarning("对应地图的边界碰撞体为空");
            }
        }
        else
        {
            Debug.LogWarning("cinemachineConfiner 或 mapBounds 未指定，或索引超出范围");
        }
        EventManager.instance.ReloadEvents();

        yield return new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(FadeOutPanel(1f));
        Debug.Log("地图切换协程结束");

    }
}