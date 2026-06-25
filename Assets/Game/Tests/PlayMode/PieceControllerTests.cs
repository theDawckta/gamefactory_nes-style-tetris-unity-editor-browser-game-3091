using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PieceControllerTests
{
    private PlayfieldController _playfield;
    private PieceController _piece;
    private TetrominoData _data;

    [SetUp]
    public void SetUp()
    {
        var pfGo = new GameObject("Playfield");
        _playfield = pfGo.AddComponent<PlayfieldController>();

        var pcGo = new GameObject("PieceController");
        _piece = pcGo.AddComponent<PieceController>();
        _piece.PlayfieldController = _playfield;

        // O-piece: 2x2 square, same shape for all rotations
        _data = ScriptableObject.CreateInstance<TetrominoData>();
        _data.colorIndex = 1;
        var oState = new TetrominoData.RotationState
        {
            cells = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }
        };
        _data.rotationStates = new[] { oState, oState, oState, oState };
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_playfield.gameObject);
        Object.Destroy(_piece.gameObject);
        Object.Destroy(_data);
    }

    [UnityTest]
    public IEnumerator PieceController_SpawnPiece_SetsPivotAtColumn4Row20()
    {
        _piece.SpawnPiece(_data);
        yield return null;
        Assert.AreEqual(new Vector2Int(4, 20), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_SpawnPiece_SetsRotationToZero()
    {
        _piece.SpawnPiece(_data);
        yield return null;
        Assert.AreEqual(0, _piece.CurrentRotation);
    }

    [UnityTest]
    public IEnumerator PieceController_IsLocked_FalseAfterSpawn()
    {
        _piece.SpawnPiece(_data);
        yield return null;
        Assert.IsFalse(_piece.IsLocked);
    }

    [UnityTest]
    public IEnumerator PieceController_Gravity_PieceMovesDownAfterLevel0Interval()
    {
        _piece.SpawnPiece(_data);
        _piece.Level = 0;
        yield return null;
        _piece.Tick(0.85f); // > 0.8s for level 0
        Assert.AreEqual(new Vector2Int(4, 19), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_Gravity_PieceDoesNotMoveBeforeInterval()
    {
        _piece.SpawnPiece(_data);
        _piece.Level = 0;
        yield return null;
        _piece.Tick(0.5f); // < 0.8s for level 0
        Assert.AreEqual(new Vector2Int(4, 20), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_Gravity_FallsMultipleRowsWhenTimerExceedsMultipleIntervals()
    {
        _piece.SpawnPiece(_data);
        _piece.Level = 0;
        yield return null;
        _piece.Tick(1.7f); // > 2 * 0.8s
        Assert.AreEqual(new Vector2Int(4, 18), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_Gravity_Level9IntervalIs100ms()
    {
        _piece.SpawnPiece(_data);
        _piece.Level = 9;
        yield return null;
        _piece.Tick(0.11f); // > 0.1s for level 9
        Assert.AreEqual(new Vector2Int(4, 19), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_LockDelay_OnPieceLockedFiresAfter500ms()
    {
        // Block the piece from falling below spawn: cells at row 19 col 4 and 5
        _playfield.SetCell(19, 4, 1);
        _playfield.SetCell(19, 5, 1);

        _piece.SpawnPiece(_data);
        _piece.Level = 9; // 0.1s gravity
        yield return null;

        // Ground the piece: gravity fires, TryMoveDown to (4,19) fails, piece is grounded at (4,20)
        _piece.Tick(0.11f); // gravity step fires, grounded; lockTimer accumulates 0.11s

        bool locked = false;
        _piece.OnPieceLocked += () => locked = true;

        // Advance lock delay past 500ms (already have 0.11s, need 0.39s more)
        _piece.Tick(0.40f); // lockTimer = 0.51 >= 0.5 -> fires
        Assert.IsTrue(locked);
    }

    [UnityTest]
    public IEnumerator PieceController_LockDelay_DoesNotLockBefore500ms()
    {
        _playfield.SetCell(19, 4, 1);
        _playfield.SetCell(19, 5, 1);

        _piece.SpawnPiece(_data);
        _piece.Level = 9;
        yield return null;

        _piece.Tick(0.11f); // grounded, lockTimer = 0.11s

        bool locked = false;
        _piece.OnPieceLocked += () => locked = true;

        _piece.Tick(0.37f); // lockTimer = 0.48 < 0.5 -> not locked
        Assert.IsFalse(locked);
    }

    [UnityTest]
    public IEnumerator PieceController_IsLocked_TrueAfterLockDelay()
    {
        _playfield.SetCell(19, 4, 1);
        _playfield.SetCell(19, 5, 1);

        _piece.SpawnPiece(_data);
        _piece.Level = 9;
        yield return null;

        _piece.Tick(0.11f);
        _piece.Tick(0.40f);
        Assert.IsTrue(_piece.IsLocked);
    }

    [UnityTest]
    public IEnumerator PieceController_SpawnPiece_ResetsLockedState()
    {
        _playfield.SetCell(19, 4, 1);
        _playfield.SetCell(19, 5, 1);

        _piece.SpawnPiece(_data);
        _piece.Level = 9;
        yield return null;

        _piece.Tick(0.11f);
        _piece.Tick(0.40f);
        Assert.IsTrue(_piece.IsLocked);

        _playfield.ClearAll();
        _piece.SpawnPiece(_data);
        Assert.IsFalse(_piece.IsLocked);
        Assert.AreEqual(new Vector2Int(4, 20), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_Tick_DoesNothingWhenLocked()
    {
        _playfield.SetCell(19, 4, 1);
        _playfield.SetCell(19, 5, 1);

        _piece.SpawnPiece(_data);
        _piece.Level = 9;
        yield return null;

        _piece.Tick(0.11f);
        _piece.Tick(0.40f);
        Assert.IsTrue(_piece.IsLocked);

        Vector2Int pivotAfterLock = _piece.CurrentPivot;
        _piece.Tick(1.0f); // should be no-op
        Assert.AreEqual(pivotAfterLock, _piece.CurrentPivot);
    }
}
