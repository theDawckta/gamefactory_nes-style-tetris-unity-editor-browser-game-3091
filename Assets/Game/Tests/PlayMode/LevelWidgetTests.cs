using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LevelWidgetTests
{
    private GameObject _screenGo;
    private UIDocument _uiDoc;
    private GameScreen _gameScreen;
    private VisualElement _levelRegion;
    private GameObject _widgetGo;
    private LevelWidget _levelWidget;
    private ScoringSystem _scoringSystem;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _screenGo.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _gameScreen = _screenGo.AddComponent<GameScreen>();

        _levelRegion = new VisualElement { name = "levelRegion" };
        _uiDoc.rootVisualElement.Add(_levelRegion);

        _widgetGo = new GameObject("LevelWidgetTest");
        _levelWidget = _widgetGo.AddComponent<LevelWidget>();

        var field = typeof(LevelWidget).GetField("_gameScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_levelWidget, _gameScreen);

        _scoringSystem = new ScoringSystem();

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
    public IEnumerator LevelWidget_InitialDisplay_ShowsLevelOne()
    {
        _levelWidget.Initialize(_scoringSystem);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.IsNotNull(valueLabel, "Level value label should exist");
        Assert.AreEqual("1", valueLabel.text);
    }

    [UnityTest]
    public IEnumerator LevelWidget_StatsChanged_UpdatesLevelDisplay()
    {
        _levelWidget.Initialize(_scoringSystem);
        yield return null;

        for (int i = 0; i < 10; i++)
            _scoringSystem.AddLines(1);
        yield return null;

        var valueLabel = FindValueLabel();
        Assert.IsNotNull(valueLabel, "Level value label should exist");
        Assert.AreEqual("2", valueLabel.text);
    }

    [UnityTest]
    public IEnumerator LevelWidget_BuildsUIIntoLevelRegion()
    {
        _levelWidget.Initialize(_scoringSystem);
        yield return null;

        int labelCount = 0;
        _levelRegion.Query<Label>().ForEach(_ => labelCount++);
        Assert.GreaterOrEqual(labelCount, 2, "Expected at least 2 labels (LEVEL header + value) in levelRegion");
    }

    [UnityTest]
    public IEnumerator LevelWidget_HeaderLabel_IsPresent()
    {
        yield return null;

        Label headerLabel = null;
        _levelRegion.Query<Label>().ForEach(l =>
        {
            if (l.text == "LEVEL") headerLabel = l;
        });
        Assert.IsNotNull(headerLabel, "LEVEL header label should be present in levelRegion");
    }

    private Label FindValueLabel()
    {
        Label valueLabel = null;
        _levelRegion.Query<Label>().ForEach(l =>
        {
            if (l.text != "LEVEL") valueLabel = l;
        });
        return valueLabel;
    }
}
