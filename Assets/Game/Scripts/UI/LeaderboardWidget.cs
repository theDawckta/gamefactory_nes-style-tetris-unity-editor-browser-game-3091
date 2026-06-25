using UnityEngine;
using UnityEngine.UIElements;

public class LeaderboardWidget : MonoBehaviour
{
    [SerializeField] private StartScreen _startScreen;

    private const int EntryCount = 5;
    private Label _loadingLabel;
    private VisualElement[] _rows;
    private Label[] _initialsLabels;
    private Label[] _scoreLabels;

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        var region = _startScreen?.LeaderboardRegion;
        if (region == null) return;
        region.Clear();

        var headerLabel = new Label("TOP 5");
        headerLabel.style.fontSize = 20;
        headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerLabel.style.color = new StyleColor(new Color(0.2f, 0.1f, 0f));
        headerLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        headerLabel.style.marginBottom = 8;
        region.Add(headerLabel);

        _loadingLabel = new Label("LOADING...");
        _loadingLabel.name = "loadingLabel";
        _loadingLabel.style.color = new StyleColor(new Color(0.4f, 0.32f, 0f));
        _loadingLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        _loadingLabel.style.fontSize = 18;
        region.Add(_loadingLabel);

        _rows = new VisualElement[EntryCount];
        _initialsLabels = new Label[EntryCount];
        _scoreLabels = new Label[EntryCount];

        for (int i = 0; i < EntryCount; i++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.marginTop = 4;
            row.style.paddingLeft = 16;
            row.style.paddingRight = 16;
            row.style.display = DisplayStyle.None;

            var rankLabel = new Label((i + 1).ToString());
            rankLabel.style.width = 24;
            rankLabel.style.color = new StyleColor(new Color(0.2f, 0.1f, 0f));
            rankLabel.style.fontSize = 16;
            rankLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            var initialsLabel = new Label("---");
            initialsLabel.style.flexGrow = 1;
            initialsLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            initialsLabel.style.color = new StyleColor(new Color(0.2f, 0.1f, 0f));
            initialsLabel.style.fontSize = 16;

            var scoreLabel = new Label("0");
            scoreLabel.style.width = 90;
            scoreLabel.style.unityTextAlign = TextAnchor.UpperRight;
            scoreLabel.style.color = new StyleColor(new Color(0.2f, 0.1f, 0f));
            scoreLabel.style.fontSize = 16;

            row.Add(rankLabel);
            row.Add(initialsLabel);
            row.Add(scoreLabel);
            region.Add(row);

            _rows[i] = row;
            _initialsLabels[i] = initialsLabel;
            _scoreLabels[i] = scoreLabel;
        }

        SetLoading();
    }

    public void SetLoading()
    {
        if (_loadingLabel != null)
            _loadingLabel.style.display = DisplayStyle.Flex;
        if (_rows != null)
        {
            foreach (var row in _rows)
                row.style.display = DisplayStyle.None;
        }
    }

    public void SetScores(LeaderboardEntry[] entries)
    {
        if (_loadingLabel != null)
            _loadingLabel.style.display = DisplayStyle.None;
        if (_rows == null) return;

        for (int i = 0; i < EntryCount; i++)
        {
            LeaderboardEntry entry = (entries != null && i < entries.Length)
                ? entries[i]
                : new LeaderboardEntry { Initials = "---", Score = 0 };

            _rows[i].style.display = DisplayStyle.Flex;
            _initialsLabels[i].text = string.IsNullOrEmpty(entry.Initials) ? "---" : entry.Initials;
            _scoreLabels[i].text = entry.Score.ToString();
        }
    }
}
