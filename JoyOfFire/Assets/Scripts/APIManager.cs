using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    public class AcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // 始终返回 true，接受所有证书
            return true;
        }
    }

    public static APIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void GetRequest(string endpoint, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendRequest(endpoint, UnityWebRequest.kHttpVerbGET, null, onSuccess, onError));
    }

    public void PostRequest(string endpoint, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendRequest(endpoint, UnityWebRequest.kHttpVerbPOST, jsonData, onSuccess, onError));
    }

    public void GetLevelData(string level, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"https://joy-fire-dev.czczcz.xyz/api/v1/level/mmcz/{level}";
        GetRequest(url, onSuccess, onError);
    }
    
    public void GetItemData(string itemId, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"https://joy-fire-dev.czczcz.xyz/api/v1/items/{itemId}";
        GetRequest(url, onSuccess, onError);
    }
    
    public void GetMonsterData(string levelOrder, string monsterId, Action<List<MonsterAttributes>> onSuccess, Action<string> onError)
    {
        string url = $"https://joy-fire-dev.czczcz.xyz/api/v1/monsters/level?levelOrder={levelOrder}&monsterId={monsterId}";
        StartCoroutine(GetMonsterRequest(url, onSuccess, onError));
    }
    
    private IEnumerator GetMonsterRequest(string url, Action<List<MonsterAttributes>> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new AcceptAllCertificates();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log(jsonResponse);
                List<ClassManager.MonsterData> monsterDataList = JsonConvert.DeserializeObject<List<ClassManager.MonsterData>>(jsonResponse);

                List<MonsterAttributes> monsters = new List<MonsterAttributes>();
                foreach (var data in monsterDataList)
                {
                    monsters.Add(NewCharacterManager.ConvertToMonsterAttributes(data));
                }

                onSuccess?.Invoke(monsters);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    public void CreateCharacter(string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        string url = "https://joy-fire-dev.czczcz.xyz/api/v1/character/create";
        PostRequest(url, jsonData, onSuccess, onError);
    }
    
    public void UpdateCharacter(string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        string url = "https://joy-fire-dev.czczcz.xyz/api/v1/character/update";
        PostRequest(url, jsonData, onSuccess, onError);
    }
    
    public void StoreCharacter(string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        string url = "https://joy-fire-dev.czczcz.xyz/api/v1/character/updateAttributes";
        PostRequest(url, jsonData, onSuccess, onError);
    }
    
    public IEnumerator LoadImage(string url, Image image)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            request.certificateHandler = new AcceptAllCertificates();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            else
            {
                Debug.LogError($"Failed to load image: {request.error}");
            }
        }
    }
    
    private IEnumerator SendRequest(string url, string method, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = new UnityWebRequest(url, method);
        request.certificateHandler = new AcceptAllCertificates();

        if (!string.IsNullOrEmpty(jsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error {request.responseCode}: {request.error}");
            onError?.Invoke(request.error);
        }
    }
}
