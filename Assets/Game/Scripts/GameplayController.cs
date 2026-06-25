using System;
using OneTimeGames.CoreSystems;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    private const string StatePlaying = "Playing";
    private const string StateLineClear = "LineClear";
    private const string StateGameOver = "GameOver";
    private const float LineClearDuration = 0.5f;

    public PlayfieldController PlayfieldController;
    public PieceController PieceController;
    public PlayfieldRenderer PlayfieldRenderer;

    public event Action OnGameOver;
    public event Action<TetrominoData> OnNextPieceChanged;

    public ScoringSystem Scoring { get; private set; }

    private GameStateMachine _stateMachine;
    private TetrominoData _nextPiece;
    private float _lineClearTimer;

    private void Awake()
    {
        Scoring = new ScoringSystem();

        _stateMachine = new GameStateMachine();
        _stateMachine.RegisterState(StatePlaying, null, TickPlaying, null);
        _stateMachine.RegisterState(StateLineClear, EnterLineClear, TickLineClear, null);
        _stateMachine.RegisterState(StateGameOver, EnterGameOver, null, null);
    }

    private void Update()
    {
        if (PieceController != null)
            PieceController.Level = Scoring.Level;
        _stateMachine.Tick();
    }

    public void StartGame()
    {
        PlayfieldController.ClearAll();
        Scoring.Reset();

        PieceController.OnPieceLocked -= HandlePieceLocked;
        PieceController.OnPieceLocked += HandlePieceLocked;

        TetrominoData firstPiece = TetrominoDefinitions.Random();
        _nextPiece = TetrominoDefinitions.Random();
        OnNextPieceChanged?.Invoke(_nextPiece);

        PieceController.SpawnPiece(firstPiece);
        _stateMachine.TransitionTo(StatePlaying);
    }

    public void StopGame()
    {
        _stateMachine.TransitionTo(StateGameOver);
    }

    private void HandlePieceLocked()
    {
        if (!_stateMachine.IsInState(StatePlaying))
            return;

        int[] clearedRows = PlayfieldController.ClearLines();
        if (clearedRows.Length > 0)
        {
            Scoring.AddLines(clearedRows.Length);
            _stateMachine.TransitionTo(StateLineClear);
        }
        else
        {
            TrySpawnNextPiece();
        }
    }

    private void TrySpawnNextPiece()
    {
        TetrominoData toSpawn = _nextPiece;
        _nextPiece = TetrominoDefinitions.Random();
        OnNextPieceChanged?.Invoke(_nextPiece);

        Vector2Int spawnPivot = new Vector2Int(4, 20);
        if (!PlayfieldController.IsValidPosition(toSpawn, 0, spawnPivot))
        {
            _stateMachine.TransitionTo(StateGameOver);
            return;
        }

        PieceController.SpawnPiece(toSpawn);
    }

    private void TickPlaying()
    {
        PieceController.Tick(Time.deltaTime);
    }

    private void EnterLineClear()
    {
        _lineClearTimer = 0f;
    }

    private void TickLineClear()
    {
        _lineClearTimer += Time.deltaTime;
        if (_lineClearTimer >= LineClearDuration)
        {
            TrySpawnNextPiece();
            if (!_stateMachine.IsInState(StateGameOver))
                _stateMachine.TransitionTo(StatePlaying);
        }
    }

    private void EnterGameOver()
    {
        PieceController.OnPieceLocked -= HandlePieceLocked;
        OnGameOver?.Invoke();
    }
}
