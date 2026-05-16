using System;
using UnityEngine;

public class RoundManager
{
    private int _currRound;
    private RoundTable _roundTable;

    public int CurrentRound => _currRound;

    private event Action<int> _onRoundChanged;

    public RoundManager(RoundTable roundTable)
    {
        _roundTable = roundTable;
        _currRound = 0;
    }
    
    public void Initialize(Action<int> onRoundChanged)
    {
        _onRoundChanged += onRoundChanged;
        _onRoundChanged?.Invoke(_currRound);
    }

    public void NextRound()
    {
        _currRound++;
        _onRoundChanged?.Invoke(_currRound);
    }

    public RoundData GetCurrentRoundData()
    {
        return _roundTable.GetRoundData(_currRound);
    }

    public void Reset()
    {
        _currRound = 0;
    }

}
