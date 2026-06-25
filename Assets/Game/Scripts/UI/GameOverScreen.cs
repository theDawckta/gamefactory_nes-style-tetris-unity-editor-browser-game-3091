using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    private VisualElement _finalScoreRegion;

    // Public setter preserved for test isolation (InitialsEntryWidgetTests injects a region directly)
    public VisualElement InitialsRegion { get; set; }
    public VisualElement ReturnPromptRegion { get; set; }

    protected override void OnShow()
    {
        var root = Document?.rootVisualElement;
        if (root == null) return;
        _finalScoreRegion = root.Q("finalScoreRegion");
        InitialsRegion = root.Q("initialsRegion");
        ReturnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();

        if (_finalScoreRegion != null)
        {
            _finalScoreRegion.Clear();
            var label = new Label("SCORE: " + score);
            label.AddToClassList("final-score-label");
            _finalScoreRegion.Add(label);
        }

        bool isTopFive = LeaderboardService.Instance != null && LeaderboardService.Instance.IsTopFive(score);
        if (isTopFive)
        {
            SetDisplay(InitialsRegion, DisplayStyle.Flex);
            SetDisplay(ReturnPromptRegion, DisplayStyle.None);
        }
        else
        {
            SetDisplay(InitialsRegion, DisplayStyle.None);
            SetDisplay(ReturnPromptRegion, DisplayStyle.Flex);
        }
    }

    public void ShowReturnPrompt()
    {
        SetDisplay(InitialsRegion, DisplayStyle.None);
        SetDisplay(ReturnPromptRegion, DisplayStyle.Flex);
    }

    private static void SetDisplay(VisualElement element, DisplayStyle display)
    {
        if (element != null)
            element.style.display = display;
    }
}
