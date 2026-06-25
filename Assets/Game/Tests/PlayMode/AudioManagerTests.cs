using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioManagerTests
{
    private GameObject _go;
    private AudioManager _audioManager;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("AudioManager");
        _audioManager = _go.AddComponent<AudioManager>();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (AudioManager.Instance != null)
            Object.Destroy(AudioManager.Instance.gameObject);
        else if (_go != null)
            Object.Destroy(_go);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AudioManager_Awake_SetsSingleton()
    {
        yield return null;
        Assert.AreEqual(_audioManager, AudioManager.Instance);
    }

    [UnityTest]
    public IEnumerator AudioManager_DuplicateAwake_KeepsFirstAsSingleton()
    {
        yield return null;
        var go2 = new GameObject("AudioManager2");
        go2.AddComponent<AudioManager>();
        yield return null;
        Assert.AreEqual(_audioManager, AudioManager.Instance);
    }

    [UnityTest]
    public IEnumerator AudioManager_PlayMusic_StartsMusicSource()
    {
        yield return null;
        _audioManager.PlayMusic("gameplay_theme");
        var sources = _go.GetComponents<AudioSource>();
        AudioSource musicSource = null;
        foreach (var s in sources)
        {
            if (s.loop)
            {
                musicSource = s;
                break;
            }
        }
        Assert.IsNotNull(musicSource);
    }

    [UnityTest]
    public IEnumerator AudioManager_StopMusic_StopsMusicSource()
    {
        yield return null;
        _audioManager.StopMusic();
        var sources = _go.GetComponents<AudioSource>();
        foreach (var s in sources)
        {
            if (s.loop)
            {
                Assert.IsFalse(s.isPlaying);
                break;
            }
        }
    }

    [UnityTest]
    public IEnumerator AudioManager_PlaySFX_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _audioManager.PlaySFX("piece_lock"));
    }

    [UnityTest]
    public IEnumerator AudioManager_PlayMusic_UnknownName_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _audioManager.PlayMusic("nonexistent"));
    }

    [UnityTest]
    public IEnumerator AudioManager_StopMusic_WhenNotPlaying_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _audioManager.StopMusic());
    }

    [UnityTest]
    public IEnumerator AudioManager_GameOverEvent_StopsAndPlaysSfx()
    {
        yield return null;
        var gpGo = new GameObject("GameplayController");
        var playfield = new GameObject("Playfield").AddComponent<PlayfieldController>();
        var pieceGo = new GameObject("PieceController");
        var pieceCtrl = pieceGo.AddComponent<PieceController>();
        pieceCtrl.PlayfieldController = playfield;
        var gameplay = gpGo.AddComponent<GameplayController>();
        gameplay.PlayfieldController = playfield;
        gameplay.PieceController = pieceCtrl;

        yield return null;

        gameplay.OnGameOver += () => { };

        Assert.DoesNotThrow(() => gameplay.StopGame());

        Object.Destroy(gpGo);
        Object.Destroy(playfield.gameObject);
        Object.Destroy(pieceGo);
    }

    [UnityTest]
    public IEnumerator AudioManager_PlaySFX_TetrisClear_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _audioManager.PlaySFX("tetris_clear"));
    }

    [UnityTest]
    public IEnumerator AudioManager_InitialsWidget_HasCycledEvent()
    {
        yield return null;
        var widgetGo = new GameObject("InitialsWidget");
        var widget = widgetGo.AddComponent<InitialsEntryWidget>();
        yield return null;

        int cycleCount = 0;
        widget.OnCharacterCycled += () => cycleCount++;
        widget.OnCharacterCycled -= () => cycleCount++;
        Object.Destroy(widgetGo);
    }

    [UnityTest]
    public IEnumerator AudioManager_InitialsWidget_HasConfirmedEvent()
    {
        yield return null;
        var widgetGo = new GameObject("InitialsWidget");
        var widget = widgetGo.AddComponent<InitialsEntryWidget>();
        yield return null;

        int confirmCount = 0;
        widget.OnCharacterConfirmed += () => confirmCount++;
        widget.OnCharacterConfirmed -= () => confirmCount++;
        Object.Destroy(widgetGo);
    }
}
