using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const string LeaderboardId = "SE192594";
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Gửi điểm cao nhất lên server.</summary>
    public async Task SubmitScore(int score)
    {
        if (!GameBootstrap.IsServicesReady)
        {
            Debug.LogWarning("[ScoreManager] Bỏ qua gửi điểm vì Unity Services chưa init thành công.");
            return;
        }
        if (!AuthenticationService.Instance.IsSignedIn) return;

        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            Debug.Log($"<color=green>[XÁC NHẬN SERVER] Điểm đã cập nhật: {response.Score}</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LỖI GỬI ĐIỂM UGS]: {e.Message}");
        }
    }

    /// <summary>Lấy top N điểm cao để hiển thị dashboard.</summary>
    public async Task<List<LeaderboardEntry>> FetchTopScoresAsync(int limit = 10)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            return new List<LeaderboardEntry>();
        }

        try
        {
            var data = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Limit = limit });
            return data.Results;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LỖI LẤY BẢNG XẾP HẠNG]: {e.Message}");
            return new List<LeaderboardEntry>();
        }
    }
}