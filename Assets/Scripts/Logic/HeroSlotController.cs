using System;
using UnityEngine;
using System.Collections.Generic;

public sealed class HeroSlotController : SlotController
{
    private IHeroBenchProvider _heroBenchProvider;
    private HeroView _heroViewPrefab;

    private SummonTrail _summonTrailPrefab;
    private Func<Vector3> _getSummonTrailOrigin;
    private readonly HashSet<int> _slotsWithTrailInFlight = new HashSet<int>();

    public void Initialize(
        IHeroBenchProvider heroBenchProvider,
        HeroView heroViewPrefab,
        Action<Vector2> onDrag,
        Func<int, Vector3, bool> onDropped,
        SummonTrail summonTrailPrefab = null,
        Func<Vector3> getSummonTrailOrigin = null)
    {
        _heroBenchProvider = heroBenchProvider;
        _heroBenchProvider.OnHeroBenchChanged += Refresh;
        _heroViewPrefab = heroViewPrefab;
        _summonTrailPrefab = summonTrailPrefab;
        _getSummonTrailOrigin = getSummonTrailOrigin;

        InitializeSlots(heroBenchProvider.HeroBench.Length, onDrag, onDropped);

        Refresh();
    }

    private void OnDestroy()
    {
        if (_heroBenchProvider != null) _heroBenchProvider.OnHeroBenchChanged -= Refresh;
    }

    protected override Slot CreateSlot() => new HeroSlot();

    public override bool IsSlotEmpty(int idx) => _heroBenchProvider.HeroBench[idx] == null;

    protected override void Refresh()
    {
        var heroBench = _heroBenchProvider.HeroBench;

        // 영웅 슬롯을 순회하며 HeroInstance 싱크를 맞추고 HeroView 갱신
        for (int idx = 0; idx < slots.Length; idx++)
        {
            var heroSlot = (HeroSlot)slots[idx];
            var heroInstance = idx < heroBench.Length ? heroBench[idx] : null;

            // 해당 슬롯에 영웅 인스턴스가 없는 경우
            if (heroInstance == null)
            {
                // 영웅 뷰가 있다면
                if (heroSlot.HeroView != null)
                {
                    Destroy(heroSlot.HeroView.gameObject);
                    heroSlot.HeroView = null;
                }
            }
            // 해당 슬롯에 영웅 인스턴스가 있는 경우
            else
            {
                // 뷰 없고 트레일도 비행 중이 아닌 경우 → 새 영웅 등장
                if (heroSlot.HeroView == null && !_slotsWithTrailInFlight.Contains(idx))
                {
                    if (_summonTrailPrefab != null && _getSummonTrailOrigin != null)
                    {
                        LaunchSummonTrailToSlot(idx, heroSlot, heroInstance);
                    }
                    else
                    {
                        SpawnHeroView(heroSlot, heroInstance, idx);
                    }
                }
                // 영웅 뷰는 있지만 다른 영웅으로 교체된 경우 → 영웅 교체 (영웅 뷰 재사용)
                else if (heroSlot.HeroView != null && heroSlot.HeroView.HeroInstance != heroInstance)
                {
                    SetupHeroView(heroSlot, heroInstance, idx);
                }
            }
        }
    }

    private void SpawnHeroView(HeroSlot heroSlot, HeroInstance heroInstance, int idx)
    {
        heroSlot.HeroView = Instantiate(_heroViewPrefab, heroSlot.Root);
        SetupHeroView(heroSlot, heroInstance, idx);
    }

    private void SetupHeroView(HeroSlot heroSlot, HeroInstance heroInstance, int idx)
    {
        heroSlot.HeroView.Setup(heroInstance, idx, onDrag, onDropped);
        heroSlot.HeroView.transform.localPosition = Vector3.zero;
    }

    private void LaunchSummonTrailToSlot(int idx, HeroSlot heroSlot, HeroInstance heroInstance)
    {
        _slotsWithTrailInFlight.Add(idx);

        var trail = Instantiate(_summonTrailPrefab);
        var startPos = _getSummonTrailOrigin();
        var endPos = heroSlot.Root.position;
        trail.Launch(startPos, endPos, () =>
        {
            _slotsWithTrailInFlight.Remove(idx);
            SpawnHeroView(heroSlot, heroInstance, idx);
        });
    }
}
