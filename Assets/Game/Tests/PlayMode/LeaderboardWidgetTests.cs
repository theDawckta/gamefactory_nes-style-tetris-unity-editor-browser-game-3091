using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LeaderboardWidgetTests
{
    private GameObject _screenGo;
    private UIDocument _uiDoc;
    private StartScreen _startScreen;
    private VisualElement _leaderboardRegion;
    private GameObject _widgetGo;
    private LeaderboardWidget _widget;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("StartScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _screenGo.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _startScreen = _screenGo.AddComponent<StartScreen>();

        yield return null;

        _leaderboardRegion = new VisualElement { name = "leaderboardRegion" };
        _uiDoc.rootVisualElement.Add(_leaderboardRegion);

        _widgetGo = new GameObject("LeaderboardWidget");
        _widget = _widgetGo.AddComponent<LeaderboardWidget>();

        var field = typeof(LeaderboardWidget).GetField(
            "_startScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _startScreen);

        yield return null; // wait for Start()
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(_widgetGo);
        Object.Destroy(_screenGo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_StartsInLoadingState()
    {
        var loadingLabel = _leaderboardRegion.Q<Label>("loadingLabel");
        Assert.IsNotNull(loadingLabel, "loadingLabel should exist after BuildUI");
        Assert.AreEqual(DisplayStyle.Flex, loadingLabel.style.display.value, "loadingLabel should be visible initially");
        yield return null;
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_StartsWithRowsHidden()
    {
        foreach (var child in _leaderboardRegion.Children())
        {
            if (child is Label) continue;
            Assert.AreEqual(DisplayStyle.None, child.style.display.value, "All rows should be hidden initially");
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetLoading_ShowsLoadingLabel()
    {
        _widget.SetScores(new LeaderboardEntry[5]);
        yield return null;
        _widget.SetLoading();
        yield return null;
        var loadingLabel = _leaderboardRegion.Q<Label>("loadingLabel");
        Assert.IsNotNull(loadingLabel);
        Assert.AreEqual(DisplayStyle.Flex, loadingLabel.style.display.value, "loadingLabel should be visible after SetLoading");
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetLoading_HidesRows()
    {
        _widget.SetScores(new LeaderboardEntry[5]);
        yield return null;
        _widget.SetLoading();
        yield return null;
        foreach (var child in _leaderboardRegion.Children())
        {
            if (child is Label) continue;
            Assert.AreEqual(DisplayStyle.None, child.style.display.value, "All rows should be hidden after SetLoading");
        }
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScores_HidesLoadingLabel()
    {
        _widget.SetScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 900 },
            new LeaderboardEntry { Initials = "CCC", Score = 800 },
            new LeaderboardEntry { Initials = "DDD", Score = 700 },
            new LeaderboardEntry { Initials = "EEE", Score = 600 }
        });
        yield return null;
        var loadingLabel = _leaderboardRegion.Q<Label>("loadingLabel");
        Assert.IsNotNull(loadingLabel);
        Assert.AreEqual(DisplayStyle.None, loadingLabel.style.display.value, "loadingLabel should be hidden after SetScores");
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScores_WithFiveEntries_ShowsAllRows()
    {
        _widget.SetScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 5000 },
            new LeaderboardEntry { Initials = "BBB", Score = 4000 },
            new LeaderboardEntry { Initials = "CCC", Score = 3000 },
            new LeaderboardEntry { Initials = "DDD", Score = 2000 },
            new LeaderboardEntry { Initials = "EEE", Score = 1000 }
        });
        yield return null;

        int visibleRowCount = 0;
        foreach (var child in _leaderboardRegion.Children())
        {
            if (child is Label) continue;
            Assert.AreEqual(DisplayStyle.Flex, child.style.display.value, "Each row should be visible after SetScores");
            visibleRowCount++;
        }
        Assert.AreEqual(5, visibleRowCount, "There should be exactly 5 rows");
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScores_WithFiveEntries_DisplaysCorrectInitialsAndScore()
    {
        _widget.SetScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 5000 },
            new LeaderboardEntry { Initials = "BBB", Score = 4000 },
            new LeaderboardEntry { Initials = "CCC", Score = 3000 },
            new LeaderboardEntry { Initials = "DDD", Score = 2000 },
            new LeaderboardEntry { Initials = "EEE", Score = 1000 }
        });
        yield return null;

        var rows = new List<VisualElement>();
        foreach (var child in _leaderboardRegion.Children())
        {
            if (!(child is Label))
                rows.Add(child);
        }

        Assert.AreEqual(5, rows.Count);

        string[] expectedInitials = { "AAA", "BBB", "CCC", "DDD", "EEE" };
        string[] expectedScores = { "5000", "4000", "3000", "2000", "1000" };

        for (int i = 0; i < rows.Count; i++)
        {
            var labels = new List<Label>();
            rows[i].Query<Label>().ForEach(l => labels.Add(l));
            Assert.AreEqual(expectedInitials[i], labels[1].text, $"Row {i} initials mismatch");
            Assert.AreEqual(expectedScores[i], labels[2].text, $"Row {i} score mismatch");
        }
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScores_WithFewerThanFive_FillsRemainderWithPlaceholder()
    {
        _widget.SetScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 5000 },
            new LeaderboardEntry { Initials = "BBB", Score = 4000 }
        });
        yield return null;

        var rows = new List<VisualElement>();
        foreach (var child in _leaderboardRegion.Children())
        {
            if (!(child is Label))
                rows.Add(child);
        }

        Assert.AreEqual(5, rows.Count);

        for (int i = 2; i < rows.Count; i++)
        {
            var labels = new List<Label>();
            rows[i].Query<Label>().ForEach(l => labels.Add(l));
            Assert.AreEqual("---", labels[1].text, $"Row {i} initials should be placeholder");
            Assert.AreEqual("0", labels[2].text, $"Row {i} score should be 0");
        }
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScores_WithNullEntries_FillsAllWithPlaceholder()
    {
        _widget.SetScores(null);
        yield return null;

        var rows = new List<VisualElement>();
        foreach (var child in _leaderboardRegion.Children())
        {
            if (!(child is Label))
                rows.Add(child);
        }

        Assert.AreEqual(5, rows.Count);

        foreach (var row in rows)
        {
            var labels = new List<Label>();
            row.Query<Label>().ForEach(l => labels.Add(l));
            Assert.AreEqual("---", labels[1].text, "All initials should be placeholder for null entries");
            Assert.AreEqual("0", labels[2].text, "All scores should be 0 for null entries");
        }
    }
}
