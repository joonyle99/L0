using System;
using UnityEngine;

public class SummonManager : IHeroBenchProvider
{
    private HeroDatabase _heroDatabase;
    private SummonConfig _summonConfig;
    private SummonTable _summonTable;

    private HeroInstance[] _heroBench;
    public HeroInstance[] HeroBench => _heroBench;
    public event Action OnHeroBenchChanged;

    private Func<int, bool> _trySpendGold;

    private int _currCost;
    public int CurrCost => _currCost;

    public SummonManager(HeroDatabase heroDatabase, SummonConfig summonConfig, SummonTable summonTable)
    {
        _heroDatabase = heroDatabase;
        _summonConfig = summonConfig;
        _summonTable = summonTable;

        _heroBench = new HeroInstance[_summonConfig.capacity];
        _currCost = _summonConfig.baseCost;
    }

    public void Initialize(Func<int, bool> trySpendGold)
    {
        _trySpendGold = trySpendGold;
    }

    // === 소환 ===

    public bool TrySummon(int round)
    {
        if (IsHeroBenchFull()) return false;
        if (!_trySpendGold(_currCost)) return false;

        var grade = PickGrade(round);
        var heroData = PickHero(grade);
        if (heroData == null) return false;

        var heroInstance = new HeroInstance(heroData);
        AddHeroToBench(heroInstance, -1);
        _currCost += _summonConfig.costIncrement;

        return true;
    }

    public void ResetCost()
    {
        _currCost = _summonConfig.baseCost;
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
        for (int i = 0; i < _heroBench.Length; i++)
        {
            if (_heroBench[i] == null)
            {
                _heroBench[i] = heroInstance;
                OnHeroBenchChanged?.Invoke();

                return true;
            }
        }

        return false;
    }

    public HeroInstance TakeHeroFromBench(int idx)
    {
        if (idx < 0 || idx >= _summonConfig.capacity) return null;
        
        var heroInstance = _heroBench[idx];
        if (heroInstance == null) return null;

        _heroBench[idx] = null;
        OnHeroBenchChanged?.Invoke();
        
        return heroInstance;
    }

    // === 소환 ===

    private HeroGrade PickGrade(int round)
    {
        var gradeChanceEntries = _summonTable.GetGradeChanceEntries(round);
        var randomChance = UnityEngine.Random.Range(0f, 1f);

        var cumulativeChance = 0f;

        foreach (var gradeChanceEntry in gradeChanceEntries)
        {
            cumulativeChance += gradeChanceEntry.chance;

            if (randomChance <= cumulativeChance)
            {
                return gradeChanceEntry.grade;
            }
        }

        return gradeChanceEntries[gradeChanceEntries.Length - 1].grade;
    }

    private HeroData PickHero(HeroGrade grade)
    {
        var pool = _heroDatabase.GetHeroesByGrade(grade);
        if (pool == null || pool.Count == 0) return null;

        int idx = UnityEngine.Random.Range(0, pool.Count);
        return pool[idx];
    }
}
