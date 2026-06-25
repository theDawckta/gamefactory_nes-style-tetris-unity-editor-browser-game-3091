using System;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public event Action OnGameStarted;

    public void NotifyGameStarted()
    {
        OnGameStarted?.Invoke();
    }
}
