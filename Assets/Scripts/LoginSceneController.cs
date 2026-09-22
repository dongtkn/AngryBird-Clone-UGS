using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginSceneController : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;
    public event Action<PlayerProfile> OnAvatarUpdate;
    private PlayerInfo playerInfo;
    private PlayerProfile playerProfile;
    public PlayerProfile PlayerProfile => playerProfile;

    private void Start()
    {
        if (GameBootstrap.IsServicesReady)
        {
            SubscribeToPlayerAccounts();
        }
        else
        {
            GameBootstrap.OnServicesReady += SubscribeToPlayerAccounts;
        }
    }
    private void SubscribeToPlayerAccounts()
    {
        GameBootstrap.OnServicesReady -= SubscribeToPlayerAccounts;
        PlayerAccountService.Instance.SignedIn += SignedIn;
    }

    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");

            // 1. Lấy PlayerId trực tiếp từ SDK (luôn luôn có sẵn sau khi sign in thành công)
            string playerId = AuthenticationService.Instance.PlayerId;

            // 2. Lấy Name an toàn (bắt Exception riêng nếu tài khoản chưa có tên trên server)
            string playerName = "";
            try
            {
                playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            }
            catch
            {
                // Nếu tài khoản chưa được đặt tên trên server Unity, lấy tạm 5 ký tự đầu của ID làm tên
                playerName = $"Player_{playerId.Substring(0, 5)}";
            }

            if (string.IsNullOrEmpty(playerName))
            {
                playerName = $"Player_{playerId.Substring(0, 5)}";
            }

            // 3. Gán dữ liệu chuẩn vào struct
            playerProfile.PlayerId = playerId;
            playerProfile.Name = playerName;

            // 4. Bắn sự kiện cập nhật UI
            OnSignedIn?.Invoke(playerProfile);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnDestroy()
    {
        GameBootstrap.OnServicesReady -= SubscribeToPlayerAccounts;
        if (PlayerAccountService.Instance != null)
        {
            PlayerAccountService.Instance.SignedIn -= SignedIn;
        }
    }

    public void LoadGameScene1()
    {
        SceneManager.LoadScene("GameScene");
    }
}

[System.Serializable]
public struct PlayerProfile
{
    public string PlayerId;
    public string Name;
}