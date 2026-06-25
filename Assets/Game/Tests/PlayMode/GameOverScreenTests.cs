using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class GameOverScreenTests
{
    private GameObject _go;
    private UIDocument _uiDoc;
    private GameOverScreen _screen;
    private VisualElement _finalScoreRegion;
    private VisualElement _initialsRegion;
    private VisualElement _returnPromptRegion;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("GameOverScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _go.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<GameOverScreen>();
        yield return null;
        _finalScoreRegion = new VisualElement();
        _initialsRegion = new VisualElement();
        _returnPromptRegion = new VisualElement();
        _screen.FinalScoreRegion = _finalScoreRegion;
        _screen.InitialsRegion = _initialsRegion;
        _screen.ReturnPromptRegion = _returnPromptRegion;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(_go);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GameOverScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_MakesScreenVisible()
    {
        _screen.ShowWithScore(1000);
        yield return null;
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_PopulatesFinalScoreRegion()
    {
        _screen.ShowWithScore(12345);
        yield return null;
        Label found = null;
        _finalScoreRegion.Query<Label>().ForEach(l => found = l);
        Assert.IsNotNull(found, "A label should be added to finalScoreRegion");
        Assert.AreEqual("12345", found.text);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_NoLeaderboardService_ShowsReturnPrompt()
    {
        _screen.ShowWithScore(1000);
        yield return null;
        Assert.AreEqual(DisplayStyle.None, _initialsRegion.style.display.value);
        Assert.AreEqual(DisplayStyle.Flex, _returnPromptRegion.style.display.value);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_TopFive_ShowsInitialsHidesReturnPrompt()
    {
        var lsGo = new GameObject("LeaderboardService");
        lsGo.AddComponent<LeaderboardService>();
        yield return null;
        // _cachedScores is null => IsTopFive returns true
        _screen.ShowWithScore(9999);
        yield return null;
        Assert.AreEqual(DisplayStyle.Flex, _initialsRegion.style.display.value);
        Assert.AreEqual(DisplayStyle.None, _returnPromptRegion.style.display.value);
        Object.Destroy(lsGo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShowWithScore_NotTopFive_ShowsReturnPromptHidesInitials()
    {
        var lsGo = new GameObject("LeaderboardService");
        var ls = lsGo.AddComponent<LeaderboardService>();
        yield return null;
        var entries = new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 10000 },
            new LeaderboardEntry { Initials = "BBB", Score = 9000 },
            new LeaderboardEntry { Initials = "CCC", Score = 8000 },
            new LeaderboardEntry { Initials = "DDD", Score = 7000 },
            new LeaderboardEntry { Initials = "EEE", Score = 6000 },
        };
        typeof(LeaderboardService)
            .GetField("_cachedScores", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(ls, entries);
        _screen.ShowWithScore(100);
        yield return null;
        Assert.AreEqual(DisplayStyle.None, _initialsRegion.style.display.value);
        Assert.AreEqual(DisplayStyle.Flex, _returnPromptRegion.style.display.value);
        Object.Destroy(lsGo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator InitialsRegion_ReturnsExpectedElement()
    {
        yield return null;
        Assert.AreEqual(_initialsRegion, _screen.InitialsRegion);
    }

    [UnityTest]
    public IEnumerator ReturnPromptRegion_ReturnsExpectedElement()
    {
        yield return null;
        Assert.AreEqual(_returnPromptRegion, _screen.ReturnPromptRegion);
    }

    [UnityTest]
    public IEnumerator ShowReturnPrompt_HidesInitialsShowsReturnPrompt()
    {
        var lsGo = new GameObject("LeaderboardService");
        lsGo.AddComponent<LeaderboardService>();
        yield return null;
        _screen.ShowWithScore(9999);
        yield return null;
        Assert.AreEqual(DisplayStyle.Flex, _initialsRegion.style.display.value);
        _screen.ShowReturnPrompt();
        yield return null;
        Assert.AreEqual(DisplayStyle.None, _initialsRegion.style.display.value);
        Assert.AreEqual(DisplayStyle.Flex, _returnPromptRegion.style.display.value);
        Object.Destroy(lsGo);
        yield return null;
    }
}
