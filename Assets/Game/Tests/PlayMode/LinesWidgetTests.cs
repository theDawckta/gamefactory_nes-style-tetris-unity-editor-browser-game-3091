using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LinesWidgetTests
{
    private GameObject _screenGo;
    private UIDocument _uiDoc;
    private GameScreen _gameScreen;
    private VisualElement _linesRegion;
    private GameObject _widgetGo;
    private LinesWidget _widget;
    private ScoringSystem _scoring;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _screenGo.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _gameScreen = _screenGo.AddComponent<GameScreen>();

        _linesRegion = new VisualElement { name = "linesRegion" };
        _uiDoc.rootVisualElement.Add(_linesRegion);

        _widgetGo = new GameObject("LinesWidget");
        _widget = _widgetGo.AddComponent<LinesWidget>();

        var field = typeof(LinesWidget).GetField(
            "_gameScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _gameScreen);

        _scoring = new ScoringSystem();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(_widgetGo);
        Object.Destroy(_screenGo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator LinesWidget_Initialize_BuildsUIIntoLinesRegion()
    {
        _widget.Initialize(_scoring);
        yield return null;

        int labelCount = 0;
        _linesRegion.Query<Label>().ForEach(_ => labelCount++);
        Assert.GreaterOrEqual(labelCount, 2, "Expected at least 2 labels (LINES header + value)");
    }

    [UnityTest]
    public IEnumerator LinesWidget_Initialize_ShowsZeroAtStart()
    {
        _widget.Initialize(_scoring);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.IsNotNull(valueLabel, "Value label should exist");
        Assert.AreEqual("0", valueLabel.text);
    }

    [UnityTest]
    public IEnumerator LinesWidget_HeaderLabel_IsPresent()
    {
        _widget.Initialize(_scoring);
        yield return null;

        Label headerLabel = null;
        _linesRegion.Query<Label>().ForEach(l =>
        {
            if (l.text == "LINES") headerLabel = l;
        });
        Assert.IsNotNull(headerLabel, "LINES header label should be present in linesRegion");
    }

    [UnityTest]
    public IEnumerator LinesWidget_OnStatsChanged_UpdatesLinesValue()
    {
        _widget.Initialize(_scoring);
        yield return null;

        _scoring.AddLines(3);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.IsNotNull(valueLabel, "Value label should exist");
        Assert.AreEqual("3", valueLabel.text);
    }

    [UnityTest]
    public IEnumerator LinesWidget_OnStatsChanged_AccumulatesLines()
    {
        _widget.Initialize(_scoring);
        yield return null;

        _scoring.AddLines(2);
        _scoring.AddLines(4);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.IsNotNull(valueLabel, "Value label should exist");
        Assert.AreEqual("6", valueLabel.text);
    }

    [UnityTest]
    public IEnumerator LinesWidget_OnDisable_UnsubscribesFromScoring()
    {
        _widget.Initialize(_scoring);
        yield return null;

        _widgetGo.SetActive(false);
        yield return null;

        _scoring.AddLines(1);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.AreEqual("0", valueLabel.text, "Label should not update after OnDisable");
    }

    [UnityTest]
    public IEnumerator LinesWidget_OnEnable_ResubscribesAfterDisable()
    {
        _widget.Initialize(_scoring);
        yield return null;

        _widgetGo.SetActive(false);
        yield return null;
        _widgetGo.SetActive(true);
        yield return null;

        _scoring.AddLines(2);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.AreEqual("2", valueLabel.text, "Label should update after re-enable");
    }

    private Label FindValueLabel()
    {
        Label valueLabel = null;
        _linesRegion.Query<Label>().ForEach(l =>
        {
            if (l.text != "LINES") valueLabel = l;
        });
        return valueLabel;
    }
}
