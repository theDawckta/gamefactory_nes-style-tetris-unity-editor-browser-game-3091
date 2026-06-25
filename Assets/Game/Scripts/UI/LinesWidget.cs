using UnityEngine;
using UnityEngine.UIElements;

public class LinesWidget : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    private ScoringSystem _scoring;
    private Label _linesValueLabel;

    public void Initialize(ScoringSystem scoring)
    {
        Unsubscribe();
        _scoring = scoring;
        BuildUI();
        if (gameObject.activeInHierarchy)
            Subscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_scoring != null)
        {
            _scoring.OnStatsChanged -= HandleStatsChanged;
            _scoring.OnStatsChanged += HandleStatsChanged;
        }
    }

    private void Unsubscribe()
    {
        if (_scoring != null)
            _scoring.OnStatsChanged -= HandleStatsChanged;
    }

    private void BuildUI()
    {
        var region = _gameScreen != null ? _gameScreen.LinesRegion : null;
        if (region == null) return;
        region.Clear();

        var headerLabel = new Label("LINES");
        headerLabel.style.fontSize = 20;
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerLabel.style.color = new StyleColor(Color.white);
        headerLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        region.Add(headerLabel);

        _linesValueLabel = new Label("0");
        _linesValueLabel.style.fontSize = 28;
        _linesValueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _linesValueLabel.style.color = new StyleColor(Color.white);
        _linesValueLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        _linesValueLabel.style.marginTop = 4;
        region.Add(_linesValueLabel);
    }

    private void HandleStatsChanged(int score, int level, int totalLines)
    {
        if (_linesValueLabel != null)
            _linesValueLabel.text = totalLines.ToString();
    }
}
