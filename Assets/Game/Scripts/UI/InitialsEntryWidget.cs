using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InitialsEntryWidget : MonoBehaviour
{
    public event Action OnCharacterCycled;
    public event Action OnCharacterConfirmed;

    [SerializeField] private GameOverScreen _gameOverScreen;

    private static readonly char[] ValidChars = BuildValidChars();
    private const int SlotCount = 3;

    private Label[] _slotLabels;
    private VisualElement[] _cursorElements;
    private Label _confirmLabel;

    private int[] _slotIndices;
    private int _currentSlot;
    private int _confirmedSlots;
    private bool _isActive;
    private bool _awaitingConfirm;
    private int _score;

    private static char[] BuildValidChars()
    {
        var chars = new char[36];
        for (int i = 0; i < 26; i++) chars[i] = (char)('A' + i);
        for (int i = 0; i < 10; i++) chars[26 + i] = (char)('0' + i);
        return chars;
    }

    private void Update()
    {
        if (!_isActive) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.leftArrowKey.wasPressedThisFrame)
            CycleCurrentSlot(-1);
        else if (kb.rightArrowKey.wasPressedThisFrame)
            CycleCurrentSlot(1);
        else if (kb.downArrowKey.wasPressedThisFrame)
            ConfirmCurrent();
        else if (kb.upArrowKey.wasPressedThisFrame)
            MoveToPrevious();
    }

    public void Activate(int score)
    {
        _score = score;
        _slotIndices = new int[SlotCount];
        _currentSlot = 0;
        _confirmedSlots = 0;
        _awaitingConfirm = false;
        _isActive = true;
        BuildUI();
        RefreshUI();
    }

    public void Deactivate()
    {
        _isActive = false;
    }

    private void BuildUI()
    {
        var region = _gameOverScreen != null ? _gameOverScreen.InitialsRegion : null;
        if (region == null) return;
        region.Clear();

        var slotsContainer = new VisualElement();
        slotsContainer.style.flexDirection = FlexDirection.Row;
        slotsContainer.style.justifyContent = Justify.Center;
        slotsContainer.style.alignItems = Align.Center;
        region.Add(slotsContainer);

        _slotLabels = new Label[SlotCount];
        _cursorElements = new VisualElement[SlotCount];

        for (int i = 0; i < SlotCount; i++)
        {
            var slotContainer = new VisualElement();
            slotContainer.style.flexDirection = FlexDirection.Column;
            slotContainer.style.alignItems = Align.Center;
            slotContainer.style.marginLeft = 10;
            slotContainer.style.marginRight = 10;

            var charLabel = new Label(ValidChars[0].ToString());
            charLabel.style.fontSize = 36;
            charLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            charLabel.style.color = new StyleColor(Color.white);
            _slotLabels[i] = charLabel;
            slotContainer.Add(charLabel);

            var cursor = new Label("_");
            cursor.style.fontSize = 24;
            cursor.style.color = new StyleColor(Color.yellow);
            cursor.style.unityTextAlign = TextAnchor.UpperCenter;
            _cursorElements[i] = cursor;
            slotContainer.Add(cursor);

            slotsContainer.Add(slotContainer);
        }

        _confirmLabel = new Label("CONFIRM");
        _confirmLabel.style.display = DisplayStyle.None;
        _confirmLabel.style.fontSize = 24;
        _confirmLabel.style.color = new StyleColor(Color.yellow);
        _confirmLabel.style.unityTextAlign = TextAnchor.UpperCenter;
        _confirmLabel.style.marginTop = 12;
        region.Add(_confirmLabel);
    }

    private void RefreshUI()
    {
        if (_slotLabels == null) return;

        for (int i = 0; i < SlotCount; i++)
        {
            _slotLabels[i].text = ValidChars[_slotIndices[i]].ToString();
            _cursorElements[i].style.display = (!_awaitingConfirm && i == _currentSlot && i >= _confirmedSlots)
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        _confirmLabel.style.display = _awaitingConfirm ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void CycleCurrentSlot(int direction)
    {
        if (_awaitingConfirm) return;
        _slotIndices[_currentSlot] = (_slotIndices[_currentSlot] + direction + ValidChars.Length) % ValidChars.Length;
        OnCharacterCycled?.Invoke();
        RefreshUI();
    }

    private void ConfirmCurrent()
    {
        OnCharacterConfirmed?.Invoke();
        if (_awaitingConfirm)
        {
            SubmitScore();
            return;
        }

        if (_currentSlot < SlotCount - 1)
        {
            _confirmedSlots++;
            _currentSlot++;
        }
        else
        {
            _confirmedSlots = SlotCount;
            _awaitingConfirm = true;
        }

        RefreshUI();
    }

    private void MoveToPrevious()
    {
        if (_awaitingConfirm)
        {
            _awaitingConfirm = false;
            _confirmedSlots = SlotCount - 1;
            _currentSlot = SlotCount - 1;
            RefreshUI();
            return;
        }

        if (_currentSlot > 0)
        {
            _currentSlot--;
            if (_confirmedSlots > _currentSlot)
                _confirmedSlots = _currentSlot;
            RefreshUI();
        }
    }

    private void SubmitScore()
    {
        string initials = string.Concat(
            ValidChars[_slotIndices[0]],
            ValidChars[_slotIndices[1]],
            ValidChars[_slotIndices[2]]);
        Deactivate();

        if (LeaderboardService.Instance != null)
        {
            LeaderboardService.Instance.SubmitScore(initials, _score, _ =>
            {
                if (_gameOverScreen != null)
                    _gameOverScreen.ShowReturnPrompt();
            });
        }
        else
        {
            if (_gameOverScreen != null)
                _gameOverScreen.ShowReturnPrompt();
        }
    }
}
