using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    private Dictionary<string, AudioClip> _musicClips;
    private Dictionary<string, AudioClip> _sfxClips;

    private int _prevLevel;
    private int _prevLines;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        LoadClips();
        WireEvents();
    }

    private void LoadClips()
    {
        _musicClips = new Dictionary<string, AudioClip>();
        _sfxClips = new Dictionary<string, AudioClip>();

        string[] musicNames = { "gameplay_theme", "start_screen_theme" };
        foreach (string name in musicNames)
        {
            AudioClip clip = Resources.Load<AudioClip>("Music/" + name);
            if (clip != null)
                _musicClips[name] = clip;
        }

        string[] sfxNames = { "piece_lock", "line_clear", "tetris_clear", "level_up", "game_over", "menu_move", "menu_confirm", "start_game" };
        foreach (string name in sfxNames)
        {
            AudioClip clip = Resources.Load<AudioClip>("SFX/" + name);
            if (clip != null)
                _sfxClips[name] = clip;
        }
    }

    private void WireEvents()
    {
        SceneController sceneController = UnityEngine.Object.FindFirstObjectByType<SceneController>();
        if (sceneController != null)
            sceneController.OnGameStarted += HandleGameStarted;

        StartScreen startScreen = UnityEngine.Object.FindFirstObjectByType<StartScreen>();
        if (startScreen != null)
            startScreen.OnScreenShown += HandleStartScreenShown;

        GameplayController gameplay = UnityEngine.Object.FindFirstObjectByType<GameplayController>();
        if (gameplay != null)
        {
            gameplay.OnGameOver += HandleGameOver;
            gameplay.Scoring.OnStatsChanged += HandleStatsChanged;
        }

        PieceController pieceController = UnityEngine.Object.FindFirstObjectByType<PieceController>();
        if (pieceController != null)
            pieceController.OnPieceLocked += HandlePieceLocked;

        InitialsEntryWidget initialsWidget = UnityEngine.Object.FindFirstObjectByType<InitialsEntryWidget>();
        if (initialsWidget != null)
        {
            initialsWidget.OnCharacterCycled += HandleCharacterCycled;
            initialsWidget.OnCharacterConfirmed += HandleCharacterConfirmed;
        }
    }

    private void HandleGameStarted()
    {
        _prevLevel = 0;
        _prevLines = 0;
        PlaySFX("start_game");
        PlayMusic("gameplay_theme");
    }

    private void HandleStartScreenShown()
    {
        PlayMusic("start_screen_theme");
    }

    private void HandleGameOver()
    {
        StopMusic();
        PlaySFX("game_over");
    }

    private void HandlePieceLocked()
    {
        PlaySFX("piece_lock");
    }

    private void HandleStatsChanged(int score, int level, int totalLines)
    {
        int lineDelta = totalLines - _prevLines;
        if (lineDelta == 4)
            PlaySFX("tetris_clear");
        else if (lineDelta > 0)
            PlaySFX("line_clear");

        if (level > _prevLevel)
            PlaySFX("level_up");

        _prevLevel = level;
        _prevLines = totalLines;
    }

    private void HandleCharacterCycled()
    {
        PlaySFX("menu_move");
    }

    private void HandleCharacterConfirmed()
    {
        PlaySFX("menu_confirm");
    }

    public void PlayMusic(string name)
    {
        if (_musicSource == null || _musicClips == null) return;
        _musicSource.Stop();
        if (_musicClips.TryGetValue(name, out AudioClip clip))
        {
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (_musicSource == null) return;
        _musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (_sfxSource == null || _sfxClips == null) return;
        if (_sfxClips.TryGetValue(name, out AudioClip clip))
            _sfxSource.PlayOneShot(clip);
    }
}
