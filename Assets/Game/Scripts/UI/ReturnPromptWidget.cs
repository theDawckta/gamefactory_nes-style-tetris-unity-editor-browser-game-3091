using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ReturnPromptWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    public event Action OnReturnPressed;

    private const float VisibleDuration = 0.6f;
    private const float HiddenDuration = 0.4f;

    private VisualElement _region;
    private Label _promptLabel;
    private bool _isActive;
    private Coroutine _blinkCoroutine;

    public void Activate()
    {
        _region = _gameOverScreen != null ? _gameOverScreen.ReturnPromptRegion : null;
        if (_region == null) return;

        _region.Clear();

        _promptLabel = new Label("PRESS ENTER TO CONTINUE");
        _promptLabel.style.fontSize = 24;
        _promptLabel.style.color = new StyleColor(Color.white);
        _promptLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        _region.Add(_promptLabel);

        _isActive = true;

        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(Blink());
    }

    public void Deactivate()
    {
        _isActive = false;
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    private void Update()
    {
        if (!_isActive) return;
        if (_region == null || _region.style.display == DisplayStyle.None) return;

        var kb = Keyboard.current;
        if (kb != null && kb.enterKey.wasPressedThisFrame)
            OnReturnPressed?.Invoke();
    }

    private IEnumerator Blink()
    {
        while (_isActive)
        {
            if (_promptLabel != null)
                _promptLabel.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(VisibleDuration);
            if (_promptLabel != null)
                _promptLabel.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(HiddenDuration);
        }
    }
}
