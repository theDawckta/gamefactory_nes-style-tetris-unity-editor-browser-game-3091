using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayfieldRendererTests
{
    private GameObject _rendererGo;
    private PlayfieldRenderer _renderer;
    private PlayfieldController _playfield;
    private PieceController _piece;
    private Sprite _blockSprite;
    private Texture2D _blockTex;
    private TetrominoData _tetrominoData;

    [SetUp]
    public void SetUp()
    {
        _blockTex = new Texture2D(1, 1);
        _blockTex.SetPixel(0, 0, Color.white);
        _blockTex.Apply();
        _blockSprite = Sprite.Create(_blockTex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);

        var pfGo = new GameObject("Playfield");
        _playfield = pfGo.AddComponent<PlayfieldController>();

        var pcGo = new GameObject("PieceController");
        _piece = pcGo.AddComponent<PieceController>();
        _piece.PlayfieldController = _playfield;

        _rendererGo = new GameObject("PlayfieldRenderer");
        _renderer = _rendererGo.AddComponent<PlayfieldRenderer>();
        _renderer.PlayfieldController = _playfield;
        _renderer.PieceController = _piece;
        _renderer.BlockSprite = _blockSprite;

        _tetrominoData = ScriptableObject.CreateInstance<TetrominoData>();
        _tetrominoData.colorIndex = 2; // O-piece yellow
        var state = new TetrominoData.RotationState
        {
            cells = new[]
            {
                new Vector2Int(0, 0), new Vector2Int(1, 0),
                new Vector2Int(0, 1), new Vector2Int(1, 1)
            }
        };
        _tetrominoData.rotationStates = new[] { state, state, state, state };
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_rendererGo);
        Object.Destroy(_piece.gameObject);
        Object.Destroy(_playfield.gameObject);
        Object.Destroy(_tetrominoData);
        Object.Destroy(_blockTex);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Start_Creates200GridCellRenderers()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _renderer.transform)
        {
            if (child.name.StartsWith("Cell_"))
                count++;
        }
        Assert.AreEqual(200, count);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Start_Creates4PieceCellRenderers()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _renderer.transform)
        {
            if (child.name.StartsWith("PieceCell_"))
                count++;
        }
        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Start_Creates4BorderRenderers()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _renderer.transform)
        {
            if (child.name.StartsWith("Border_"))
                count++;
        }
        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_TotalChildCount_Is208()
    {
        yield return null;
        Assert.AreEqual(208, _renderer.transform.childCount);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_EmptyCell_HasAlphaZero()
    {
        yield return null;
        var cellGo = _renderer.transform.Find("Cell_0_0");
        Assert.IsNotNull(cellGo);
        var sr = cellGo.GetComponent<SpriteRenderer>();
        Assert.AreEqual(0f, sr.color.a, 0.001f);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_LockedCell_HasCorrectColor()
    {
        yield return null;
        _playfield.SetCell(0, 0, 1); // colorIndex 1 = cyan
        yield return null;
        var cellGo = _renderer.transform.Find("Cell_0_0");
        var sr = cellGo.GetComponent<SpriteRenderer>();
        Assert.AreEqual(0f,            sr.color.r, 0.01f);
        Assert.AreEqual(240f / 255f,   sr.color.g, 0.01f);
        Assert.AreEqual(240f / 255f,   sr.color.b, 0.01f);
        Assert.AreEqual(1f,            sr.color.a, 0.01f);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_CellScale_Is90PercentOfCellSize()
    {
        yield return null;
        var cellGo = _renderer.transform.Find("Cell_0_0");
        Assert.IsNotNull(cellGo);
        float expected = 24f * 0.9f; // 21.6
        Assert.AreEqual(expected, cellGo.localScale.x, 0.001f);
        Assert.AreEqual(expected, cellGo.localScale.y, 0.001f);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Cell00_PositionedAtBottomLeft()
    {
        yield return null;
        var cellGo = _renderer.transform.Find("Cell_0_0");
        Assert.IsNotNull(cellGo);
        // startX = -(10*24)/2 + 24/2 = -120 + 12 = -108
        // startY = -(20*24)/2 + 24/2 = -240 + 12 = -228
        Assert.AreEqual(-108f, cellGo.localPosition.x, 0.001f);
        Assert.AreEqual(-228f, cellGo.localPosition.y, 0.001f);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_ActivePiece_DisplaysCorrectColor()
    {
        yield return null;
        _piece.SpawnPiece(_tetrominoData); // O-piece colorIndex=2, yellow
        yield return null;
        var pieceCell0 = _renderer.transform.Find("PieceCell_0");
        Assert.IsNotNull(pieceCell0);
        var sr = pieceCell0.GetComponent<SpriteRenderer>();
        Assert.AreEqual(240f / 255f, sr.color.r, 0.01f);
        Assert.AreEqual(240f / 255f, sr.color.g, 0.01f);
        Assert.AreEqual(0f,          sr.color.b, 0.01f);
        Assert.AreEqual(1f,          sr.color.a, 0.01f);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_NoPiece_PieceCellsAreTransparent()
    {
        yield return null;
        // No piece spawned -- all piece renderers should be transparent
        var pieceCell0 = _renderer.transform.Find("PieceCell_0");
        var sr = pieceCell0.GetComponent<SpriteRenderer>();
        Assert.AreEqual(0f, sr.color.a, 0.001f);
    }
}
