using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class ScoreWidgetTests
{
    private GameObject _widgetGo;
    private ScoreWidget _widget;
    private GameObject _screenGo;
    private GameScreen _gameScreen;
    private UIDocument _uiDoc;
    private VisualElement _scoreRegion;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _screenGo.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _gameScreen = _screenGo.AddComponent<GameScreen>();

        yield return null; // wait for UIDocument to initialize

        _scoreRegion = new VisualElement { name = "scoreRegion" };
        _uiDoc.rootVisualElement.Add(_scoreRegion);

        _widgetGo = new GameObject("ScoreWidget");
        _widget = _widgetGo.AddComponent<ScoreWidget>();

        var field = typeof(ScoreWidget).GetField(
            "_gameScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _gameScreen);

        yield return null; // wait for ScoreWidget.Start()
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(_widgetGo);
        Object.Destroy(_screenGo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ScoreWidget_InitialScoreValue_IsZeroPaddedSevenDigits()
    {
        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual("0000000", scoreLabel.text);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ScoreWidget_ScoreHeaderLabel_IsPresent()
    {
        Label headerLabel = null;
        _scoreRegion.Query<Label>().ForEach(l => { if (l.text == "SCORE") headerLabel = l; });
        Assert.IsNotNull(headerLabel, "SCORE header label should be present in scoreRegion");
        yield return null;
    }

    [UnityTest]
    public IEnumerator ScoreWidget_BuildsUIIntoScoreRegion()
    {
        int labelCount = 0;
        _scoreRegion.Query<Label>().ForEach(_ => labelCount++);
        Assert.GreaterOrEqual(labelCount, 2, "Expected at least 2 labels (SCORE header + value) in scoreRegion");
        yield return null;
    }

    [UnityTest]
    public IEnumerator ScoreWidget_OnStatsChanged_UpdatesScoreLabel()
    {
        var scoringSystem = new ScoringSystem();
        _widget.SetScoringSystem(scoringSystem);

        scoringSystem.AddLines(4); // 1200 * (0+1) = 1200
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual("0001200", scoreLabel.text);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_ScoreFormat_IsSevenDigitZeroPadded()
    {
        var scoringSystem = new ScoringSystem();
        _widget.SetScoringSystem(scoringSystem);

        scoringSystem.AddLines(1); // 40 * 1 = 40
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual(7, scoreLabel.text.Length, "Score label should be 7 characters");
        Assert.AreEqual("0000040", scoreLabel.text);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_OnDisable_UnsubscribesFromScoringSystem()
    {
        var scoringSystem = new ScoringSystem();
        _widget.SetScoringSystem(scoringSystem);

        _widgetGo.SetActive(false); // triggers OnDisable
        yield return null;

        scoringSystem.AddLines(1); // widget unsubscribed, label should not update

        _widgetGo.SetActive(true); // re-enable
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual("0000000", scoreLabel.text, "Score should not have updated while widget was disabled");
    }

    [UnityTest]
    public IEnumerator ScoreWidget_OnEnable_ResubscribesAfterDisable()
    {
        var scoringSystem = new ScoringSystem();
        _widget.SetScoringSystem(scoringSystem);

        _widgetGo.SetActive(false);
        yield return null;
        _widgetGo.SetActive(true); // OnEnable resubscribes
        yield return null;

        scoringSystem.AddLines(4); // 1200 * 1 = 1200
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual("0001200", scoreLabel.text, "Score should update after re-enabling widget");
    }

    private Label FindScoreValueLabel()
    {
        Label valueLabel = null;
        _scoreRegion.Query<Label>().ForEach(l =>
        {
            if (l.text != "SCORE") valueLabel = l;
        });
        return valueLabel;
    }
}
