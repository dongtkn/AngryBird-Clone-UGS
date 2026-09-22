using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public static bool IsServicesReady { get; private set; }
    public static event Action OnServicesReady;

    [SerializeField] private GameObject loadingOverlay;

    private async void Start()
    {
        if (loadingOverlay != null) loadingOverlay.SetActive(true);

        try
        {
            await UnityServices.InitializeAsync();
            IsServicesReady = true;
            Debug.Log("Unity Services initialized successfully.");
            OnServicesReady?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Khởi tạo UGS thất bại: {ex.Message}");
        }

        if (loadingOverlay != null) loadingOverlay.SetActive(false);
    }
}