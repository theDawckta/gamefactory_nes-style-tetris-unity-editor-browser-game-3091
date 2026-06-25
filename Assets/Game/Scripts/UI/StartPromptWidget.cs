using UnityEngine;
using UnityEngine.UIElements;

public class StartPromptWidget : MonoBehaviour
{
    [SerializeField] private StartScreen _startScreen;

    private Label _promptLabel;
    private float _blinkTimer;
    private bool _labelVisible;
    private bool _isActive;

    public void Activate()
    {
        var region = _startScreen != null ? _startScreen.PromptRegion : null;
        if (region == null) return;

        region.Clear();
        _promptLabel = new Label("PRESS ENTER TO START");
        _promptLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        _promptLabel.style.fontSize = 20;
        _promptLabel.style.color = new StyleColor(Color.white);
        _promptLabel.style.display = DisplayStyle.Flex;
        region.Add(_promptLabel);

        _blinkTimer = 0f;
        _labelVisible = true;
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
        if (_promptLabel != null)
            _promptLabel.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (!_isActive || _promptLabel == null) return;

        _blinkTimer += Time.deltaTime;

        if (_labelVisible && _blinkTimer >= 0.6f)
        {
            _promptLabel.style.display = DisplayStyle.None;
            _labelVisible = false;
            _blinkTimer -= 0.6f;
        }
        else if (!_labelVisible && _blinkTimer >= 0.4f)
        {
            _promptLabel.style.display = DisplayStyle.Flex;
            _labelVisible = true;
            _blinkTimer -= 0.4f;
        }
    }
}
