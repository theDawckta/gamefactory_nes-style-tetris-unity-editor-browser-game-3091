using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NextPieceWidgetTests
{
    private GameObject _widgetGo;
    private NextPieceWidget _widget;
    private Sprite _blockSprite;
    private Texture2D _blockTex;
    private TetrominoData _oPiece;
    private TetrominoData _iPiece;

    [SetUp]
    public void SetUp()
    {
        _blockTex = new Texture2D(1, 1);
        _blockTex.SetPixel(0, 0, Color.white);
        _blockTex.Apply();
        _blockSprite = Sprite.Create(_blockTex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);

        _widgetGo = new GameObject("NextPieceWidget");
        _widget = _widgetGo.AddComponent<NextPieceWidget>();
        _widget.BlockSprite = _blockSprite;

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
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_widgetGo);
        Object.Destroy(_oPiece);
        Object.Destroy(_iPiece);
        Object.Destroy(_blockTex);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_Start_Creates16PreviewCells()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _widget.transform)
        {
            if (child.name.StartsWith("PreviewCell_"))
                count++;
        }
        Assert.AreEqual(16, count);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_InitialCells_AreAllTransparent()
    {
        yield return null;
        foreach (Transform child in _widget.transform)
        {
            if (!child.name.StartsWith("PreviewCell_"))
                continue;
            var sr = child.GetComponent<SpriteRenderer>();
            Assert.AreEqual(0f, sr.color.a, 0.001f, $"{child.name} should be transparent");
        }
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_OPiece_ShowsCorrectColor()
    {
        yield return null;
        _widget.DrawPiece(_oPiece);
        yield return null;

        // O-piece colorIndex=2 = yellow: r=240/255, g=240/255, b=0
        int litCount = 0;
        foreach (Transform child in _widget.transform)
        {
            if (!child.name.StartsWith("PreviewCell_"))
                continue;
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr.color.a > 0f)
            {
                Assert.AreEqual(240f / 255f, sr.color.r, 0.01f, $"{child.name} wrong red");
                Assert.AreEqual(240f / 255f, sr.color.g, 0.01f, $"{child.name} wrong green");
                Assert.AreEqual(0f, sr.color.b, 0.01f, $"{child.name} wrong blue");
                litCount++;
            }
        }
        Assert.AreEqual(4, litCount, "O-piece should light exactly 4 cells");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_IPiece_ShowsCorrectColor()
    {
        yield return null;
        _widget.DrawPiece(_iPiece);
        yield return null;

        // I-piece colorIndex=1 = cyan: r=0, g=240/255, b=240/255
        int litCount = 0;
        foreach (Transform child in _widget.transform)
        {
            if (!child.name.StartsWith("PreviewCell_"))
                continue;
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr.color.a > 0f)
            {
                Assert.AreEqual(0f, sr.color.r, 0.01f, $"{child.name} wrong red");
                Assert.AreEqual(240f / 255f, sr.color.g, 0.01f, $"{child.name} wrong green");
                Assert.AreEqual(240f / 255f, sr.color.b, 0.01f, $"{child.name} wrong blue");
                litCount++;
            }
        }
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

        // After switching pieces only 4 cells total should be lit
        int litCount = 0;
        foreach (Transform child in _widget.transform)
        {
            if (!child.name.StartsWith("PreviewCell_"))
                continue;
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr.color.a > 0f)
                litCount++;
        }
        Assert.AreEqual(4, litCount, "Exactly 4 cells should be lit after switching piece");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_DrawPiece_NullPiece_ClearsAllCells()
    {
        yield return null;
        _widget.DrawPiece(_oPiece);
        yield return null;
        _widget.DrawPiece(null);
        yield return null;

        foreach (Transform child in _widget.transform)
        {
            if (!child.name.StartsWith("PreviewCell_"))
                continue;
            var sr = child.GetComponent<SpriteRenderer>();
            Assert.AreEqual(0f, sr.color.a, 0.001f, $"{child.name} should be transparent after null piece");
        }
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_OnNextPieceChanged_UpdatesPreview()
    {
        // Create a fresh widget in the test body so GameplayController is set before Start() runs.
        // The test runner may process a frame between SetUp and the first yield, causing
        // the shared _widget's Start() to fire before the test body can set GameplayController.
        var gcGo = new GameObject("GameplayController");
        var controller = gcGo.AddComponent<GameplayController>();

        var freshGo = new GameObject("FreshNextPieceWidget");
        var freshWidget = freshGo.AddComponent<NextPieceWidget>();
        freshWidget.BlockSprite = _blockSprite;
        freshWidget.GameplayController = controller; // set before Start()

        yield return null; // Start() runs; freshWidget subscribes to OnNextPieceChanged

        // Invoke the backing delegate via reflection (event can only be raised from inside the class)
        var field = typeof(GameplayController).GetField("OnNextPieceChanged",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var del = field?.GetValue(controller) as System.Action<TetrominoData>;
        del?.Invoke(_oPiece);

        int litCount = 0;
        foreach (Transform child in freshGo.transform)
        {
            if (!child.name.StartsWith("PreviewCell_"))
                continue;
            if (child.GetComponent<SpriteRenderer>().color.a > 0f)
                litCount++;
        }
        Assert.AreEqual(4, litCount, "Exactly 4 cells should be lit after OnNextPieceChanged fires");

        Object.Destroy(freshGo);
        Object.Destroy(gcGo);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_CellScale_Is90PercentOfCellSize()
    {
        yield return null;
        var cell = _widget.transform.Find("PreviewCell_0_0");
        Assert.IsNotNull(cell);
        float expected = 24f * 0.9f;
        Assert.AreEqual(expected, cell.localScale.x, 0.001f);
        Assert.AreEqual(expected, cell.localScale.y, 0.001f);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_PreviewCells_HaveBlockSprite()
    {
        yield return null;
        var cell = _widget.transform.Find("PreviewCell_0_0");
        Assert.IsNotNull(cell);
        var sr = cell.GetComponent<SpriteRenderer>();
        Assert.AreEqual(_blockSprite, sr.sprite);
    }
}
