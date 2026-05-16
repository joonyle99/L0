using UnityEngine;

public class HeroInstance
{
    public HeroInstance(HeroData data, int level = 1)
    {
        Data = data;
        CurrAttack = data.BaseAttack;
        CurrHealth = data.BaseHealth;
        CurrLevel = level;
    }

    public HeroData Data { get; private set; }
    public int CurrAttack { get; set; }
    public int CurrHealth { get; set; }
    public int CurrLevel { get; private set; }
    
    public void LevelUp()
    {
        CurrLevel++;
    }
    
    public void TakeDamage(int damage)
    {
        CurrHealth -= damage;
    }
}
