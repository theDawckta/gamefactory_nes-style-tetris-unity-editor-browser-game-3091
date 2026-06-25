using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class GameScreenTests
{
    private GameObject _go;
    private UIDocument _uiDoc;
    private GameScreen _screen;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("GameScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _go.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<GameScreen>();
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator GameScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        yield return null;
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        yield return null;
        _screen.Hide();
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameScreen_ScoreRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "scoreRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;
        Assert.AreEqual(el, _screen.ScoreRegion);
    }

    [UnityTest]
    public IEnumerator GameScreen_LevelRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "levelRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;
        Assert.AreEqual(el, _screen.LevelRegion);
    }

    [UnityTest]
    public IEnumerator GameScreen_LinesRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "linesRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;
        Assert.AreEqual(el, _screen.LinesRegion);
    }

    [UnityTest]
    public IEnumerator GameScreen_NextPieceRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "nextPieceRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;
        Assert.AreEqual(el, _screen.NextPieceRegion);
    }
}
