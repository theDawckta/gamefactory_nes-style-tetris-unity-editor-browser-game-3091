using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayfieldControllerTests
{
    private PlayfieldController _controller;
    private TetrominoData _piece;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject("PlayfieldControllerTests");
        _controller = go.AddComponent<PlayfieldController>();

        _piece = ScriptableObject.CreateInstance<TetrominoData>();
        _piece.colorIndex = 1;
        _piece.rotationStates = new TetrominoData.RotationState[]
        {
            new TetrominoData.RotationState
            {
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                }
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_controller.gameObject);
        Object.Destroy(_piece);
    }

    [UnityTest]
    public IEnumerator IsValidPosition_ReturnsTrueForEmptyGrid()
    {
        yield return null;
        Assert.IsTrue(_controller.IsValidPosition(_piece, 0, new Vector2Int(0, 0)));
    }

    [UnityTest]
    public IEnumerator IsValidPosition_ReturnsFalseOutOfBoundsLeft()
    {
        yield return null;
        Assert.IsFalse(_controller.IsValidPosition(_piece, 0, new Vector2Int(-1, 0)));
    }

    [UnityTest]
    public IEnumerator IsValidPosition_ReturnsFalseOutOfBoundsRight()
    {
        yield return null;
        // piece is 2 wide; pivot at col 9 puts a cell at col 10 (out of bounds)
        Assert.IsFalse(_controller.IsValidPosition(_piece, 0, new Vector2Int(9, 0)));
    }

    [UnityTest]
    public IEnumerator IsValidPosition_ReturnsFalseOutOfBoundsBottom()
    {
        yield return null;
        Assert.IsFalse(_controller.IsValidPosition(_piece, 0, new Vector2Int(0, -1)));
    }

    [UnityTest]
    public IEnumerator IsValidPosition_ReturnsFalseWhenOverlapsLockedCell()
    {
        _controller.SetCell(0, 0, 2);
        yield return null;
        Assert.IsFalse(_controller.IsValidPosition(_piece, 0, new Vector2Int(0, 0)));
    }

    [UnityTest]
    public IEnumerator LockPiece_WritesColorIndexToAllFourCells()
    {
        yield return null;
        _controller.LockPiece(_piece, 0, new Vector2Int(0, 0));
        Assert.AreEqual(1, _controller.GetCell(0, 0));
        Assert.AreEqual(1, _controller.GetCell(0, 1));
        Assert.AreEqual(1, _controller.GetCell(1, 0));
        Assert.AreEqual(1, _controller.GetCell(1, 1));
    }

    [UnityTest]
    public IEnumerator LockPiece_DoesNotOverwriteOtherCells()
    {
        yield return null;
        _controller.LockPiece(_piece, 0, new Vector2Int(0, 0));
        Assert.AreEqual(0, _controller.GetCell(0, 2));
        Assert.AreEqual(0, _controller.GetCell(2, 0));
    }

    [UnityTest]
    public IEnumerator ClearLines_ReturnsEmptyArrayWhenNoFullRows()
    {
        yield return null;
        int[] cleared = _controller.ClearLines();
        Assert.AreEqual(0, cleared.Length);
    }

    [UnityTest]
    public IEnumerator ClearLines_ClearsOneFullRowAndShiftsDown()
    {
        for (int col = 0; col < 10; col++)
            _controller.SetCell(0, col, 1);
        _controller.SetCell(1, 0, 2);

        yield return null;
        int[] cleared = _controller.ClearLines();

        Assert.AreEqual(1, cleared.Length);
        Assert.AreEqual(0, cleared[0]);
        Assert.AreEqual(2, _controller.GetCell(0, 0));
        Assert.AreEqual(0, _controller.GetCell(1, 0));
    }

    [UnityTest]
    public IEnumerator ClearLines_ClearsTwoFullRowsAndShiftsDown()
    {
        for (int col = 0; col < 10; col++)
        {
            _controller.SetCell(0, col, 1);
            _controller.SetCell(1, col, 2);
        }
        _controller.SetCell(2, 5, 3);

        yield return null;
        int[] cleared = _controller.ClearLines();

        Assert.AreEqual(2, cleared.Length);
        Assert.AreEqual(3, _controller.GetCell(0, 5));
        Assert.AreEqual(0, _controller.GetCell(1, 5));
    }

    [UnityTest]
    public IEnumerator ClearLines_TopRowsAreZeroAfterClear()
    {
        for (int col = 0; col < 10; col++)
            _controller.SetCell(0, col, 1);

        yield return null;
        _controller.ClearLines();

        Assert.AreEqual(0, _controller.GetCell(21, 0));
        Assert.AreEqual(0, _controller.GetCell(20, 0));
    }

    [UnityTest]
    public IEnumerator ClearAll_ResetsAllCellsToZero()
    {
        for (int row = 0; row < 22; row++)
            for (int col = 0; col < 10; col++)
                _controller.SetCell(row, col, row + 1);

        yield return null;
        _controller.ClearAll();

        for (int row = 0; row < 22; row++)
            for (int col = 0; col < 10; col++)
                Assert.AreEqual(0, _controller.GetCell(row, col));
    }

    [UnityTest]
    public IEnumerator GetCell_ReturnsZeroForUntouchedCell()
    {
        yield return null;
        Assert.AreEqual(0, _controller.GetCell(10, 5));
    }

    [UnityTest]
    public IEnumerator SetCell_StoresAndRetrievesValue()
    {
        yield return null;
        _controller.SetCell(3, 4, 7);
        Assert.AreEqual(7, _controller.GetCell(3, 4));
    }
}
