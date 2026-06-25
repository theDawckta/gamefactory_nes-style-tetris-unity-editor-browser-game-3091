using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class FinalScoreWidgetTests
{
    private GameObject _widgetGo;
    private FinalScoreWidget _widget;
    private GameObject _screenGo;
    private GameOverScreen _gameOverScreen;
    private VisualElement _finalScoreRegion;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameOverScreen");
        _gameOverScreen = _screenGo.AddComponent<GameOverScreen>();
        _finalScoreRegion = new VisualElement();
        _gameOverScreen.FinalScoreRegion = _finalScoreRegion;

        _widgetGo = new GameObject("FinalScoreWidget");
        _widget = _widgetGo.AddComponent<FinalScoreWidget>();

        var field = typeof(FinalScoreWidget).GetField(
            "_gameOverScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _gameOverScreen);

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
    public IEnumerator FinalScoreWidget_SetScore_FormatsSevenDigitZeroPadded()
    {
        _widget.SetScore(12345);
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual("0012345", scoreLabel.text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_Zero_DisplaysAllZeroes()
    {
        _widget.SetScore(0);
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual("0000000", scoreLabel.text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_ScoreHeaderLabel_IsPresent()
    {
        yield return null;

        Label headerLabel = null;
        _finalScoreRegion.Query<Label>().ForEach(l =>
        {
            if (l.text == "SCORE") headerLabel = l;
        });
        Assert.IsNotNull(headerLabel, "SCORE header label should be present in finalScoreRegion");
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_BuildsUIIntoFinalScoreRegion()
    {
        yield return null;

        int labelCount = 0;
        _finalScoreRegion.Query<Label>().ForEach(_ => labelCount++);
        Assert.GreaterOrEqual(labelCount, 2, "Expected at least 2 labels (SCORE header + value) in finalScoreRegion");
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_InitialScoreValue_IsZeroPadded()
    {
        yield return null;

        var scoreLabel = FindScoreValueLabel();
        Assert.IsNotNull(scoreLabel, "Score value label should exist");
        Assert.AreEqual(7, scoreLabel.text.Length, "Initial score label should be 7 characters");
    }

    private Label FindScoreValueLabel()
    {
        Label valueLabel = null;
        _finalScoreRegion.Query<Label>().ForEach(l =>
        {
            if (l.text != "SCORE") valueLabel = l;
        });
        return valueLabel;
    }
}
