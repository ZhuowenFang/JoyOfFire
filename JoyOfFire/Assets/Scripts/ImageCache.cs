using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public static class ImageCache
{
    private static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

    /// <summary>
    /// 获取指定 URL 的图片，如果缓存中没有就下载
    /// </summary>
    /// <param name="url">图片的 URL</param>
    /// <param name="callback">下载完成后的回调，返回 Texture2D（失败返回 null）</param>
    public static IEnumerator GetTexture(string url, Action<Texture2D> callback)
    {
        // 如果缓存中已经存在，则直接回调返回缓存图片
        if (cache.ContainsKey(url))
        {
            callback(cache[url]);
            yield break;
        }

        // 使用 UnityWebRequest 下载图片
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError("Error downloading image: " + request.error);
            callback(null);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            // 保存到缓存中
            cache[url] = texture;
            callback(texture);
        }
    }
}