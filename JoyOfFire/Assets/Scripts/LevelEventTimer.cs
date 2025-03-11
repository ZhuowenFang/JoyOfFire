using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class LevelEventTimer : MonoBehaviour
{
    // 当前关卡编号，如 "3-7"
    public string level;
    // 计时器（以秒为单位）
    private float timer;
    private bool timerRunning = false;
    public static LevelEventTimer instance;
    private string type = "_Event_exposure";
    
    private void Awake()
    {
        instance = this;
    }

    // 开始关卡事件时调用（例如在 API 响应成功后）
    public void OnLevelDataSuccess(string Level, bool isBattle)
    {
        level = Level;
        if (isBattle)
        {
            type = "_Battle_exposure";
        }
        StartTimer();
    }
    

    // 开始计时
    public void StartTimer()
    {
        timer = 0f;
        timerRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    // 停止计时并返回累计时间
    public IEnumerator TimerCoroutine()
    {
        while (timerRunning)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // 当事件结束时调用此方法，将计时器时间更新到 Cloud Save，并清零计时器
    public async void OnEventEnd()
    {
        // 停止计时器
        timerRunning = false;
        float exposureTime = timer; // 单位秒；若需要转换为分钟，则 exposureTime /= 60f;

        // 构造动态字段名称，如 "3-7_Event_exposure"
        string fieldKey = $"{level}{type}";

        // 更新 Cloud Save 中对应字段的当前记录（这里使用通用的 UpdateLevelField 方法）
        await CloudSaveManager.Instance.UpdateLevelField<float>(fieldKey, exposureTime);

        Debug.Log($"关卡 {level} 事件曝光时间更新为: {exposureTime} 秒");
        // 重置计时器
        timer = 0f;
    }
}