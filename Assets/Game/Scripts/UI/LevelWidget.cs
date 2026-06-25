using UnityEngine;
using UnityEngine.UIElements;

public class LevelWidget : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    private ScoringSystem _scoringSystem;
    private Label _levelValueLabel;

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
        var region = _gameScreen != null ? _gameScreen.LevelRegion : null;
        if (region == null) return;
        region.Clear();

        var headerLabel = new Label("LEVEL");
        headerLabel.style.fontSize = 24;
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerLabel.style.color = new StyleColor(Color.white);
        headerLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        region.Add(headerLabel);

        _levelValueLabel = new Label("1");
        _levelValueLabel.style.fontSize = 32;
        _levelValueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _levelValueLabel.style.color = new StyleColor(Color.yellow);
        _levelValueLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        _levelValueLabel.style.marginTop = 8;
        region.Add(_levelValueLabel);
    }

    public void Initialize(ScoringSystem scoringSystem)
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged -= HandleStatsChanged;

        _scoringSystem = scoringSystem;

        if (enabled && gameObject.activeInHierarchy)
            _scoringSystem.OnStatsChanged += HandleStatsChanged;

        UpdateLabel(_scoringSystem.Level);
    }

    private void HandleStatsChanged(int score, int level, int totalLines)
    {
        UpdateLabel(level);
    }

    private void UpdateLabel(int level)
    {
        if (_levelValueLabel != null)
            _levelValueLabel.text = (level + 1).ToString();
    }
}
