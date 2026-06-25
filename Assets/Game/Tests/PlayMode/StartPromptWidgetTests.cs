using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class StartPromptWidgetTests
{
    private GameObject _widgetGo;
    private StartPromptWidget _widget;
    private GameObject _screenGo;
    private StartScreen _startScreen;
    private VisualElement _promptRegion;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("StartScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var uiDoc = _screenGo.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        _startScreen = _screenGo.AddComponent<StartScreen>();

        yield return null;

        _promptRegion = new VisualElement { name = "promptRegion" };
        uiDoc.rootVisualElement.Add(_promptRegion);

        _widgetGo = new GameObject("StartPromptWidget");
        _widget = _widgetGo.AddComponent<StartPromptWidget>();

        var field = typeof(StartPromptWidget).GetField(
            "_startScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _startScreen);

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
    public IEnumerator Activate_CreatesPromptLabelInRegion()
    {
        _widget.Activate();
        yield return null;

        var label = _promptRegion.Q<Label>();
        Assert.IsNotNull(label, "Activate should create a label in promptRegion");
    }

    [UnityTest]
    public IEnumerator Activate_LabelTextIsCorrect()
    {
        _widget.Activate();
        yield return null;

        var label = _promptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual("PRESS ENTER TO START", label.text);
    }

    [UnityTest]
    public IEnumerator Activate_SetsIsActiveTrue()
    {
        _widget.Activate();
        yield return null;

        bool isActive = (bool)typeof(StartPromptWidget)
            .GetField("_isActive", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsTrue(isActive);
    }

    [UnityTest]
    public IEnumerator Deactivate_SetsIsActiveFalse()
    {
        _widget.Activate();
        yield return null;

        _widget.Deactivate();
        yield return null;

        bool isActive = (bool)typeof(StartPromptWidget)
            .GetField("_isActive", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsFalse(isActive);
    }

    [UnityTest]
    public IEnumerator Activate_LabelIsInitiallyVisible()
    {
        _widget.Activate();
        yield return null;

        var label = _promptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.Flex, label.style.display.value);
    }

    [UnityTest]
    public IEnumerator Blink_LabelHidesAfterVisibleDuration()
    {
        _widget.Activate();
        yield return new WaitForSeconds(0.7f);

        var label = _promptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.None, label.style.display.value,
            "Label should be hidden after 0.6s visible phase");
    }

    [UnityTest]
    public IEnumerator Blink_LabelVisibleAgainAfterFullCycle()
    {
        _widget.Activate();
        yield return new WaitForSeconds(1.1f);

        var label = _promptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.Flex, label.style.display.value,
            "Label should be visible again after full blink cycle (1.0s)");
    }

    [UnityTest]
    public IEnumerator Deactivate_HidesLabel()
    {
        _widget.Activate();
        yield return null;

        _widget.Deactivate();
        yield return null;

        var label = _promptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.None, label.style.display.value,
            "Label should be hidden after Deactivate");
    }
}
