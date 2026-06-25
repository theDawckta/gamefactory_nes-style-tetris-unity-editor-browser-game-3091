using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public struct LeaderboardEntry
{
    public string Initials;
    public int Score;
}

public class LeaderboardService : MonoBehaviour
{
    public static LeaderboardService Instance { get; private set; }

    private const string BASE_URL = "http://localhost:3000";
    private LeaderboardEntry[] _cachedScores;

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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void FetchScores(Action<LeaderboardEntry[]> onComplete)
    {
        StartCoroutine(FetchScoresCoroutine(onComplete));
    }

    public void SubmitScore(string initials, int score, Action<LeaderboardEntry[]> onComplete)
    {
        StartCoroutine(SubmitScoreCoroutine(initials, score, onComplete));
    }

    public bool IsTopFive(int score)
    {
        if (_cachedScores == null || _cachedScores.Length < 5)
            return true;

        foreach (var entry in _cachedScores)
        {
            if (entry.Initials == "---")
                return true;
        }

        int lowestScore = int.MaxValue;
        foreach (var entry in _cachedScores)
        {
            if (entry.Score < lowestScore)
                lowestScore = entry.Score;
        }

        return score > lowestScore;
    }

    private IEnumerator FetchScoresCoroutine(Action<LeaderboardEntry[]> onComplete)
    {
        using (var request = UnityWebRequest.Get(BASE_URL + "/leaderboard"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(_cachedScores);
                yield break;
            }

            var entries = ParseEntries(request.downloadHandler.text);
            if (entries != null)
                _cachedScores = entries;
            onComplete?.Invoke(entries ?? _cachedScores);
        }
    }

    private IEnumerator SubmitScoreCoroutine(string initials, int score, Action<LeaderboardEntry[]> onComplete)
    {
        string body = "{\"initials\":\"" + initials + "\",\"score\":" + score + "}";
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        using (var request = new UnityWebRequest(BASE_URL + "/leaderboard", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(_cachedScores);
                yield break;
            }

            var entries = ParseEntries(request.downloadHandler.text);
            if (entries != null)
                _cachedScores = entries;
            onComplete?.Invoke(entries ?? _cachedScores);
        }
    }

    private LeaderboardEntry[] ParseEntries(string json)
    {
        try
        {
            var wrapper = JsonUtility.FromJson<EntryWrapper>("{\"entries\":" + json + "}");
            if (wrapper?.entries == null)
                return null;

            var result = new LeaderboardEntry[wrapper.entries.Length];
            for (int i = 0; i < wrapper.entries.Length; i++)
            {
                result[i] = new LeaderboardEntry
                {
                    Initials = wrapper.entries[i].initials,
                    Score = wrapper.entries[i].score
                };
            }
            return result;
        }
        catch
        {
            return null;
        }
    }

    [Serializable]
    private class EntryDto
    {
        public string initials;
        public int score;
    }

    [Serializable]
    private class EntryWrapper
    {
        public EntryDto[] entries;
    }
}
