using UnityEngine;
using DG.Tweening;
using JoonyleGameDevKit;
using System.Collections.Generic;

public enum SfxType
{
    
}

[System.Serializable]
public struct SfxEntry
{
    public SfxType type;
    public AudioClip clip;
}

public class SoundManager : Singleton<SoundManager>, IManager, IGameStateListener<OutGameState>, IGameStateListener<InGameState>
{
    public int Priority => 10;

    [SerializeField] private SfxEntry[] _sfxEntries;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private float _bgmFadeDuration = 0.35f;

    private Dictionary<SfxType, SfxEntry> _sfxMap;

    private float _bgmVolume;
    private Tween _bgmFadeTween;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        _bgmFadeTween?.Kill();
    }

    public void Initialize()
    {
        _sfxMap = new Dictionary<SfxType, SfxEntry>();

        foreach (var entry in _sfxEntries)
        {
            _sfxMap.TryAdd(entry.type, entry);
        }

        _bgmVolume = _bgmSource.volume;
    }

    public void OnStateChanged(OutGameState prevState, OutGameState currState)
    {
        
    }

    public void OnStateChanged(InGameState prevState, InGameState currState)
    {
        
    }

    public void PlaySfx(SfxType type, float volume = 0.5f)
    {
        if (_sfxMap.TryGetValue(type, out var entry) && entry.clip != null)
        {
            _sfxSource.PlayOneShot(entry.clip, volume);
        }
    }

    private void FadeBgmTo(float targetVolume)
    {
        _bgmFadeTween?.Kill();
        _bgmFadeTween = _bgmSource.DOFade(targetVolume, _bgmFadeDuration).SetUpdate(true);
    }

    private void SetSfxPaused(bool paused)
    {
        var sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            if (source == _bgmSource) continue;
            if (paused) source.Pause();
            else source.UnPause();
        }
    }
}
