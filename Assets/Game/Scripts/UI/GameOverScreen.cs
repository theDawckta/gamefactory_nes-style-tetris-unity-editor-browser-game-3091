using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    public VisualElement FinalScoreRegion { get; set; }
    public VisualElement InitialsRegion { get; set; }
    public VisualElement ReturnPromptRegion { get; set; }

    protected override void OnShow()
    {
        FinalScoreRegion = Document?.rootVisualElement?.Q("finalScoreRegion");
        InitialsRegion = Document?.rootVisualElement?.Q("initialsRegion");
        ReturnPromptRegion = Document?.rootVisualElement?.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();

        if (FinalScoreRegion != null)
        {
            FinalScoreRegion.Clear();
            var scoreLabel = new Label(score.ToString());
            scoreLabel.AddToClassList("score-value-label");
            FinalScoreRegion.Add(scoreLabel);
        }

        bool isTopFive = LeaderboardService.Instance != null && LeaderboardService.Instance.IsTopFive(score);
        if (isTopFive)
        {
            if (InitialsRegion != null)
                InitialsRegion.style.display = DisplayStyle.Flex;
            if (ReturnPromptRegion != null)
                ReturnPromptRegion.style.display = DisplayStyle.None;
        }
        else
        {
            if (InitialsRegion != null)
                InitialsRegion.style.display = DisplayStyle.None;
            if (ReturnPromptRegion != null)
                ReturnPromptRegion.style.display = DisplayStyle.Flex;
        }
    }

    public void ShowReturnPrompt()
    {
        if (InitialsRegion != null)
            InitialsRegion.style.display = DisplayStyle.None;
        if (ReturnPromptRegion != null)
            ReturnPromptRegion.style.display = DisplayStyle.Flex;
    }
}
