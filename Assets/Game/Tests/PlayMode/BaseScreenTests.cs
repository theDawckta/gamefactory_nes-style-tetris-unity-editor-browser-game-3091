using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TestTools;

public class BaseScreenTests
{
    private GameObject _go;
    private PanelSettings _panelSettings;
    private TestableBaseScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _go = new GameObject("TestScreen");
        var uiDoc = _go.AddComponent<UIDocument>();
        uiDoc.panelSettings = _panelSettings;
        _screen = _go.AddComponent<TestableBaseScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null) Object.Destroy(_go);
        Object.Destroy(_panelSettings);
    }

    [UnityTest]
    public IEnumerator BaseScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Document_IsCachedInAwake()
    {
        yield return null;
        Assert.IsNotNull(_screen.PublicDocument);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_SetsDisplayFlex()
    {
        yield return null;
        _screen.Show();
        var uiDoc = _go.GetComponent<UIDocument>();
        Assert.AreEqual(DisplayStyle.Flex, uiDoc.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_SetsDisplayNone()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        var uiDoc = _go.GetComponent<UIDocument>();
        Assert.AreEqual(DisplayStyle.None, uiDoc.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_FiresOnShow()
    {
        yield return null;
        _screen.ResetFlags();
        _screen.Show();
        Assert.IsTrue(_screen.OnShowCalled);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_FiresOnHide()
    {
        yield return null;
        _screen.Show();
        _screen.ResetFlags();
        _screen.Hide();
        Assert.IsTrue(_screen.OnHideCalled);
    }
}

public class TestableBaseScreen : BaseScreen
{
    public bool OnShowCalled { get; private set; }
    public bool OnHideCalled { get; private set; }
    public UIDocument PublicDocument => Document;

    public void ResetFlags()
    {
        OnShowCalled = false;
        OnHideCalled = false;
    }

    protected override void OnShow() => OnShowCalled = true;
    protected override void OnHide() => OnHideCalled = true;
}
