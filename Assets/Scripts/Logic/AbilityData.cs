using UnityEngine;

public enum AbilityTrigger
{
    OnBattleStart,    // 전투 시작 시
    OnAttack,         // 공격 시
    OnHit,            // 피격 시
    OnAllyDeath,      // 아군 사망 시
    OnSelfDeath,      // 자신 사망 시
    OnSold,           // 판매 시
    OnSummon,         // 소환 시
    OnRoundStart,     // 라운드 시작 시
    OnRoundEnd,       // 라운드 종료 시
}

public enum AbilityEffect
{
    BuffAttack,       // 공격력 증가
    BuffHealth,       // 체력 증가
    DealDamage,       // 데미지
    SummonUnit,       // 유닛 소환
    Heal,             // 회복
}

public enum AbilityTarget
{
    Self,             // 자신
    RandomAlly,       // 랜덤 아군
    AllAllies,        // 전체 아군
    FrontAlly,        // 맨 앞 아군
    RandomEnemy,      // 랜덤 적
    FrontEnemy,       // 맨 앞 적
    AllEnemies,       // 전체 적
}

[System.Serializable]
public struct AbilityData
{
    public AbilityTrigger Trigger; // 트리거 조건
    public AbilityEffect Effect; // 능력 효과
    public AbilityTarget Target; // 효과 대상
    public int Value; // 효과 수치
}
