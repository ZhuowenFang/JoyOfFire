using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using Newtonsoft.Json; // 需要 Newtonsoft.Json 包
using Unity.Services.Authentication;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance;

    // 本地缓存数据，每个字段单独存储
    private Dictionary<string, object> userData = new Dictionary<string, object>();
    public Dictionary<string, string[]> levelFieldKeys = new Dictionary<string, string[]>();
    public void InitializeLevelFieldKeys()
    {
        // 第一层：1-1 到 1-21
        for (int i = 1; i <= 21; i++)
        {
            string level = $"1-{i}";
            string eventExposure = $"{level}_Event_exposure";
            string battleExposure = $"{level}_Battle_exposure";
            string costAverage = $"{level}_Cost_average";
            levelFieldKeys[level] = new string[] { eventExposure, battleExposure, costAverage };
        }
        // 第二层：2-1 到 2-14
        for (int i = 1; i <= 14; i++)
        {
            string level = $"2-{i}";
            string eventExposure = $"{level}_Event_exposure";
            string battleExposure = $"{level}_Battle_exposure";
            string costAverage = $"{level}_Cost_average";
            levelFieldKeys[level] = new string[] { eventExposure, battleExposure, costAverage };
        }
        // 第三层：3-1 到 3-21
        for (int i = 1; i <= 21; i++)
        {
            string level = $"3-{i}";
            string eventExposure = $"{level}_Event_exposure";
            string battleExposure = $"{level}_Battle_exposure";
            string costAverage = $"{level}_Cost_average";
            levelFieldKeys[level] = new string[] { eventExposure, battleExposure, costAverage };
        }
    }
    // 使用固定的键来存储各个字段
    private const string Key_Mmcz_time = "Mmcz_time";
    private const string Key_Mmcz_success = "Mmcz_success";
    private const string Key_Mmcz_create = "Mmcz_create";
    private const string Key_Mmcz_entries = "Mmcz_entries";

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLevelFieldKeys();

            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // 加载数据：这里我们直接从 Cloud Save 加载各个键的数据
            await LoadUserData();

            // 如果没有数据就初始化默认值
            if (!userData.ContainsKey(Key_Mmcz_time))
                userData[Key_Mmcz_time] = "[]";
            if (!userData.ContainsKey(Key_Mmcz_success))
                userData[Key_Mmcz_success] = 0;
            if (!userData.ContainsKey(Key_Mmcz_create))
                userData[Key_Mmcz_create] = "[]";
            if (!userData.ContainsKey(Key_Mmcz_entries))
            {
                userData[Key_Mmcz_entries] = 0;
                // await AppendAllLevelFieldKeys();
            }
            await AppendLevelTime(false);
            await AppendCreateCount(false);
            await UpdateLevelTime(0f, false);
            await InitialCreateCount(false);
            await IncrementEntries(false);
            await AppendAllLevelFieldKeys();
            await updateAllLevelFieldKeys();

            // 登录时追加新数据
            
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public async Task updateAllLevelFieldKeys()
    {
        foreach (var kvp in levelFieldKeys)
        {
            foreach (string fieldKey in kvp.Value)
            {
                await UpdateLevelField<float>(fieldKey, 0f, false);
            }
        }
        await SaveUserData();
    }

    /// <summary>
    /// 从 Cloud Save 中加载用户数据（单独加载每个键的数据）
    /// </summary>
    public async Task LoadUserData()
{
    // 构造需要加载的键集合，包括静态字段和动态关卡键
    HashSet<string> keysToLoad = new HashSet<string> { Key_Mmcz_success, Key_Mmcz_entries, Key_Mmcz_time, Key_Mmcz_create };
    
        foreach (var kvp in levelFieldKeys)
        {
            foreach (string dynamicKey in kvp.Value)
            {
                keysToLoad.Add(dynamicKey);
            }
        }
    
    
    Dictionary<string, string> aggregatedResults = new Dictionary<string, string>();
    List<string> keysList = new List<string>(keysToLoad);
    int batchSize = 20; // 根据 Cloud Save 限制，每批最多 50 个键

    for (int i = 0; i < keysList.Count; i += batchSize)
    {
        // 获取本批次的键列表
        int count = Math.Min(batchSize, keysList.Count - i);
        var batchKeys = new HashSet<string>(keysList.GetRange(i, count));

        try
        {
            var result = await CloudSaveService.Instance.Data.LoadAsync(batchKeys);
            foreach (var kvp in result)
            {
                aggregatedResults[kvp.Key] = kvp.Value;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading batch: {e.Message}");
        }
    }

    // 合并加载结果到 userData 字典中
    foreach (string key in keysToLoad)
    {
        if (aggregatedResults.ContainsKey(key))
        {
            userData[key] = aggregatedResults[key];
            Debug.Log($"Loaded {key}: {aggregatedResults[key]}");
        }
        else
        {
            // 如果该键没有数据，则根据键的类型初始化默认值
            if (IsDynamicKey(key))
            {
                userData[key] = "[]";
                Debug.Log($"Initialized dynamic key {key} to default: []");
            }
            else if (key == Key_Mmcz_success || key == Key_Mmcz_entries)
            {
                userData[key] = 0;
                Debug.Log($"Initialized {key} to default: 0");
            } else if (key == Key_Mmcz_create)
            {
                userData[key] = "[]";
                Debug.Log($"Initialized {key} to default: []");
            }
            else if (key == Key_Mmcz_time)
            {
                userData[key] = "[]";
                Debug.Log($"Initialized {key} to default: []");
            }
        }
    }
}

/// <summary>
/// 判断键是否为动态关卡键。根据你的规则，这里假设如果键包含 "_" 且首字符为数字，则为动态键
/// </summary>
private bool IsDynamicKey(string key)
{
    return !string.IsNullOrEmpty(key) && char.IsDigit(key[0]) && key.Contains("_");
}


    public async Task AppendAllLevelFieldKeys()
    {
        foreach (var kvp in levelFieldKeys)
        {
            foreach (string fieldKey in kvp.Value)
            {
                if (!userData.ContainsKey(fieldKey))
                {
                    userData[fieldKey] = "[]";
                }
                // 使用 saveImmediately = false
                await AppendLevelField<float>(fieldKey, 0f, false);
            }
        }
    }

    /// <summary>
    /// 保存当前 userData 到 Cloud Save 中（各字段单独存储）
    /// </summary>
    public async Task SaveUserData()
    {
        try
        {
            // 将 userData 中所有键值对转成列表
            List<KeyValuePair<string, object>> allData = new List<KeyValuePair<string, object>>(userData);
            int batchSize = 20; // 每个批次最大键数，根据实际限制调整

            for (int i = 0; i < allData.Count; i += batchSize)
            {
                int count = Math.Min(batchSize, allData.Count - i);
                Dictionary<string, object> batch = new Dictionary<string, object>();
                for (int j = 0; j < count; j++)
                {
                    var kvp = allData[i + j];
                    batch[kvp.Key] = kvp.Value;
                }
                await CloudSaveService.Instance.Data.ForceSaveAsync(batch);
                Debug.Log("Batch saved: " + JsonConvert.SerializeObject(batch));
            }
            Debug.Log("All user data saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving user data: {e.Message}");
        }
    }


    #region 更新方法

    // 更新关卡时长列表中的最后一个元素（单位：分钟），存储为 JSON 数组字符串
    public async Task UpdateLevelTime(float newDuration, bool saveImmediately = true)
    {
        string currentJson = userData[Key_Mmcz_time] as string;
        userData[Key_Mmcz_time] = UpdateLastElementInJsonArray<float>(currentJson, newDuration);
        Debug.Log("UpdateLevelTime: " + userData[Key_Mmcz_time]);
        await UpdateSingleField(Key_Mmcz_time, userData[Key_Mmcz_time]);
    }

    // 每次登录时追加一个初始关卡时长记录（初始值为 0）
    public async Task AppendLevelTime(bool saveImmediately = true)
    {
        string currentJson = userData.ContainsKey(Key_Mmcz_time) ? userData[Key_Mmcz_time] as string : "[]";
        userData[Key_Mmcz_time] = AppendToJsonArray<float>(currentJson, 0f);
        Debug.Log("AppendLevelTime: " + userData[Key_Mmcz_time]);
        if (saveImmediately)
        {
            await UpdateSingleField(Key_Mmcz_time, userData[Key_Mmcz_time]);
        }
    }

    // 更新创建次数列表中的最后一个元素，累计加一
    public async Task UpdateCreateCount(bool saveImmediately = true)
    {
        string json = userData[Key_Mmcz_create] as string;
        List<int> list = JsonArrayToList<int>(json);
        if (list.Count > 0)
        {
            list[list.Count - 1]++;
        }
        else
        {
            list.Add(1);
        }
        userData[Key_Mmcz_create] = ListToJsonArray(list);
        Debug.Log("UpdateCreateCount: " + userData[Key_Mmcz_create]);
        if (saveImmediately)
        {
            await UpdateSingleField(Key_Mmcz_create, userData[Key_Mmcz_create]);
        }
    }

    // 每次登录时追加一个初始创建次数记录（初始值 0）
    public async Task AppendCreateCount(bool saveImmediately = true)
    {
        string currentJson = userData.ContainsKey(Key_Mmcz_create) ? userData[Key_Mmcz_create] as string : "[]";
        userData[Key_Mmcz_create] = AppendToJsonArray<int>(currentJson, 0);
        Debug.Log("AppendCreateCount: " + userData[Key_Mmcz_create]);
        if (saveImmediately)
        {
            await UpdateSingleField(Key_Mmcz_create, userData[Key_Mmcz_create]);
        }
    }
    public async Task UpdateSingleField(string key, object value)
    {
        try
        {
            // 创建一个只包含目标键和值的字典
            var updateData = new Dictionary<string, object>()
            {
                { key, value }
            };

            // 调用 Cloud Save 更新单个字段
            await CloudSaveService.Instance.Data.ForceSaveAsync(updateData);
            Debug.Log($"Field {key} updated with value: {value}");
        
            // 同时更新本地缓存（如果需要）
            userData[key] = value;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating field {key}: {e.Message}");
        }
    }

    // 将当前会话的创建次数初始化为 0（更新最后一个元素）
    public async Task InitialCreateCount(bool saveImmediately = true)
    {
        string json = userData[Key_Mmcz_create] as string;
        List<int> list = JsonArrayToList<int>(json);
        if (list.Count > 0)
        {
            list[list.Count - 1] = 0;
        }
        else
        {
            list.Add(0);
        }
        userData[Key_Mmcz_create] = ListToJsonArray(list);
        Debug.Log("InitialCreateCount: " + userData[Key_Mmcz_create]);
        if (saveImmediately)
        {
            await UpdateSingleField(Key_Mmcz_create, userData[Key_Mmcz_create]);
        }
    }
    
    public async Task IncrementEntries(bool saveImmediately = true)
    {
        int current = 0;
        if (userData.ContainsKey(Key_Mmcz_entries))
        {
            int.TryParse(userData[Key_Mmcz_entries].ToString(), out current);
        }
        current++;
        userData[Key_Mmcz_entries] = current;
        Debug.Log("IncrementEntries: " + current);
        if (saveImmediately)
        {
            await UpdateSingleField(Key_Mmcz_entries, userData[Key_Mmcz_entries]);
        }
    }

    public async Task IncrementSuccess(bool saveImmediately = true)
    {
        int current = 0;
        if (userData.ContainsKey(Key_Mmcz_success))
        {
            int.TryParse(userData[Key_Mmcz_success].ToString(), out current);
        }
        current++;
        userData[Key_Mmcz_success] = current;
        Debug.Log("IncrementSuccess: " + current);
        if (saveImmediately)
        {
            await UpdateSingleField(Key_Mmcz_success, userData[Key_Mmcz_success]);
        }
    }
    // 追加新元素到指定字段的 JSON 数组中，并保存到云端
    public async Task AppendLevelField<T>(string fieldKey, T newElement, bool saveImmediately = true)
    {
        string currentJson = userData.ContainsKey(fieldKey) ? userData[fieldKey] as string : "[]";
        userData[fieldKey] = AppendToJsonArray<T>(currentJson, newElement);
        Debug.Log($"AppendLevelField({fieldKey}): " + userData[fieldKey]);
        if (saveImmediately)
        {
            await UpdateSingleField(fieldKey, userData[fieldKey]);
        }
    }

// 更新指定字段 JSON 数组中最后一个元素，并保存到云端
    public async Task UpdateLevelField<T>(string fieldKey, T newValue, bool saveImmediately = true)
    {
        string currentJson = userData[fieldKey] as string;
        userData[fieldKey] = UpdateLastElementInJsonArray<T>(currentJson, newValue);
        Debug.Log($"UpdateLevelField({fieldKey}): " + userData[fieldKey]);
        if (saveImmediately)
        {
            await UpdateSingleField(fieldKey, userData[fieldKey]);
        }
    }


    #endregion

    #region 辅助方法：处理 JSON 数组

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

    private string ListToJsonArray<T>(List<T> list)
    {
        return JsonConvert.SerializeObject(list);
    }

    private string AppendToJsonArray<T>(string jsonArray, T newElement)
    {
        List<T> list = JsonArrayToList<T>(jsonArray);
        list.Add(newElement);
        return ListToJsonArray(list);
    }

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
