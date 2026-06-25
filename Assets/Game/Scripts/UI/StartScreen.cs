using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StartScreen : BaseScreen
{
    public event Action OnStartPressed;

    private bool _listening;
    private Label _loadingLabel;

    public override void Show()
    {
        base.Show();
        CacheElements();
        SetLoadingText("Loading...");
        if (LeaderboardService.Instance != null)
            LeaderboardService.Instance.FetchScores(OnScoresFetched);
    }

    protected override void OnShow()
    {
        _listening = true;
    }

    protected override void OnHide()
    {
        _listening = false;
    }

    private void CacheElements()
    {
        if (Document == null || Document.rootVisualElement == null) return;
        _loadingLabel = Document.rootVisualElement.Q<Label>("loadingLabel");
    }

    private void SetLoadingText(string text)
    {
        if (_loadingLabel != null)
            _loadingLabel.text = text;
    }

    private void OnScoresFetched(LeaderboardEntry[] entries)
    {
        if (_loadingLabel == null) return;
        if (entries == null || entries.Length == 0)
        {
            _loadingLabel.text = "No scores yet.";
            return;
        }
        var sb = new StringBuilder();
        foreach (var entry in entries)
            sb.AppendLine(entry.Initials + "  " + entry.Score);
        _loadingLabel.text = sb.ToString().TrimEnd();
    }

    private void Update()
    {
        if (!_listening) return;
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.enterKey.wasPressedThisFrame)
            OnStartPressed?.Invoke();
    }
}
