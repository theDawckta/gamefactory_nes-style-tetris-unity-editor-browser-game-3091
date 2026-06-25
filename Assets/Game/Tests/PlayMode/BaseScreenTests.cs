using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class BaseScreenTests
{
    private GameObject _go;
    private UIDocument _uiDocument;
    private BaseScreen _baseScreen;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("BaseScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDocument = _go.AddComponent<UIDocument>();
        _uiDocument.panelSettings = panelSettings;
        _baseScreen = _go.AddComponent<BaseScreen>();
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator BaseScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_baseScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Document_IsCachedInAwake()
    {
        yield return null;
        Assert.IsNotNull(_baseScreen.GetType()
            .GetProperty("Document", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_baseScreen));
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_SetsIsVisibleTrue()
    {
        yield return null;
        _baseScreen.Show();
        yield return null;
        Assert.IsTrue(_baseScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_SetsIsVisibleFalse()
    {
        yield return null;
        _baseScreen.Show();
        yield return null;
        _baseScreen.Hide();
        yield return null;
        Assert.IsFalse(_baseScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_SetsDisplayFlex()
    {
        yield return null;
        _baseScreen.Show();
        yield return null;
        Assert.AreEqual(DisplayStyle.Flex, _uiDocument.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_SetsDisplayNone()
    {
        yield return null;
        _baseScreen.Show();
        yield return null;
        _baseScreen.Hide();
        yield return null;
        Assert.AreEqual(DisplayStyle.None, _uiDocument.rootVisualElement.style.display.value);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Show_FiresOnShow()
    {
        yield return null;
        var subclass = new GameObject("SubclassTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var doc = subclass.AddComponent<UIDocument>();
        doc.panelSettings = panelSettings;
        var tracker = subclass.AddComponent<TrackingScreen>();
        yield return null;
        tracker.Show();
        yield return null;
        Assert.IsTrue(tracker.OnShowCalled);
        Object.Destroy(subclass);
    }

    [UnityTest]
    public IEnumerator BaseScreen_Hide_FiresOnHide()
    {
        yield return null;
        var subclass = new GameObject("SubclassTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var doc = subclass.AddComponent<UIDocument>();
        doc.panelSettings = panelSettings;
        var tracker = subclass.AddComponent<TrackingScreen>();
        yield return null;
        tracker.Show();
        tracker.Hide();
        yield return null;
        Assert.IsTrue(tracker.OnHideCalled);
        Object.Destroy(subclass);
    }
}

public class TrackingScreen : BaseScreen
{
    public bool OnShowCalled;
    public bool OnHideCalled;

    protected override void OnShow() => OnShowCalled = true;
    protected override void OnHide() => OnHideCalled = true;
}
