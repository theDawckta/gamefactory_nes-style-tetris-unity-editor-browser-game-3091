using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StartScreen : BaseScreen
{
    public event Action OnStartPressed;

    private VisualElement _leaderboardRegion;
    private VisualElement _promptRegion;
    private bool _listening;

    public VisualElement LeaderboardRegion => _leaderboardRegion ??= Document?.rootVisualElement?.Q("leaderboardRegion");
    public VisualElement PromptRegion => _promptRegion ??= Document?.rootVisualElement?.Q("promptRegion");

    public override void Show()
    {
        base.Show();
        if (LeaderboardRegion != null)
        {
            var loadingLabel = LeaderboardRegion.Q<Label>("loadingLabel");
            if (loadingLabel != null)
                loadingLabel.style.display = DisplayStyle.Flex;
        }
        if (LeaderboardService.Instance != null)
            LeaderboardService.Instance.FetchScores(OnScoresFetched);
    }

    protected override void OnShow() => _listening = true;
    protected override void OnHide() => _listening = false;

    private void Update()
    {
        if (_listening && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            OnStartPressed?.Invoke();
    }

    private void OnScoresFetched(LeaderboardEntry[] entries)
    {
        if (LeaderboardRegion == null) return;
        var loadingLabel = LeaderboardRegion.Q<Label>("loadingLabel");
        if (loadingLabel != null)
            loadingLabel.style.display = DisplayStyle.None;
    }
}
