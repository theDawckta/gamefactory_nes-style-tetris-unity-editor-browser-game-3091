using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class GameOverScreenTests
{
    private GameObject _go;
    private UIDocument _uiDoc;
    private GameOverScreen _screen;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("GameOverScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _go.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<GameOverScreen>();
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        yield return null;
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        yield return null;
        _screen.Hide();
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_MakesScreenVisible()
    {
        yield return null;
        _screen.ShowWithScore(1234);
        yield return null;
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_PopulatesFinalScoreRegion()
    {
        var finalScoreEl = new VisualElement { name = "finalScoreRegion" };
        _uiDoc.rootVisualElement.Add(finalScoreEl);
        yield return null;

        _screen.ShowWithScore(9999);
        yield return null;

        var label = finalScoreEl.Q<Label>();
        Assert.IsNotNull(label, "finalScoreRegion should contain a score label");
        Assert.AreEqual("9999", label.text);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_NoLeaderboardService_ShowsReturnPrompt()
    {
        var initialsEl = new VisualElement { name = "initialsRegion" };
        var returnEl = new VisualElement { name = "returnPromptRegion" };
        _uiDoc.rootVisualElement.Add(initialsEl);
        _uiDoc.rootVisualElement.Add(returnEl);
        yield return null;

        _screen.ShowWithScore(500);
        yield return null;

        Assert.AreEqual(DisplayStyle.None, initialsEl.style.display.value);
        Assert.AreEqual(DisplayStyle.Flex, returnEl.style.display.value);
    }

    [UnityTest]
    public IEnumerator InitialsRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "initialsRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;

        _screen.Show();
        yield return null;

        Assert.AreEqual(el, _screen.InitialsRegion);
    }

    [UnityTest]
    public IEnumerator ReturnPromptRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "returnPromptRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;

        _screen.Show();
        yield return null;

        Assert.AreEqual(el, _screen.ReturnPromptRegion);
    }

    [UnityTest]
    public IEnumerator ShowReturnPrompt_HidesInitialsShowsReturnPrompt()
    {
        var initialsEl = new VisualElement { name = "initialsRegion" };
        var returnEl = new VisualElement { name = "returnPromptRegion" };
        _uiDoc.rootVisualElement.Add(initialsEl);
        _uiDoc.rootVisualElement.Add(returnEl);
        yield return null;

        _screen.Show();
        yield return null;

        initialsEl.style.display = DisplayStyle.Flex;
        returnEl.style.display = DisplayStyle.None;

        _screen.ShowReturnPrompt();
        yield return null;

        Assert.AreEqual(DisplayStyle.None, initialsEl.style.display.value);
        Assert.AreEqual(DisplayStyle.Flex, returnEl.style.display.value);
    }
}
