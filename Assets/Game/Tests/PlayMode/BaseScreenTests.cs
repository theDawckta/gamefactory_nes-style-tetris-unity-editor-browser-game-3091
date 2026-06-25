using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class BaseScreenTests
{
    private class TestScreen : BaseScreen
    {
        public UIDocument PublicDocument => Document;
        public int OnShowCount { get; private set; }
        public int OnHideCount { get; private set; }
        protected override void OnShow() => OnShowCount++;
        protected override void OnHide() => OnHideCount++;
    }

    private GameObject _go;
    private PanelSettings _panelSettings;

    [SetUp]
    public void SetUp()
    {
        _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _go = new GameObject("BaseScreenTest");
        var doc = _go.AddComponent<UIDocument>();
        doc.panelSettings = _panelSettings;
        _go.AddComponent<TestScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
        Object.Destroy(_panelSettings);
    }

    [UnityTest]
    public IEnumerator BaseScreen_StartsHidden()
    {
        yield return null;
        var screen = _go.GetComponent<TestScreen>();
        Assert.IsFalse(screen.IsVisible, "BaseScreen should start hidden after Awake");
    }

    [UnityTest]
    public IEnumerator BaseScreen_Document_IsCachedInAwake()
    {
        yield return null;
        var screen = _go.GetComponent<TestScreen>();
        var doc = _go.GetComponent<UIDocument>();
        Assert.IsNotNull(screen.PublicDocument, "Document property should be cached in Awake");
        Assert.AreEqual(doc, screen.PublicDocument, "Document should reference the sibling UIDocument");
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        var screen = _go.GetComponent<TestScreen>();
        screen.Show();
        yield return null;
        Assert.IsTrue(screen.IsVisible, "IsVisible should be true after Show()");
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        var screen = _go.GetComponent<TestScreen>();
        screen.Show();
        yield return null;
        screen.Hide();
        yield return null;
        Assert.IsFalse(screen.IsVisible, "IsVisible should be false after Hide()");
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_SetsDisplayFlex()
    {
        yield return null;
        var doc = _go.GetComponent<UIDocument>();
        Assume.That(doc.rootVisualElement, Is.Not.Null, "rootVisualElement is null - UIDocument may not have initialized with PanelSettings");
        var screen = _go.GetComponent<TestScreen>();
        screen.Show();
        yield return null;
        Assert.AreEqual(DisplayStyle.Flex, doc.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_SetsDisplayNone()
    {
        yield return null;
        var doc = _go.GetComponent<UIDocument>();
        Assume.That(doc.rootVisualElement, Is.Not.Null, "rootVisualElement is null - UIDocument may not have initialized with PanelSettings");
        var screen = _go.GetComponent<TestScreen>();
        screen.Show();
        yield return null;
        screen.Hide();
        yield return null;
        Assert.AreEqual(DisplayStyle.None, doc.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_FiresOnShow()
    {
        yield return null;
        var screen = _go.GetComponent<TestScreen>();
        int countBefore = screen.OnShowCount;
        screen.Show();
        yield return null;
        Assert.AreEqual(countBefore + 1, screen.OnShowCount, "Show() should fire OnShow()");
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_FiresOnHide()
    {
        yield return null;
        var screen = _go.GetComponent<TestScreen>();
        int countBefore = screen.OnHideCount;
        screen.Hide();
        yield return null;
        Assert.AreEqual(countBefore + 1, screen.OnHideCount, "Hide() should fire OnHide()");
    }
}
