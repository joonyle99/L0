using UnityEngine;

public enum HeroGrade
{
    Basic, // 일반
    Rare, // 레어
    Epic, // 영웅
    Legendary, // 전설
    Myth, // 신화
}

[CreateAssetMenu(fileName = "HeroData", menuName = "Lucky/HeroData")]
public class HeroData : ScriptableObject
{
    public string Name;
    public int BaseAttack;
    public int BaseHealth;
    public HeroGrade Grade;
    public AbilityData Ability;
    public GameObject BodyPrefab;
}
