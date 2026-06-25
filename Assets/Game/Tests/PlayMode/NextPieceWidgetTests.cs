using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class NextPieceWidgetTests
{
    private GameObject _screenGo;
    private UIDocument _uiDoc;
    private GameScreen _gameScreen;
    private VisualElement _nextPieceRegion;
    private GameObject _widgetGo;
    private NextPieceWidget _widget;
    private TetrominoData _oPiece;
    private TetrominoData _iPiece;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _screenGo = new GameObject("GameScreenTest");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _uiDoc = _screenGo.AddComponent<UIDocument>();
        _uiDoc.panelSettings = panelSettings;
        _gameScreen = _screenGo.AddComponent<GameScreen>();

        _nextPieceRegion = new VisualElement { name = "nextPieceRegion" };
        _uiDoc.rootVisualElement.Add(_nextPieceRegion);

        _widgetGo = new GameObject("NextPieceWidget");
        _widget = _widgetGo.AddComponent<NextPieceWidget>();

        var screenField = typeof(NextPieceWidget).GetField("_gameScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        screenField.SetValue(_widget, _gameScreen);

        _oPiece = ScriptableObject.CreateInstance<TetrominoData>();
        _oPiece.colorIndex = 2;
        var oState = new TetrominoData.RotationState
        {
            cells = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }
        };
        _oPiece.rotationStates = new[] { oState, oState, oState, oState };

        _iPiece = ScriptableObject.CreateInstance<TetrominoData>();
        _iPiece.colorIndex = 1;
        var iState = new TetrominoData.RotationState
        {
            cells = new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) }
        };
        _iPiece.rotationStates = new[] { iState, iState, iState, iState };

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(_widgetGo);
        Object.Destroy(_screenGo);
        Object.Destroy(_oPiece);
        Object.Destroy(_iPiece);
        yield return null;
    }

    private int CountLitCells(VisualElement region)
    {
        int count = 0;
        region.Query<VisualElement>().ForEach(el =>
        {
            if (el.name != null && el.name.StartsWith("PreviewCell_") && el.style.backgroundColor.value.a > 0f)
                count++;
        });
        return count;
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_Start_Creates16PreviewCellsInUIDocument()
    {
        yield return null;
        int count = 0;
        _nextPieceRegion.Query<VisualElement>().ForEach(el =>
        {
            if (el.name != null && el.name.StartsWith("PreviewCell_"))
                count++;
        });
        Assert.AreEqual(16, count);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_Start_HasNoWorldSpaceChildObjects()
    {
        yield return null;
        Assert.AreEqual(0, _widgetGo.transform.childCount, "NextPieceWidget must not create world-space GameObjects");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_InitialCells_AreAllTransparent()
    {
        yield return null;
        _nextPieceRegion.Query<VisualElement>().ForEach(el =>
        {
            if (el.name != null && el.name.StartsWith("PreviewCell_"))
                Assert.AreEqual(0f, el.style.backgroundColor.value.a, 0.001f, $"{el.name} should be transparent initially");
        });
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_OPiece_ShowsCorrectColor()
    {
        yield return null;
        _widget.DrawPiece(_oPiece);
        yield return null;

        int litCount = 0;
        _nextPieceRegion.Query<VisualElement>().ForEach(el =>
        {
            if (el.name == null || !el.name.StartsWith("PreviewCell_")) return;
            var bg = el.style.backgroundColor.value;
            if (bg.a > 0f)
            {
                // O-piece colorIndex=2 = yellow: r=240/255, g=240/255, b=0
                Assert.AreEqual(240f / 255f, bg.r, 0.01f, $"{el.name} wrong red");
                Assert.AreEqual(240f / 255f, bg.g, 0.01f, $"{el.name} wrong green");
                Assert.AreEqual(0f, bg.b, 0.01f, $"{el.name} wrong blue");
                litCount++;
            }
        });
        Assert.AreEqual(4, litCount, "O-piece should light exactly 4 cells");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_IPiece_ShowsCorrectColor()
    {
        yield return null;
        _widget.DrawPiece(_iPiece);
        yield return null;

        int litCount = 0;
        _nextPieceRegion.Query<VisualElement>().ForEach(el =>
        {
            if (el.name == null || !el.name.StartsWith("PreviewCell_")) return;
            var bg = el.style.backgroundColor.value;
            if (bg.a > 0f)
            {
                // I-piece colorIndex=1 = cyan: r=0, g=240/255, b=240/255
                Assert.AreEqual(0f, bg.r, 0.01f, $"{el.name} wrong red");
                Assert.AreEqual(240f / 255f, bg.g, 0.01f, $"{el.name} wrong green");
                Assert.AreEqual(240f / 255f, bg.b, 0.01f, $"{el.name} wrong blue");
                litCount++;
            }
        });
        Assert.AreEqual(4, litCount, "I-piece should light exactly 4 cells");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_ClearsOldCellsBeforeDrawingNew()
    {
        yield return null;
        _widget.DrawPiece(_oPiece);
        yield return null;
        _widget.DrawPiece(_iPiece);
        yield return null;

        Assert.AreEqual(4, CountLitCells(_nextPieceRegion), "Exactly 4 cells should be lit after switching piece");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_NullPiece_ClearsAllCells()
    {
        yield return null;
        _widget.DrawPiece(_oPiece);
        yield return null;
        _widget.DrawPiece(null);
        yield return null;

        _nextPieceRegion.Query<VisualElement>().ForEach(el =>
        {
            if (el.name != null && el.name.StartsWith("PreviewCell_"))
                Assert.AreEqual(0f, el.style.backgroundColor.value.a, 0.001f, $"{el.name} should be transparent after null piece");
        });
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_OnNextPieceChanged_UpdatesPreview()
    {
        var gcGo = new GameObject("GameplayController");
        var controller = gcGo.AddComponent<GameplayController>();

        var freshScreenGo = new GameObject("GameScreenFresh");
        var freshPanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var freshUiDoc = freshScreenGo.AddComponent<UIDocument>();
        freshUiDoc.panelSettings = freshPanelSettings;
        var freshScreen = freshScreenGo.AddComponent<GameScreen>();
        var freshRegion = new VisualElement { name = "nextPieceRegion" };
        freshUiDoc.rootVisualElement.Add(freshRegion);

        var freshGo = new GameObject("FreshNextPieceWidget");
        var freshWidget = freshGo.AddComponent<NextPieceWidget>();
        freshWidget.GameplayController = controller;

        var screenField = typeof(NextPieceWidget).GetField("_gameScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        screenField.SetValue(freshWidget, freshScreen);

        yield return null;

        var eventField = typeof(GameplayController).GetField("OnNextPieceChanged",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var del = eventField?.GetValue(controller) as System.Action<TetrominoData>;
        del?.Invoke(_oPiece);

        Assert.AreEqual(4, CountLitCells(freshRegion), "Exactly 4 cells should be lit after OnNextPieceChanged fires");

        Object.Destroy(freshGo);
        Object.Destroy(gcGo);
        Object.Destroy(freshScreenGo);
    }
}
