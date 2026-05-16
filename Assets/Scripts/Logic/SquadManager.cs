using System;
using UnityEngine;
using System.Collections.Generic;

public class SquadManager : IHeroBenchProvider
{
    private readonly SquadConfig _squadConfig;

    private HeroInstance[] _heroBench;
    public HeroInstance[] HeroBench => _heroBench;
    public event Action OnHeroBenchChanged;

    public SquadManager(SquadConfig squadConfig)
    {
        _squadConfig = squadConfig;
        _heroBench = new HeroInstance[_squadConfig.capacity];
    }

    public void Initialize()
    {

    }

    // === ... 관리 ===

    public bool IsHeroBenchFull()
    {
        for (int i = 0; i < _heroBench.Length; i++)
        {
            if (_heroBench[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    public bool AddHeroToBench(HeroInstance heroInstance, int idx)
    {
        if (idx < 0 || idx >= _heroBench.Length) return false;
        if (_heroBench[idx] != null) return false;

        _heroBench[idx] = heroInstance;
        OnHeroBenchChanged?.Invoke();

        return true;
    }

    public HeroInstance TakeHeroFromBench(int idx)
    {
        if (idx < 0 || idx >= _heroBench.Length) return null;

        var heroInstance = _heroBench[idx];
        if (heroInstance == null) return null;

        _heroBench[idx] = null;
        OnHeroBenchChanged?.Invoke();

        return heroInstance;
    }

    public bool MoveHero(int srcIdx, int dstIdx)
    {
        // Debug.Log($"srcIdx: {srcIdx} / dstIdx: {dstIdx}");
        if (srcIdx < 0 || srcIdx >= _heroBench.Length) return false;
        if (dstIdx < 0 || dstIdx >= _heroBench.Length) return false;
        if (srcIdx == dstIdx) return false;
        if (_heroBench[srcIdx] == null) return false;

        (_heroBench[dstIdx], _heroBench[srcIdx]) = (_heroBench[srcIdx], _heroBench[dstIdx]);
        OnHeroBenchChanged?.Invoke();

        return true;
    }
}
