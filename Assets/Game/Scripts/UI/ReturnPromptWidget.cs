using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ReturnPromptWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    public event Action OnReturnPressed;

    private Label _promptLabel;
    private Coroutine _blinkCoroutine;
    private bool _isActive;

    private void Update()
    {
        if (!_isActive) return;

        var region = _gameOverScreen != null ? _gameOverScreen.ReturnPromptRegion : null;
        if (region == null || region.style.display == DisplayStyle.None) return;

        var kb = Keyboard.current;
        if (kb != null && kb.enterKey.wasPressedThisFrame)
            OnReturnPressed?.Invoke();
    }

    public void Activate()
    {
        var region = _gameOverScreen != null ? _gameOverScreen.ReturnPromptRegion : null;
        if (region == null) return;

        region.Clear();
        _promptLabel = new Label("PRESS ENTER TO CONTINUE");
        _promptLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        _promptLabel.style.fontSize = 20;
        _promptLabel.style.color = new StyleColor(Color.white);
        region.Add(_promptLabel);

        _isActive = true;

        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    public void Deactivate()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        _isActive = false;
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            if (_promptLabel != null)
                _promptLabel.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.6f);

            if (_promptLabel != null)
                _promptLabel.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
