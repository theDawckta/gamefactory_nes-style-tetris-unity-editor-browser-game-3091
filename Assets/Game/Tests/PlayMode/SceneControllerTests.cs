using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SceneControllerTests
{
    private GameObject _go;
    private SceneController _controller;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("SceneController");
        _controller = _go.AddComponent<SceneController>();
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator SceneController_Awake_ComponentExists()
    {
        yield return null;
        Assert.IsNotNull(_controller);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_WithNoRefs_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _controller.StartGame());
    }

    [UnityTest]
    public IEnumerator SceneController_GoToGameOver_WithNoRefs_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _controller.GoToGameOver());
    }

    [UnityTest]
    public IEnumerator SceneController_GoToStart_WithNoRefs_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _controller.GoToStart());
    }

    [UnityTest]
    public IEnumerator SceneController_NotifyGameStarted_WithNoRefs_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _controller.NotifyGameStarted());
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_FiresOnGameStarted()
    {
        yield return null;
        bool fired = false;
        _controller.OnGameStarted += () => fired = true;
        _controller.StartGame();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator SceneController_NotifyGameStarted_FiresOnGameStarted()
    {
        yield return null;
        bool fired = false;
        _controller.OnGameStarted += () => fired = true;
        _controller.NotifyGameStarted();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator SceneController_HasStartGameMethod()
    {
        yield return null;
        var method = typeof(SceneController).GetMethod("StartGame");
        Assert.IsNotNull(method, "StartGame method must exist for QA navigation contract");
    }

    [UnityTest]
    public IEnumerator SceneController_HasGoToGameOverMethod()
    {
        yield return null;
        var method = typeof(SceneController).GetMethod("GoToGameOver");
        Assert.IsNotNull(method, "GoToGameOver method must exist for QA navigation contract");
    }

    [UnityTest]
    public IEnumerator SceneController_HasGoToStartMethod()
    {
        yield return null;
        var method = typeof(SceneController).GetMethod("GoToStart");
        Assert.IsNotNull(method, "GoToStart method must exist for QA navigation contract");
    }
}
