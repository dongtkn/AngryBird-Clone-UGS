using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject scoreItemPrefab; // Prefab có chứa TMP_Text
    [SerializeField] private TMP_Text statusText;

    public async void RefreshLeaderboard()
    {
        if (statusText != null) statusText.text = "Đang tải...";

        // Xóa các item cũ trong danh sách
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (ScoreManager.Instance == null)
        {
            if (statusText != null) statusText.text = "ScoreManager chưa sẵn sàng.";
            return;
        }

        List<Unity.Services.Leaderboards.Models.LeaderboardEntry> scores = await ScoreManager.Instance.FetchTopScoresAsync();

        if (scores == null || scores.Count == 0)
        {
            if (statusText != null) statusText.text = "Chưa có dữ liệu điểm.";
            return;
        }

        if (statusText != null) statusText.text = "";

        foreach (var entry in scores)
        {
            GameObject item = Instantiate(scoreItemPrefab, container);
            var textComponent = item.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = $"{entry.Rank + 1}. {entry.PlayerName ?? entry.PlayerId} - {entry.Score} pts";
            }
        }
    }
}