using UnityEngine;

/// <summary>
/// 등급별 소환 확률 (예: 일반 30%, 레어 70%)
/// </summary>
[System.Serializable]
public struct GradeChanceEntry
{
    public HeroGrade grade;
    [Range(0f, 1f)] public float chance; // 0~1 사이 확률 (0.3 = 30%)
}

/// <summary>
/// 라운드별 소환 확률 테이블 (해당 라운드부터 이 확률 적용)
/// </summary>
[System.Serializable]
public struct RoundEntry
{
    public int round; // 적용 시작 라운드
    public GradeChanceEntry[] gradeChanceEntries; // 등급별 확률 목록 (합계 1.0)
}

[CreateAssetMenu(fileName = "SummonTable", menuName = "Lucky/SummonTable")]
public class SummonTable : ScriptableObject
{
    [SerializeField] private RoundEntry[] _roundEntries;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var roundEntry in _roundEntries)
        {
            var sum = 0f;

            foreach (var gradeChance in roundEntry.gradeChanceEntries)
            {
                sum += gradeChance.chance;
            }

            if (Mathf.Abs(sum - 1f) > 0.01f)
            {
                Debug.LogWarning($"라운드 {roundEntry.round} 확률 합: {sum} (1.0이어야 함)");
            }
        }
    }
#endif

    public GradeChanceEntry[] GetGradeChanceEntries(int round)
    {
        var bestRoundEntry = _roundEntries[_roundEntries.Length - 1];

        foreach (var roundEntry in _roundEntries)
        {
            if (roundEntry.round <= round)
            {
                bestRoundEntry = roundEntry;
            }
        }
        
        return bestRoundEntry.gradeChanceEntries;
    }
}
