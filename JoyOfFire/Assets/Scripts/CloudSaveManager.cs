using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Services.Authentication; // 请确保已经安装了 Newtonsoft.Json 包

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance;

    // 本地缓存数据，每个用户只有一条记录
    private Dictionary<string, object> userData = new Dictionary<string, object>();
    private string userDataKey;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // 之后继续加载数据
            userDataKey = $"user_{AuthenticationService.Instance.PlayerId}_data";
            await LoadUserData();
            await AppendLevelTime();
            await AppendCreateCount();
            await UpdateLevelTime(0f);
            await InitialCreateCount();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 从 Cloud Save 中加载用户数据
    /// </summary>
    public async Task LoadUserData()
    {
        var keys = new HashSet<string> { userDataKey };
        try
        {
            var result = await CloudSaveService.Instance.Data.LoadAsync(keys);
            if (result.ContainsKey(userDataKey))
            {
                string jsonData = result[userDataKey];
                // 使用现有的包装类（或直接反序列化为 Dictionary<string, object>）
                SerializationWrapper wrapper = JsonUtility.FromJson<SerializationWrapper>(jsonData);
                userData = wrapper.ToDictionary();
                Debug.Log("User data loaded: " + jsonData);
            }
            else
            {
                // 没有数据时，初始化默认值
                userData = new Dictionary<string, object>()
                {
                    { "Mmcz_time", "[]" },    // 存储数组的 JSON 字符串
                    { "Mmcz_success", 0 },
                    { "Mmcz_create", "[]" }
                };
                Debug.Log("No existing data, initialized new user data.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading user data: {e.Message}");
        }
    }

    /// <summary>
    /// 保存当前 userData 到 Cloud Save 中
    /// </summary>
    public async Task SaveUserData()
    {
        try
        {
            // 使用包装类将字典转换成 JSON 字符串
            string jsonData = JsonUtility.ToJson(new SerializationWrapper(userData));
            var saveData = new Dictionary<string, object>()
            {
                { userDataKey, jsonData }
            };
            await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);
            Debug.Log("User data saved: " + jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving user data: {e.Message}");
        }
    }

    #region 更新方法

    // 更新在线状态（实时更新）
  
    // 更新关卡时长列表中的最后一个元素（duration 单位：分钟），列表以 JSON 数组字符串形式存储
    public async Task UpdateLevelTime(float newDuration)
    {
        userData["Mmcz_time"] = UpdateLastElementInJsonArray<float>(userData["Mmcz_time"] as string, newDuration);
        Debug.Log("UpdateLevelTime");
        await SaveUserData();
    }

    // 每次登录时追加一个初始关卡时长记录（例如初始值可以为 0）
    public async Task AppendLevelTime()
    {
        userData["Mmcz_time"] = AppendToJsonArray<float>(userData["Mmcz_time"] as string, 0f);
        Debug.Log("AppendLevelTime");
        await SaveUserData();
    }

    // 更新创建次数列表中的最后一个元素
    public async Task UpdateCreateCount()
    {
        string json = userData["Mmcz_create"] as string;
        List<int> list = JsonArrayToList<int>(json);
        list[list.Count - 1]++;
        // if (list.Count > 0)
        // {
        //     
        // }
        // else
        // {
        //     list.Add(0);
        // }
        userData["Mmcz_create"] = ListToJsonArray(list);
        await SaveUserData();
    }

    // 每次登录时追加一个初始创建次数记录（初始值 0）
    public async Task AppendCreateCount()
    {
        userData["Mmcz_create"] = AppendToJsonArray<int>(userData["Mmcz_create"] as string, 0);
        Debug.Log("AppendCreateCount");
        await SaveUserData();
    }
    
    public async Task InitialCreateCount()
    {
        string json = userData["Mmcz_create"] as string;
        List<int> list = JsonArrayToList<int>(json);
        list[list.Count - 1] = 0;
        // if (list.Count > 0)
        // {
        //     
        // }
        // else
        // {
        //     list.Add(0);
        // }
        userData["Mmcz_create"] = ListToJsonArray(list);
        Debug.Log("InitialCreateCount");
        await SaveUserData();
    }


    public void UpdateSuccess(int entryCountAtSuccess)
    {
        userData["Mmcz_success"] = entryCountAtSuccess;
        SaveUserData();
    }

    #endregion

    #region 辅助方法：处理 JSON 数组

    // 将 JSON 数组字符串转换为 List<T>
    private List<T> JsonArrayToList<T>(string jsonArray)
    {
        if (string.IsNullOrEmpty(jsonArray))
        {
            return new List<T>();
        }
        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonArray);
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonArrayToList error: {e.Message}");
            return new List<T>();
        }
    }

    // 将 List<T> 转换为 JSON 数组字符串
    private string ListToJsonArray<T>(List<T> list)
    {
        return JsonConvert.SerializeObject(list);
    }

    // 追加一个新元素到 JSON 数组字符串中
    private string AppendToJsonArray<T>(string jsonArray, T newElement)
    {
        List<T> list = JsonArrayToList<T>(jsonArray);
        list.Add(newElement);
        return ListToJsonArray(list);
    }

    // 更新 JSON 数组中最后一个元素为 newValue
    private string UpdateLastElementInJsonArray<T>(string jsonArray, T newValue)
    {
        List<T> list = JsonArrayToList<T>(jsonArray);
        if (list.Count > 0)
        {
            list[list.Count - 1] = newValue;
        }
        else
        {
            list.Add(newValue);
        }
        return ListToJsonArray(list);
    }

    #endregion
}

/// <summary>
/// 辅助包装类，用于将 Dictionary<string, object> 转换为 JSON 字符串。
/// 注意：JsonUtility 对 Dictionary 支持有限，通常需要简单的结构。若结构复杂，可考虑使用 Newtonsoft.Json。
/// </summary>
[Serializable]
public class SerializationWrapper
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public SerializationWrapper() { }

    public SerializationWrapper(Dictionary<string, object> dict)
    {
        foreach (var kvp in dict)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value.ToString());
        }
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        for (int i = 0; i < keys.Count; i++)
        {
            dict[keys[i]] = values[i];
        }
        return dict;
    }
}
