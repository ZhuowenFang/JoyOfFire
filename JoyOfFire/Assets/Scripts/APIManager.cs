using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    
    public void CreateCharacter(string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        string url = "https://joy-fire-dev.czczcz.xyz/api/v1/character/create";
        PostRequest(url, jsonData, onSuccess, onError);
    }

    public IEnumerator LoadImage(string url, Image image)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
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
    private IEnumerator SendRequest(
        string url,
        string method,
        string jsonData,
        Action<string> onSuccess,
        Action<string> onError)
    {
        UnityWebRequest request = new UnityWebRequest(url, method);

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
