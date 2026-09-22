using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Shots")]
    public int MaxNumberOfShots = 3;
    private int usedNumberOfShots;
    private IconHandler iconHandler;

    [Header("Timing")]
    [SerializeField] private float secondToWaitBeforeDeathCheck = 3f;

    [Header("UI Panels")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject leaderboardPanel; // Panel Bảng xếp hạng
    [SerializeField] private GameObject nextLevelButton;  // Nút Qua Màn
    [SerializeField] private SlingShotHandler slingShotHandler;

    [Header("Scene Names")]
    [SerializeField] private string loginSceneName = "SceneLogin";
    [SerializeField] private string nextSceneName = "Scene2"; // Scene2 hoặc SceneThankYou

    [Header("Score")]
    [SerializeField] private int baseScore = 1000;

    private bool isGameEnd;
    private readonly List<PigBad> pigs = new List<PigBad>();
    private Coroutine checkDeathCoroutine;

    private void Awake()
    {
        Time.timeScale = 1f; // Khôi phục lại thời gian thực cho UI và Physics
        if (!GameBootstrap.IsServicesReady || AuthenticationService.Instance == null || !AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Chưa đăng nhập! Đang chuyển về SceneLogin...");
            SceneManager.LoadScene(loginSceneName);
            return;
        }

        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        iconHandler = FindAnyObjectByType<IconHandler>();
        pigs.AddRange(FindObjectsByType<PigBad>(FindObjectsInactive.Exclude));

        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler?.UseShot(usedNumberOfShots);
        CheckForLastShot();
    }

    public bool HasEnoughShots() => usedNumberOfShots < MaxNumberOfShots;

    public void CheckForLastShot()
    {
        if (usedNumberOfShots >= MaxNumberOfShots)
        {
            if (checkDeathCoroutine != null) StopCoroutine(checkDeathCoroutine);
            checkDeathCoroutine = StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(secondToWaitBeforeDeathCheck);
        EndGame(pigs.Count == 0);
    }

    public void RemovePigBad(PigBad pig)
    {
        pigs.Remove(pig);
        if (pigs.Count == 0)
        {
            if (checkDeathCoroutine != null)
            {
                StopCoroutine(checkDeathCoroutine);
                checkDeathCoroutine = null;
            }
            EndGame(true);
        }
    }

    private async void EndGame(bool isWin)
    {
        if (isGameEnd) return;
        isGameEnd = true;

        if (slingShotHandler != null) slingShotHandler.enabled = false;
        if (endGamePanel != null) endGamePanel.SetActive(true);

        // Nút qua màn chỉ hiển thị khi Thắng
        if (nextLevelButton != null) nextLevelButton.SetActive(isWin);

        int finalScore = isWin ? CalculateScore() : 0;

        if (isWin && ScoreManager.Instance != null)
        {
            try
            {
                await ScoreManager.Instance.SubmitScore(finalScore);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Lỗi gửi điểm: {ex.Message}");
            }
        }
    }

    private int CalculateScore()
    {
        return usedNumberOfShots switch
        {
            1 => baseScore,
            2 => baseScore / 2,
            3 => (int)(baseScore * 0.25f),
            _ => 0
        };
    }

    #region UI Buttons Event Handlers

    // 1. Nút Chơi Lại (Load lại chính Scene hiện tại)
    public void OnRestartButtonPressed()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // 2. Nút Qua Màn (Load nextSceneName đã cấu hình ở Inspector)
    public void OnNextLevelButtonPressed()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // 3. Nút Bảng Xếp Hạng (Bật/Tắt Leaderboard Panel an toàn)
    public void OnLeaderboardButtonPressed()
    {
        if (leaderboardPanel != null)
        {
            bool isActive = leaderboardPanel.activeSelf;
            leaderboardPanel.SetActive(!isActive);

            // Nếu mở panel lên thì gọi load dữ liệu
            if (!isActive)
            {
                var leaderboardUI = leaderboardPanel.GetComponent<LeaderboardUI>();
                if (leaderboardUI != null)
                {
                    leaderboardUI.RefreshLeaderboard();
                }
            }
        }
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}