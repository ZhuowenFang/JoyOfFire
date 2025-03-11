using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using Object = UnityEngine.Object;

public class CloudSaveQueryExporter : MonoBehaviour
{
    // 填写你的项目 ID 和环境 ID
    public string projectId = "d09bb6ca-70f0-44d9-9981-e07aa72fea0e";
    public string environmentId = "6517cfaf-38bd-4375-85b0-f235235b7265";
    // 具有管理员权限的服务帐户 Token
    public string adminToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6InVuaXR5LWtleXM6MzU0OWFkNDMtN2RjYS00YTdkLTg2MWMtYjJmM2ZjZmMyZTAyIiwiamt1IjoiaHR0cHM6Ly9rZXlzLnNlcnZpY2VzLnVuaXR5LmNvbS8ifQ.eyJleHAiOjE3NDE2NzkwNTEsImlhdCI6MTc0MTY3NTQ1MSwibmJmIjoxNzQxNjc1NDUxLCJqdGkiOiI0MDg2ODAzZC02YTVkLTRkZWYtOTYxYS0zYzZjZjRiZTgwNjMiLCJzdWIiOiI4YTQ4Mjk4My00ODc0LTRmZmItOGY0MS1mNTRmNTAzOTVjOGUiLCJ2ZXJzaW9uIjoxLCJpc3MiOiJodHRwczovL3NlcnZpY2VzLnVuaXR5LmNvbSIsImF1ZCI6WyJ1cGlkOmQwOWJiNmNhLTcwZjAtNDRkOS05OTgxLWUwN2FhNzJmZWEwZSIsImVudklkOjY1MTdjZmFmLTM4YmQtNDM3NS04NWIwLWYyMzUyMzViNzI2NSJdLCJzY29wZXMiOlsiY2xvdWRfc2F2ZS5kYXRhLmRlbGV0ZSIsImNsb3VkX3NhdmUuZGF0YS5yZWFkIiwiY2xvdWRfc2F2ZS5kYXRhLnVwc2VydCIsImNsb3VkX3NhdmUuZW50aXRpZXMubGlzdCIsImNsb3VkX3NhdmUuZmlsZXMuZGVsZXRlIiwiY2xvdWRfc2F2ZS5maWxlcy5saXN0IiwiY2xvdWRfc2F2ZS5maWxlcy5yZWFkIiwiY2xvdWRfc2F2ZS5maWxlc19lbnYucHJvdmlzaW9uIiwiY2xvdWRfc2F2ZS5maWxlc19lbnYucmVhZCIsImNsb3VkX3NhdmUuaW5kZXhlcy5jcmVhdGUiLCJjbG91ZF9zYXZlLmluZGV4ZXMuZGVsZXRlIiwiY2xvdWRfc2F2ZS5pbmRleGVzLmdldCIsImNsb3VkX3NhdmUuaW5kZXhlcy5saXN0Il0sImFwaUtleVB1YmxpY0lkZW50aWZpZXIiOiI2ZGZjMDI3Ni1kOWJlLTQzNGEtODlkZi0wYTJmZTc2YTdlYWIifQ.l0_QuChrDOdvJCJ88kJWNEuhI1RSRZmcfd-NDm6a1pkVrmi0A0SsGAVosDuHWvK8I8VUAuojhvgw2LNybHk2K7IOzLxSpDGim6bTmkTX69wOsG_HRunyd5vjno-3mioGdF-3VKCOSxVeMu1UAcYqyNaltnlWX5oJfYPnsaUwYvWKNnmjswVLsLzmo0pl1F_VCuUp3Q20eJC892c7XDA01L631089OcmYKQ9cGpjnQ52gp1QPUtTMlCOxWHvAj2Y5tR5735I5LNwp4Q77FeNv1qP2ZwsF5zD6QVnR63sTjhW-8MH570M2Yckuraqm3rbg960dDkQoE8TLg85Ms4ei4A";
    
    public CloudSaveQueryExporter instance;
    
    [Serializable]
    public class FieldFilter
    {
        // “key” 必须是 1-50 个字符，只允许英文字母、数字、下划线和连字符
        public string key;
        // “value” 是对象类型，通常你可以传入一个空字符串或其它默认值
        public object value;
        // “op” 必须是 "EQ", "NE", "LT", "LE", "GT" 或 "GE" 之一
        public string op;
        // “asc” 指定是否按升序排序
        public bool asc;
    }
    private void Awake()
    {
        instance = this;
    }

    // 查询返回时你想要的字段
    // 注意：如果你没有在 Dashboard 中为 Cloud Save 配置索引，
    // 这个查询可能会失败，因为 “If no index is available to fulfil the query then the query will fail.”
    private List<string> returnKeys = new List<string>()
    {
        "Mmcz_time",
        "Mmcz_success",
        "Mmcz_create"
    };
    private List<FieldFilter> fields = new List<FieldFilter>()
    {
        new FieldFilter() { key = "Mmcz_success", value = null, op = "NE", asc = true }    };


    // 分页参数
    public int offset = 0;
    public int limit = 100; // 每次最多返回 100 条

    // 调用该方法开始查询并导出 CSV
    public void QueryAndExport()
    {
        StartCoroutine(QueryAllPlayersCoroutine());
    }

    private IEnumerator QueryAllPlayersCoroutine()
    {
        string url = $"https://services.api.unity.com/cloud-save/v1/data/projects/{projectId}/environments/{environmentId}/players/query";

        // 构造查询对象
        var queryBody = new
        {
            fields = fields,
            returnKeys = returnKeys,
            offset = offset,
            limit = limit,
            sampleSize = 5 // 不取随机样本，返回所有结果（或使用默认数量）
        };

        string jsonBody = JsonConvert.SerializeObject(queryBody);
        Debug.Log("Query body: " + jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Basic 6dfc0276-d9be-434a-89df-0a2fe76a7eab:vyAQkDtm8J_eZpAA3AZjfuVg-njWSm_N");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.Success)
#else
        if (!request.isNetworkError && !request.isHttpError)
#endif
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Query Response: " + responseText);

            // 假设返回数据为一个 JSON 对象，其中包含一个字段 "results" 是一个数组，每个元素是一个玩家的存储数据（键值对形式）。
            // 根据文档，返回的数据格式可能是：
            // { "results": [ { "key": "user_x_data", "data": "{\"Online_status\":1, ...}" }, ... ] }
            // 你需要根据实际返回格式进行调整
            CloudSaveQueryResult queryResult = JsonConvert.DeserializeObject<CloudSaveQueryResult>(responseText);
            List<Dictionary<string, object>> allPlayerData = new List<Dictionary<string, object>>();

            foreach (var result in queryResult.results)
            {
                // 为每个玩家记录创建一个字典，先存储 id
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["id"] = result.id;
                // 将 data 数组中的每个项存储到字典中，key 为 field.key, value 为 field.value
                foreach (var field in result.data)
                {
                    dict[field.key] = field.value;
                }
                allPlayerData.Add(dict);
            }


            // 导出 CSV 文件
            ExportToCSV(allPlayerData, "AllPlayerData.csv");
        }
        else
        {
            Debug.LogError($"Query failed: Response Code: {request.responseCode}, Error: {request.error}, Response: {request.downloadHandler.text}");
        }
    }

    private void ExportToCSV(List<Dictionary<string, object>> allPlayerData, string fileName)
    {
        StringBuilder csvBuilder = new StringBuilder();
        // CSV 头部
        csvBuilder.AppendLine("id,Mmcz_time,Mmcz_entries,Mmcz_success,Mmcz_create");

        foreach (var user in allPlayerData)
        {
            string id = user.ContainsKey("id") ? EscapeCSVField(user["id"].ToString()) : "";
            string mmczTime = user.ContainsKey("Mmcz_time") ? EscapeCSVField(user["Mmcz_time"].ToString()) : "";
            string mmczEntries = user.ContainsKey("Mmcz_entries") ? EscapeCSVField(user["Mmcz_entries"].ToString()) : "";
            string mmczSuccess = user.ContainsKey("Mmcz_success") ? EscapeCSVField(user["Mmcz_success"].ToString()) : "";
            string mmczCreate = user.ContainsKey("Mmcz_create") ? EscapeCSVField(user["Mmcz_create"].ToString()) : "";

            csvBuilder.AppendLine($"{id},{mmczTime},{mmczEntries},{mmczSuccess},{mmczCreate}");
        }

        // 将 CSV 文件保存到指定路径
        string filePath = @"C:\Users\13404\Desktop\" + fileName;
        System.IO.File.WriteAllText(filePath, csvBuilder.ToString());
        Debug.Log("CSV file saved to: " + filePath);
    }
    private string EscapeCSVField(string field)
    {
        // 如果字段包含逗号、双引号或换行符，则需要转义
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            // 将双引号替换为两个双引号
            field = field.Replace("\"", "\"\"");
            // 用双引号包裹字段
            return $"\"{field}\"";
        }
        else
        {
            return field;
        }
    }

}


/// <summary>
/// 根据 Cloud Save Query API 的返回格式创建一个辅助类
/// 具体字段可能需要根据实际返回数据进行调整
/// </summary>
[Serializable]
public class FieldData
{
    public string key;
    public string value;
    // 其他字段（如 writeLock、modified、created）根据需要添加
}

[Serializable]
public class PlayerDataResult
{
    public string id;
    public List<FieldData> data;
}

[Serializable]
public class CloudSaveQueryResult
{
    public List<PlayerDataResult> results;
}
