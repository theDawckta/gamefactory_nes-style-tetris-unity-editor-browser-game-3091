using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PieceController : MonoBehaviour
{
    private static readonly float[] GravityIntervals =
    {
        0.800f, 0.717f, 0.633f, 0.550f, 0.467f, 0.383f, 0.300f, 0.217f, 0.133f, 0.100f
    };

    private const float DasDelay = 0.267f;
    private const float DasRepeat = 0.100f;
    private const float LockDelayDuration = 0.500f;

    public PlayfieldController PlayfieldController;

    public event Action OnPieceLocked;

    public bool IsLocked { get; private set; }
    public Vector2Int CurrentPivot { get; private set; }
    public int CurrentRotation { get; private set; }
    public int Level { get; set; }
    public TetrominoData ActivePiece => _activePiece;

    private TetrominoData _activePiece;
    private float _gravityTimer;
    private float _lockTimer;
    private bool _isGrounded;
    private float _dasLeftTimer;
    private float _dasRightTimer;
    private bool _dasLeftActive;
    private bool _dasRightActive;

    public void SpawnPiece(TetrominoData data)
    {
        _activePiece = data;
        CurrentPivot = new Vector2Int(4, 20);
        CurrentRotation = 0;
        IsLocked = false;
        _gravityTimer = 0f;
        _lockTimer = 0f;
        _isGrounded = false;
        _dasLeftTimer = 0f;
        _dasRightTimer = 0f;
        _dasLeftActive = false;
        _dasRightActive = false;
    }

    public void Tick(float deltaTime)
    {
        if (_activePiece == null || IsLocked)
            return;

        HandleRotation();
        HandleLateralMovement(deltaTime);
        HandleGravity(deltaTime);
        HandleLockDelay(deltaTime);
    }

    private void HandleRotation()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.upArrowKey.wasPressedThisFrame)
            TryRotate();
    }

    private void TryRotate()
    {
        int newRotation = (CurrentRotation + 1) % 4;
        if (PlayfieldController.IsValidPosition(_activePiece, newRotation, CurrentPivot))
        {
            CurrentRotation = newRotation;
            if (_isGrounded)
                _lockTimer = 0f;
        }
    }

    private void HandleLateralMovement(float deltaTime)
    {
        var keyboard = Keyboard.current;
        bool leftDown = keyboard != null && keyboard.leftArrowKey.wasPressedThisFrame;
        bool leftHeld = keyboard != null && keyboard.leftArrowKey.isPressed;
        bool rightDown = keyboard != null && keyboard.rightArrowKey.wasPressedThisFrame;
        bool rightHeld = keyboard != null && keyboard.rightArrowKey.isPressed;

        if (leftDown)
        {
            TryMoveLateral(-1);
            _dasLeftTimer = 0f;
            _dasLeftActive = false;
        }
        else if (leftHeld)
        {
            _dasLeftTimer += deltaTime;
            if (!_dasLeftActive)
            {
                if (_dasLeftTimer >= DasDelay)
                {
                    TryMoveLateral(-1);
                    _dasLeftActive = true;
                    _dasLeftTimer = 0f;
                }
            }
            else if (_dasLeftTimer >= DasRepeat)
            {
                TryMoveLateral(-1);
                _dasLeftTimer = 0f;
            }
        }
        else
        {
            _dasLeftTimer = 0f;
            _dasLeftActive = false;
        }

        if (rightDown)
        {
            TryMoveLateral(1);
            _dasRightTimer = 0f;
            _dasRightActive = false;
        }
        else if (rightHeld)
        {
            _dasRightTimer += deltaTime;
            if (!_dasRightActive)
            {
                if (_dasRightTimer >= DasDelay)
                {
                    TryMoveLateral(1);
                    _dasRightActive = true;
                    _dasRightTimer = 0f;
                }
            }
            else if (_dasRightTimer >= DasRepeat)
            {
                TryMoveLateral(1);
                _dasRightTimer = 0f;
            }
        }
        else
        {
            _dasRightTimer = 0f;
            _dasRightActive = false;
        }
    }

    private bool TryMoveLateral(int dx)
    {
        Vector2Int newPivot = new Vector2Int(CurrentPivot.x + dx, CurrentPivot.y);
        if (PlayfieldController.IsValidPosition(_activePiece, CurrentRotation, newPivot))
        {
            CurrentPivot = newPivot;
            if (_isGrounded)
                _lockTimer = 0f;
            return true;
        }
        return false;
    }

    private void HandleGravity(float deltaTime)
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.downArrowKey.isPressed)
        {
            TryMoveDown();
            return;
        }

        _gravityTimer += deltaTime;
        float interval = GravityIntervals[Mathf.Clamp(Level, 0, GravityIntervals.Length - 1)];
        while (_gravityTimer >= interval)
        {
            _gravityTimer -= interval;
            TryMoveDown();
            if (_isGrounded)
                break;
        }
    }

    private void TryMoveDown()
    {
        Vector2Int newPivot = new Vector2Int(CurrentPivot.x, CurrentPivot.y - 1);
        if (PlayfieldController.IsValidPosition(_activePiece, CurrentRotation, newPivot))
        {
            CurrentPivot = newPivot;
            _isGrounded = false;
            _lockTimer = 0f;
        }
        else
        {
            if (!_isGrounded)
            {
                _isGrounded = true;
                _lockTimer = 0f;
            }
        }
    }

    private void HandleLockDelay(float deltaTime)
    {
        if (!_isGrounded)
            return;

        _lockTimer += deltaTime;
        if (_lockTimer >= LockDelayDuration)
            LockCurrentPiece();
    }

    private void LockCurrentPiece()
    {
        PlayfieldController.LockPiece(_activePiece, CurrentRotation, CurrentPivot);
        IsLocked = true;
        _activePiece = null;
        OnPieceLocked?.Invoke();
    }
}
