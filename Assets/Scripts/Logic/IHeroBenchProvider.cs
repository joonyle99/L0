using System;

public interface IHeroBenchProvider
{
    HeroInstance[] HeroBench { get; }
    
    event Action OnHeroBenchChanged;

    bool IsHeroBenchFull();
    bool AddHeroToBench(HeroInstance heroInstance, int idx);
    HeroInstance TakeHeroFromBench(int idx);
}
