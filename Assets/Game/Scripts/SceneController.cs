using System;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public event Action OnGameStarted;

    [SerializeField] private StartScreen _startScreen;
    [SerializeField] private GameScreen _gameScreen;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private GameplayController _gameplayController;
    [SerializeField] private StartPromptWidget _startPromptWidget;
    [SerializeField] private FinalScoreWidget _finalScoreWidget;
    [SerializeField] private InitialsEntryWidget _initialsEntryWidget;
    [SerializeField] private ReturnPromptWidget _returnPromptWidget;

    private ScoreWidget _scoreWidget;
    private LevelWidget _levelWidget;
    private LinesWidget _linesWidget;

    private void Start()
    {
        _scoreWidget = UnityEngine.Object.FindAnyObjectByType<ScoreWidget>();
        _levelWidget = UnityEngine.Object.FindAnyObjectByType<LevelWidget>();
        _linesWidget = UnityEngine.Object.FindAnyObjectByType<LinesWidget>();

        if (_startScreen != null)
            _startScreen.OnStartPressed += HandleStartPressed;
        if (_gameplayController != null)
            _gameplayController.OnGameOver += HandleGameOver;
        if (_returnPromptWidget != null)
            _returnPromptWidget.OnReturnPressed += GoToStart;

        GoToStart();
    }

    private void HandleStartPressed() => StartGame();
    private void HandleGameOver() => GoToGameOver();

    public void StartGame()
    {
        _startScreen?.Hide();
        _startPromptWidget?.Deactivate();

        if (_gameplayController != null)
        {
            _scoreWidget?.SetScoringSystem(_gameplayController.Scoring);
            _levelWidget?.Initialize(_gameplayController.Scoring);
            _linesWidget?.Initialize(_gameplayController.Scoring);
        }

        _gameplayController?.StartGame();
        _gameScreen?.Show();
        OnGameStarted?.Invoke();
    }

    public void GoToGameOver()
    {
        _gameScreen?.Hide();
        int score = _gameplayController != null ? _gameplayController.Scoring.Score : 0;
        _gameOverScreen?.ShowWithScore(score);
        _finalScoreWidget?.SetScore(score);

        bool isTopFive = LeaderboardService.Instance != null && LeaderboardService.Instance.IsTopFive(score);
        if (isTopFive)
            _initialsEntryWidget?.Activate(score);
        else
            _returnPromptWidget?.Activate();
    }

    public void GoToStart()
    {
        _returnPromptWidget?.Deactivate();
        _initialsEntryWidget?.Deactivate();
        _gameOverScreen?.Hide();
        _gameScreen?.Hide();
        _startScreen?.Show();
        _startPromptWidget?.Activate();
    }

    public void NotifyGameStarted()
    {
        OnGameStarted?.Invoke();
    }
}
