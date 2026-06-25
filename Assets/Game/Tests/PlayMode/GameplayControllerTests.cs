using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayControllerTests
{
    private GameplayController _controller;
    private PlayfieldController _playfield;
    private PieceController _pieceController;

    [SetUp]
    public void SetUp()
    {
        var pfGo = new GameObject("Playfield");
        _playfield = pfGo.AddComponent<PlayfieldController>();

        var pcGo = new GameObject("PieceController");
        _pieceController = pcGo.AddComponent<PieceController>();
        _pieceController.PlayfieldController = _playfield;

        var gcGo = new GameObject("GameplayController");
        _controller = gcGo.AddComponent<GameplayController>();
        _controller.PlayfieldController = _playfield;
        _controller.PieceController = _pieceController;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_controller.gameObject);
        Object.Destroy(_pieceController.gameObject);
        Object.Destroy(_playfield.gameObject);
    }

    [UnityTest]
    public IEnumerator GameplayController_ScoringProperty_IsNotNull()
    {
        yield return null;
        Assert.IsNotNull(_controller.Scoring);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_ResetsScoringToZero()
    {
        yield return null;
        _controller.StartGame();
        Assert.AreEqual(0, _controller.Scoring.Score);
        Assert.AreEqual(0, _controller.Scoring.Level);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_SpawnsPieceOnPieceController()
    {
        yield return null;
        _controller.StartGame();
        Assert.IsNotNull(_pieceController.ActivePiece);
    }

    [UnityTest]
    public IEnumerator GameplayController_StopGame_FiresOnGameOver()
    {
        yield return null;
        _controller.StartGame();
        bool gameOverFired = false;
        _controller.OnGameOver += () => gameOverFired = true;
        _controller.StopGame();
        Assert.IsTrue(gameOverFired);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_FiresOnNextPieceChanged()
    {
        yield return null;
        TetrominoData receivedPiece = null;
        _controller.OnNextPieceChanged += piece => receivedPiece = piece;
        _controller.StartGame();
        Assert.IsNotNull(receivedPiece);
    }

    [UnityTest]
    public IEnumerator GameplayController_Update_FeedsScoringLevelToPieceController()
    {
        _controller.StartGame();
        yield return null;
        Assert.AreEqual(_controller.Scoring.Level, _pieceController.Level);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_ClearsPlayfield()
    {
        yield return null;
        _playfield.SetCell(0, 0, 1);
        _controller.StartGame();
        Assert.AreEqual(0, _playfield.GetCell(0, 0));
    }
}
