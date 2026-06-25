using UnityEngine;
using UnityEngine.UIElements;

// Stub for issues #28/#5 -- exposes regions and methods for UI widgets
public class GameOverScreen : MonoBehaviour
{
    public VisualElement InitialsRegion { get; set; }
    public VisualElement FinalScoreRegion { get; set; }
    public VisualElement ReturnPromptRegion { get; set; }

    [SerializeField] private FinalScoreWidget _finalScoreWidget;

    public void ShowWithScore(int score)
    {
        _finalScoreWidget?.SetScore(score);
    }

    public void ShowReturnPrompt()
    {
        // Full implementation in issue #28
    }
}
