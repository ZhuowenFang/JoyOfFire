using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GetLevelData(string level, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"https://joy-fire-dev.czczcz.xyz/api/v1/level/{level}";

        StartCoroutine(GetRequest(url, onSuccess, onError));
    }

    private IEnumerator GetRequest(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = Encoding.UTF8.GetString(request.downloadHandler.data);
                onSuccess?.Invoke(jsonResponse);            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
   

    [System.Serializable]
    public class LevelInfo
    {
        public string level_desc;               // 关卡描述
        public string level_name;               // 关卡名称
        public string option_a_text;            // 选项 A 的文本
        public string option_b_text;            // 选项 B 的文本
        public string option_c_text;            // 选项 C 的文本
        public string option_a_experience;      // 选项 A 的经验描述
        public string option_b_experience;      // 选项 B 的经验描述
        public string option_c_experience;      // 选项 C 的经验描述
        public string option_a_res;             // 选项 A 的结果（JSON 格式）
        public string option_b_res;             // 选项 B 的结果（JSON 格式）
        public string option_c_res;             // 选项 C 的结果（JSON 格式）
        public string option_a_type;            // 选项 A 的类型
        public string option_b_type;            // 选项 B 的类型
        public string option_c_type;            // 选项 C 的类型
        public string level_pre_cond;           // 前置条件
        public string stick_point;              // 是否为关键点
        public string f19;                      // 备用字段 F19
        public string f20;                      // 备用字段 F20
    }

    [System.Serializable]
    public class LevelData
    {
        public string id;                       // 关卡 ID
        public int levelOrder;                  // 关卡顺序
        public int levelNum;                    // 关卡编号
        public LevelInfo levelInfo;             // 关卡信息
    }

    [System.Serializable]
    public class LevelResponse
    {
        public string code;                     // 响应码
        public string message;                  // 响应消息
        public LevelData data;                  // 关卡数据
    }

    [System.Serializable]
    public class OptionResult
    {
        public Dictionary<string, string> tool_required { get; set; }      // 需要的工具
        public Dictionary<string, bool> success_condition { get; set; } // 成功条件
        public string mini_game { get; set; }                             // 迷你游戏
        public Dictionary<string, int> monsters { get; set; }             // 怪物信息
        public Dictionary<string, int> rewards { get; set; }              // 奖励信息
        public Dictionary<string, string> success_buff { get; set; }      // 成功后的 Buff
        public string success_copywriting { get; set; }                   // 成功后的文案
        public Dictionary<string, string> fail_buff { get; set; }         // 失败后的 Buff
        public string fail_copywriting { get; set; }                      // 失败后的文案
        public string next_stage { get; set; }                            // 下一阶段
        public string unlock_stage { get; set; }                          // 解锁的阶段
    }
}