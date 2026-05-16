using TMPro;
using System;
using UnityEngine;

public class HUDPanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI _gold;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private TextMeshProUGUI _round;

    public void Initialize()
    {
        
    }

    public void SetGoldText(int gold)
    {
        _gold.text = gold.ToString();
    }

    public void SetTimerText(float seconds)
    {
        _timer.text = TimeFormatter.FormatMMSS(seconds);
    }
}
