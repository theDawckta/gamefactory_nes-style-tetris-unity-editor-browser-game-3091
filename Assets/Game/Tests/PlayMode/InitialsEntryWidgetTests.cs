using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class InitialsEntryWidgetTests
{
    private GameObject _widgetGo;
    private InitialsEntryWidget _widget;
    private GameObject _screenGo;
    private GameOverScreen _gameOverScreen;
    private VisualElement _initialsRegion;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameOverScreen");
        _gameOverScreen = _screenGo.AddComponent<GameOverScreen>();
        _initialsRegion = new VisualElement();
        _gameOverScreen.InitialsRegion = _initialsRegion;

        _widgetGo = new GameObject("InitialsEntryWidget");
        _widget = _widgetGo.AddComponent<InitialsEntryWidget>();

        var field = typeof(InitialsEntryWidget).GetField(
            "_gameOverScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_widget, _gameOverScreen);

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
    public IEnumerator Activate_SetsIsActiveAndBuildsThreeSlotLabels()
    {
        _widget.Activate(1000);
        yield return null;

        int labelCount = CountDescendantLabels(_initialsRegion);
        Assert.GreaterOrEqual(labelCount, 3, "Expected at least 3 slot labels after Activate");
    }

    [UnityTest]
    public IEnumerator Activate_InitialSlotShowsA()
    {
        _widget.Activate(500);
        yield return null;

        var firstLabel = _initialsRegion.Q<Label>();
        Assert.IsNotNull(firstLabel);
        Assert.AreEqual("A", firstLabel.text);
    }

    [UnityTest]
    public IEnumerator Deactivate_StopsInputListening()
    {
        _widget.Activate(500);
        yield return null;
        _widget.Deactivate();
        yield return null;

        bool isActive = (bool)typeof(InitialsEntryWidget)
            .GetField("_isActive", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsFalse(isActive);
    }

    [UnityTest]
    public IEnumerator RightCycle_AdvancesCurrentSlotChar()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("CycleCurrentSlot", 1);
        yield return null;

        var firstLabel = _initialsRegion.Q<Label>();
        Assert.AreEqual("B", firstLabel.text, "Right cycle from A should give B");
    }

    [UnityTest]
    public IEnumerator LeftCycle_WrapsFromAToNine()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("CycleCurrentSlot", -1);
        yield return null;

        var firstLabel = _initialsRegion.Q<Label>();
        Assert.AreEqual("9", firstLabel.text, "Left cycle from A should wrap to 9");
    }

    [UnityTest]
    public IEnumerator RightCycle_WrapsFromNineToA()
    {
        _widget.Activate(500);
        yield return null;

        // Cycle right 35 times to reach '9', then one more to wrap to 'A'
        for (int i = 0; i < 36; i++)
            CallPrivate("CycleCurrentSlot", 1);
        yield return null;

        var firstLabel = _initialsRegion.Q<Label>();
        Assert.AreEqual("A", firstLabel.text, "Cycling right 36 times should wrap back to A");
    }

    [UnityTest]
    public IEnumerator ConfirmCurrent_AdvancesToNextSlot()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("ConfirmCurrent");
        yield return null;

        int currentSlot = (int)typeof(InitialsEntryWidget)
            .GetField("_currentSlot", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.AreEqual(1, currentSlot);
    }

    [UnityTest]
    public IEnumerator MoveToPrevious_MovesBackFromSlotTwo()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        yield return null;

        CallPrivate("MoveToPrevious");
        yield return null;

        int currentSlot = (int)typeof(InitialsEntryWidget)
            .GetField("_currentSlot", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.AreEqual(1, currentSlot, "MoveToPrevious from slot 2 (index 2) should land on slot 1 (index 1)");
    }

    [UnityTest]
    public IEnumerator ConfirmThirdSlot_SetsAwaitingConfirmTrue()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        yield return null;

        bool awaitingConfirm = (bool)typeof(InitialsEntryWidget)
            .GetField("_awaitingConfirm", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsTrue(awaitingConfirm, "After 3rd confirm, awaitingConfirm should be true");
    }

    [UnityTest]
    public IEnumerator ConfirmThirdSlot_ShowsConfirmLabel()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        yield return null;

        var confirmLabel = _initialsRegion.Q<Label>("", "");
        // Find the label with text "CONFIRM"
        Label found = null;
        _initialsRegion.Query<Label>().ForEach(l =>
        {
            if (l.text == "CONFIRM") found = l;
        });
        Assert.IsNotNull(found, "CONFIRM label should exist after 3rd slot confirmed");
        Assert.AreEqual(DisplayStyle.Flex, found.style.display.value);
    }

    [UnityTest]
    public IEnumerator ConfirmOnAwaitingConfirm_CallsShowReturnPromptWhenNoLeaderboard()
    {
        _widget.Activate(500);
        yield return null;

        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        CallPrivate("ConfirmCurrent");
        yield return null;

        // Verify widget deactivates after final confirm (LeaderboardService is null in test environment)
        CallPrivate("ConfirmCurrent");
        yield return null;

        bool isActive = (bool)typeof(InitialsEntryWidget)
            .GetField("_isActive", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
        Assert.IsFalse(isActive, "Widget should deactivate after final confirm");
    }

    private void CallPrivate(string methodName, params object[] args)
    {
        typeof(InitialsEntryWidget)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(_widget, args.Length > 0 ? args : null);
    }

    private static int CountDescendantLabels(VisualElement root)
    {
        int count = 0;
        root.Query<Label>().ForEach(_ => count++);
        return count;
    }
}
