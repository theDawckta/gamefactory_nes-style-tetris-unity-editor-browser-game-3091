using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class ReturnPromptWidgetTests
{
    private GameObject _widgetGo;
    private ReturnPromptWidget _widget;
    private GameObject _screenGo;
    private GameOverScreen _gameOverScreen;
    private VisualElement _returnPromptRegion;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameOverScreen");
        _gameOverScreen = _screenGo.AddComponent<GameOverScreen>();
        _returnPromptRegion = new VisualElement();
        _returnPromptRegion.style.display = DisplayStyle.Flex;
        _gameOverScreen.ReturnPromptRegion = _returnPromptRegion;

        _widgetGo = new GameObject("ReturnPromptWidget");
        _widget = _widgetGo.AddComponent<ReturnPromptWidget>();

        var field = typeof(ReturnPromptWidget).GetField(
            "_gameOverScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _gameOverScreen);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        UnityEngine.Object.Destroy(_widgetGo);
        UnityEngine.Object.Destroy(_screenGo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Activate_CreatesPromptLabelInRegion()
    {
        _widget.Activate();
        yield return null;

        var label = _returnPromptRegion.Q<Label>();
        Assert.IsNotNull(label, "Activate should create a label in returnPromptRegion");
    }

    [UnityTest]
    public IEnumerator Activate_LabelTextIsCorrect()
    {
        _widget.Activate();
        yield return null;

        var label = _returnPromptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual("PRESS ENTER TO CONTINUE", label.text);
    }

    [UnityTest]
    public IEnumerator Activate_SetsIsActiveTrue()
    {
        _widget.Activate();
        yield return null;

        bool isActive = (bool)typeof(ReturnPromptWidget)
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

        bool isActive = (bool)typeof(ReturnPromptWidget)
            .GetField("_isActive", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsFalse(isActive);
    }

    [UnityTest]
    public IEnumerator Activate_LabelIsInitiallyVisible()
    {
        _widget.Activate();
        yield return null;

        var label = _returnPromptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.Flex, label.style.display.value);
    }

    [UnityTest]
    public IEnumerator Blink_LabelHidesAfterVisibleDuration()
    {
        _widget.Activate();
        yield return new WaitForSeconds(0.7f);

        var label = _returnPromptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.None, label.style.display.value,
            "Label should be hidden after 0.6s visible phase");
    }

    [UnityTest]
    public IEnumerator Blink_LabelVisibleAgainAfterFullCycle()
    {
        _widget.Activate();
        yield return new WaitForSeconds(1.1f);

        var label = _returnPromptRegion.Q<Label>();
        Assert.IsNotNull(label);
        Assert.AreEqual(DisplayStyle.Flex, label.style.display.value,
            "Label should be visible again after full blink cycle (1.0s)");
    }

    [UnityTest]
    public IEnumerator Deactivate_StopsBlinkCoroutine()
    {
        _widget.Activate();
        yield return null;

        _widget.Deactivate();
        yield return null;

        var coroutineField = typeof(ReturnPromptWidget)
            .GetField("_blinkCoroutine", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsNull(coroutineField, "Blink coroutine reference should be null after Deactivate");
    }

    [UnityTest]
    public IEnumerator OnReturnPressed_CanBeSubscribedAndUnsubscribed()
    {
        int callCount = 0;
        Action handler = () => callCount++;

        _widget.OnReturnPressed += handler;
        _widget.OnReturnPressed -= handler;

        yield return null;

        Assert.AreEqual(0, callCount, "Handler should not fire after being unsubscribed");
    }
}
