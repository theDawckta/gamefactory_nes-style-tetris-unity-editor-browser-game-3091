using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LeaderboardServiceTests
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (LeaderboardService.Instance != null)
            Object.Destroy(LeaderboardService.Instance.gameObject);
        yield return null;

        var go = new GameObject("LeaderboardService");
        go.AddComponent<LeaderboardService>();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (LeaderboardService.Instance != null)
            Object.Destroy(LeaderboardService.Instance.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Instance_IsNotNullAfterAwake()
    {
        Assert.IsNotNull(LeaderboardService.Instance);
        yield return null;
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWhenCacheIsNull()
    {
        Assert.IsTrue(LeaderboardService.Instance.IsTopFive(0));
        yield return null;
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWhenFewerThanFiveRealEntries()
    {
        SetCache(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 900 }
        });

        Assert.IsTrue(LeaderboardService.Instance.IsTopFive(1));
        yield return null;
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWithPlaceholderEntry()
    {
        SetCache(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 900 },
            new LeaderboardEntry { Initials = "CCC", Score = 800 },
            new LeaderboardEntry { Initials = "---", Score = 0 },
            new LeaderboardEntry { Initials = "---", Score = 0 }
        });

        Assert.IsTrue(LeaderboardService.Instance.IsTopFive(1));
        yield return null;
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWhenScoreBeatsLowest()
    {
        SetCache(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 900 },
            new LeaderboardEntry { Initials = "CCC", Score = 800 },
            new LeaderboardEntry { Initials = "DDD", Score = 700 },
            new LeaderboardEntry { Initials = "EEE", Score = 600 }
        });

        Assert.IsTrue(LeaderboardService.Instance.IsTopFive(601));
        yield return null;
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsFalseWhenScoreDoesNotBeatLowest()
    {
        SetCache(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 900 },
            new LeaderboardEntry { Initials = "CCC", Score = 800 },
            new LeaderboardEntry { Initials = "DDD", Score = 700 },
            new LeaderboardEntry { Initials = "EEE", Score = 600 }
        });

        Assert.IsFalse(LeaderboardService.Instance.IsTopFive(600));
        yield return null;
    }

    [UnityTest]
    public IEnumerator FetchScores_CallsOnCompleteEvenWhenServerUnavailable()
    {
        bool called = false;
        LeaderboardService.Instance.FetchScores(entries => { called = true; });

        float elapsed = 0f;
        while (!called && elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(called, "onComplete was not called within timeout");
    }

    [UnityTest]
    public IEnumerator SubmitScore_CallsOnCompleteEvenWhenServerUnavailable()
    {
        bool called = false;
        LeaderboardService.Instance.SubmitScore("TST", 100, entries => { called = true; });

        float elapsed = 0f;
        while (!called && elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(called, "onComplete was not called within timeout");
    }

    [UnityTest]
    public IEnumerator FetchScores_PassesNullToOnCompleteWhenNoServerAndNoCache()
    {
        LeaderboardEntry[] result = new LeaderboardEntry[0];
        bool called = false;
        LeaderboardService.Instance.FetchScores(entries =>
        {
            result = entries;
            called = true;
        });

        float elapsed = 0f;
        while (!called && elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(called);
        Assert.IsNull(result);
    }

    private static void SetCache(LeaderboardEntry[] entries)
    {
        var field = typeof(LeaderboardService).GetField(
            "_cachedScores", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(LeaderboardService.Instance, entries);
    }
}
