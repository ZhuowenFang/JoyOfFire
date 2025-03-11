using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance;

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await InitializeServicesAndAuthentication();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task InitializeServicesAndAuthentication()
    {
        try
        {
            // 初始化 Unity Services
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services Initialized");

            // 检查是否已登录
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // 匿名登录
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Anonymous sign-in successful. PlayerID: " + AuthenticationService.Instance.PlayerId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Initialization failed: " + e.Message);
        }
    }
}
