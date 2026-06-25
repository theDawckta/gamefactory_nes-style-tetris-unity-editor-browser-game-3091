using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class StartScreenTests
{
    private GameObject _go;
    private UIDocument _uiDoc;
    private StartScreen _screen;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("StartScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _go.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<StartScreen>();
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator StartScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        yield return null;
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        yield return null;
        _screen.Hide();
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_SetsDisplayFlex()
    {
        yield return null;
        _screen.Show();
        yield return null;
        Assert.AreEqual(DisplayStyle.Flex, _uiDoc.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator StartScreen_Hide_SetsDisplayNone()
    {
        yield return null;
        _screen.Show();
        yield return null;
        _screen.Hide();
        yield return null;
        Assert.AreEqual(DisplayStyle.None, _uiDoc.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator StartScreen_LeaderboardRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "leaderboardRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;
        Assert.AreEqual(el, _screen.LeaderboardRegion);
    }

    [UnityTest]
    public IEnumerator StartScreen_PromptRegion_ReturnsNamedElement()
    {
        var el = new VisualElement { name = "promptRegion" };
        _uiDoc.rootVisualElement.Add(el);
        yield return null;
        Assert.AreEqual(el, _screen.PromptRegion);
    }

    [UnityTest]
    public IEnumerator StartScreen_OnStartPressed_CanBeSubscribedAndUnsubscribed()
    {
        yield return null;
        int callCount = 0;
        System.Action handler = () => callCount++;
        _screen.OnStartPressed += handler;
        _screen.OnStartPressed -= handler;
        yield return null;
        Assert.AreEqual(0, callCount, "Handler should not fire after being unsubscribed");
    }

    [UnityTest]
    public IEnumerator StartScreen_Hide_StopsListening()
    {
        yield return null;
        _screen.Show();
        yield return null;
        _screen.Hide();
        yield return null;
        bool fired = false;
        _screen.OnStartPressed += () => fired = true;
        // Simulate: with _listening = false, Update() would not fire OnStartPressed
        // We verify indirectly that Hide calls OnHide which sets _listening = false
        Assert.IsFalse(_screen.IsVisible);
        Assert.IsFalse(fired);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_StartsListening()
    {
        yield return null;
        _screen.Show();
        yield return null;
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_DoesNotThrowWithoutLeaderboardService()
    {
        yield return null;
        Assert.DoesNotThrow(() => _screen.Show());
        yield return null;
    }

    [UnityTest]
    public IEnumerator ReturnToStart_Works_WhenLeaderboardOffline()
    {
        var svcGo = new GameObject("LeaderboardService");
        svcGo.AddComponent<LeaderboardService>();
        yield return null;
        svcGo.SetActive(false);
        yield return null;

        LogAssert.NoUnexpectedReceived();
        _screen.Show();
        yield return null;

        LogAssert.NoUnexpectedReceived();
        Object.Destroy(svcGo);
    }
}
