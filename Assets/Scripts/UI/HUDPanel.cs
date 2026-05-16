using TMPro;
using System;
using UnityEngine;

public class HUDPanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI _gold;

    public void Initialize()
    {
        
    }

    public void SetGoldText(int gold)
    {
        _gold.text = gold.ToString();
    }
}
