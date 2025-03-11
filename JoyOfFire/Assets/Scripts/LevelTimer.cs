using System.Collections;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    // 当前会话的关卡时长（分钟）
    private float sessionTime = 0f;
    private bool timerRunning = false;

    void Start()
    {
        // 开始计时（你也可以在进入关卡时调用 StartTimer）
        StartTimer();

    }

    public void StartTimer()
    {
        if (!timerRunning)
        {
            timerRunning = true;
            StartCoroutine(TimerCoroutine());
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    private IEnumerator TimerCoroutine()
    {
        while (timerRunning)
        {
            // 使用 WaitForSecondsRealtime 来确保计时不受 Time.timeScale 影响
            yield return new WaitForSecondsRealtime(60f);
            sessionTime += 1f;
            Debug.Log("关卡时长更新：" + sessionTime + " 分钟");
            // 更新当前会话的关卡时长
            CloudSaveManager.Instance.UpdateLevelTime(sessionTime);
        }
    }
}