using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class StartScreenTests
{
    private GameObject _go;
    private UIDocument _uiDocument;
    private StartScreen _startScreen;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("StartScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDocument = _go.AddComponent<UIDocument>();
        _uiDocument.panelSettings = panelSettings;
        _startScreen = _go.AddComponent<StartScreen>();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(_go);
        yield return null;
    }

    [UnityTest]
    public IEnumerator StartScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        _startScreen.Show();
        yield return null;
        Assert.IsTrue(_startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        _startScreen.Show();
        yield return null;
        _startScreen.Hide();
        yield return null;
        Assert.IsFalse(_startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_SetsDisplayFlex()
    {
        yield return null;
        _startScreen.Show();
        yield return null;
        Assert.AreEqual(DisplayStyle.Flex, _uiDocument.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator StartScreen_Hide_SetsDisplayNone()
    {
        yield return null;
        _startScreen.Show();
        yield return null;
        _startScreen.Hide();
        yield return null;
        Assert.AreEqual(DisplayStyle.None, _uiDocument.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator StartScreen_OnStartPressed_FiresWhenInvoked()
    {
        yield return null;
        _startScreen.Show();
        yield return null;

        bool fired = false;
        _startScreen.OnStartPressed += () => fired = true;

        // Directly fire event via reflection to test the subscription mechanism
        var method = typeof(StartScreen).GetMethod("Update",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        // Verify subscription - manually invoke the event via the public API contract
        // by invoking the delegate list through reflection
        var field = typeof(StartScreen).GetField("OnStartPressed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var del = field?.GetValue(_startScreen) as System.Action;
        del?.Invoke();

        yield return null;
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator StartScreen_Hide_StopsListening()
    {
        yield return null;
        _startScreen.Show();
        yield return null;
        _startScreen.Hide();
        yield return null;

        bool listeningField = (bool)typeof(StartScreen)
            .GetField("_listening", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_startScreen);
        Assert.IsFalse(listeningField);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_StartsListening()
    {
        yield return null;
        _startScreen.Show();
        yield return null;

        bool listeningField = (bool)typeof(StartScreen)
            .GetField("_listening", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_startScreen);
        Assert.IsTrue(listeningField);
    }

    [UnityTest]
    public IEnumerator StartScreen_Show_DoesNotThrowWithoutLeaderboardService()
    {
        yield return null;
        Assert.DoesNotThrow(() => _startScreen.Show());
        yield return null;
    }
}
