using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StartScreen : BaseScreen
{
    public event Action OnStartPressed;
    public event Action OnScreenShown;

    [SerializeField] private LeaderboardWidget _leaderboardWidget;

    private VisualElement _leaderboardRegion;
    private VisualElement _promptRegion;
    private bool _listening;

    public VisualElement LeaderboardRegion => _leaderboardRegion ??= Document?.rootVisualElement?.Q("leaderboardRegion");
    public VisualElement PromptRegion => _promptRegion ??= Document?.rootVisualElement?.Q("promptRegion");

    public override void Show()
    {
        base.Show();
        _leaderboardWidget?.SetLoading();
        if (LeaderboardService.Instance != null)
            LeaderboardService.Instance.FetchScores(OnScoresFetched);
    }

    protected override void OnShow()
    {
        _listening = true;
        OnScreenShown?.Invoke();
    }
    protected override void OnHide() => _listening = false;

    private void Update()
    {
        if (_listening && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            OnStartPressed?.Invoke();
    }

    private void OnScoresFetched(LeaderboardEntry[] entries)
    {
        _leaderboardWidget?.SetScores(entries);
    }
}
