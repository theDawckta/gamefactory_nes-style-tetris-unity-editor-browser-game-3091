using UnityEngine;
using UnityEngine.UIElements;

public class FinalScoreWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    private Label _scoreValueLabel;

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        var region = _gameOverScreen != null ? _gameOverScreen.FinalScoreRegion : null;
        if (region == null) return;
        region.Clear();

        var headerLabel = new Label("SCORE");
        headerLabel.style.fontSize = 24;
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerLabel.style.color = new StyleColor(Color.white);
        headerLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        region.Add(headerLabel);

        _scoreValueLabel = new Label("0000000");
        _scoreValueLabel.style.fontSize = 32;
        _scoreValueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _scoreValueLabel.style.color = new StyleColor(Color.yellow);
        _scoreValueLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        _scoreValueLabel.style.marginTop = 8;
        region.Add(_scoreValueLabel);
    }

    public void SetScore(int score)
    {
        if (_scoreValueLabel != null)
            _scoreValueLabel.text = score.ToString("D7");
    }
}
