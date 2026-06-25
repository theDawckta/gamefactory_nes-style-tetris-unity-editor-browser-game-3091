using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    private VisualElement _finalScoreRegion;
    private VisualElement _initialsRegion;
    private VisualElement _returnPromptRegion;

    public VisualElement FinalScoreRegion
    {
        get => _finalScoreRegion;
        set => _finalScoreRegion = value;
    }

    public VisualElement InitialsRegion
    {
        get => _initialsRegion;
        set => _initialsRegion = value;
    }

    public VisualElement ReturnPromptRegion
    {
        get => _returnPromptRegion;
        set => _returnPromptRegion = value;
    }

    protected override void OnShow()
    {
        var root = Document?.rootVisualElement;
        if (root == null) return;
        _finalScoreRegion ??= root.Q("finalScoreRegion");
        _initialsRegion ??= root.Q("initialsRegion");
        _returnPromptRegion ??= root.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();

        if (_finalScoreRegion != null)
        {
            _finalScoreRegion.Clear();
            _finalScoreRegion.Add(new Label(score.ToString()));
        }

        bool isTopFive = LeaderboardService.Instance != null &&
                         LeaderboardService.Instance.IsTopFive(score);

        if (isTopFive)
        {
            if (_initialsRegion != null)
                _initialsRegion.style.display = DisplayStyle.Flex;
            if (_returnPromptRegion != null)
                _returnPromptRegion.style.display = DisplayStyle.None;
        }
        else
        {
            if (_initialsRegion != null)
                _initialsRegion.style.display = DisplayStyle.None;
            if (_returnPromptRegion != null)
                _returnPromptRegion.style.display = DisplayStyle.Flex;
        }
    }

    public void ShowReturnPrompt()
    {
        if (_initialsRegion != null)
            _initialsRegion.style.display = DisplayStyle.None;
        if (_returnPromptRegion != null)
            _returnPromptRegion.style.display = DisplayStyle.Flex;
    }
}
