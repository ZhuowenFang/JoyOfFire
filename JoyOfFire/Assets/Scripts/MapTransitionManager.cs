using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

public class MapTransitionManager : MonoBehaviour
{
    [Header("地图设置")]
    // 旧地图和新地图的 GameObject（切换时启用/禁用）
    public List<GameObject> maps;
    // 每个地图对应的边界碰撞体（顺序与 maps 一一对应）
    public List<Collider2D> mapBounds;

    [Header("摄像机设置")]
    public CinemachineConfiner cinemachineConfiner;

    public static MapTransitionManager instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    /// <summary>
    /// 触发地图切换，完成禁用旧地图、启用新地图、修改玩家位置以及更新虚拟摄像机边界的操作
    /// </summary>
    public void TransitionMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= maps.Count)
        {
            Debug.LogError("无效的地图索引");
            return;
        }

        // 禁用所有地图
        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
        // 启用目标地图
        maps[mapIndex].SetActive(true);

        // 更新虚拟摄像机的边界
        if (cinemachineConfiner != null && mapBounds != null && mapBounds.Count > mapIndex)
        {
            Collider2D newBounds = mapBounds[mapIndex];
            if (newBounds != null)
            {
                cinemachineConfiner.m_BoundingShape2D = newBounds;
                // 强制重新计算路径缓存
                cinemachineConfiner.InvalidatePathCache();
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
    }
}