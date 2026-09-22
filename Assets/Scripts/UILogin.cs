using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text userIdText;
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private Transform loginPanel, userPanel;
    [SerializeField] private LoginSceneController loginController;

    private PlayerProfile playerProfile;

    private void OnEnable()
    {
        loginButton.onClick.AddListener(LoginButtonPressed);
        loginController.OnSignedIn += LoginController_OnSignedIn;
        loginController.OnAvatarUpdate += LoginController_OnAvatarUpdate;
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(LoginButtonPressed);
        loginController.OnSignedIn -= LoginController_OnSignedIn;
        loginController.OnAvatarUpdate -= LoginController_OnAvatarUpdate;
    }

    private async void LoginButtonPressed()
    {
        // Kiểm tra xem Services đã ready chưa trước khi gọi đăng nhập
        if (!GameBootstrap.IsServicesReady)
        {
            Debug.LogWarning("Hệ thống đang khởi tạo, vui lòng đợi...");
            return;
        }

        loginButton.interactable = false; // Tránh spam click
        await loginController.InitSignIn();
        loginButton.interactable = true;
    }

    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        playerProfile = profile;
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);

        // Gán dữ liệu hiển thị
        userIdText.text = $"ID: {playerProfile.PlayerId}";
        userNameText.text = $"Name: {playerProfile.Name}";
    }

    private void LoginController_OnAvatarUpdate(PlayerProfile profile)
    {
        playerProfile = profile;
    }
}