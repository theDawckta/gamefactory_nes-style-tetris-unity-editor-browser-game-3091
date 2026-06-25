using UnityEngine;
using UnityEngine.UIElements;

// Stub for issue #28 -- exposes regions and ShowReturnPrompt for child widgets
public class GameOverScreen : MonoBehaviour
{
    public VisualElement InitialsRegion { get; set; }
    public VisualElement ReturnPromptRegion { get; set; }

    public void ShowReturnPrompt()
    {
        // Full implementation in issue #28
    }
}
