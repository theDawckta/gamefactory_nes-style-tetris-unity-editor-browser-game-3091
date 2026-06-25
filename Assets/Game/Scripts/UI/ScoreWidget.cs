using UnityEngine;
using UnityEngine.UIElements;

public class ScoreWidget : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    private ScoringSystem _scoringSystem;
    private Label _scoreValueLabel;

    public void SetScoringSystem(ScoringSystem scoringSystem)
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged -= HandleStatsChanged;
        _scoringSystem = scoringSystem;
        if (isActiveAndEnabled && _scoringSystem != null)
            _scoringSystem.OnStatsChanged += HandleStatsChanged;
    }

    private void Start()
    {
        BuildUI();
    }

    private void OnEnable()
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged += HandleStatsChanged;
    }

    private void OnDisable()
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged -= HandleStatsChanged;
    }

    private void BuildUI()
    {
        var region = _gameScreen?.ScoreRegion;
        if (region == null) return;
        region.Clear();

        var headerLabel = new Label("SCORE");
        headerLabel.style.fontSize = 14;
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerLabel.style.color = new StyleColor(new Color(0.2f, 0.1f, 0f));
        headerLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        region.Add(headerLabel);

        _scoreValueLabel = new Label("0000000");
        _scoreValueLabel.style.fontSize = 18;
        _scoreValueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _scoreValueLabel.style.color = new StyleColor(new Color(0.2f, 0.1f, 0f));
        _scoreValueLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        _scoreValueLabel.style.marginTop = 4;
        region.Add(_scoreValueLabel);
    }

    private void HandleStatsChanged(int score, int level, int totalLines)
    {
        if (_scoreValueLabel != null)
            _scoreValueLabel.text = score.ToString("D7");
    }
}
