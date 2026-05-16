using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IGameStateListener<InGameState>
{
    [SerializeField] private HUDPanel _hudPanel;
    [SerializeField] private ActionPanel _actionPanel;
    
    private Camera _camera;

    private void OnDestroy()
    {
        
    }

    public void Initialize(Action onSummonClicked, Action onBattleClicked)
    {
        _camera = Camera.main;

        _hudPanel.Initialize();
        _actionPanel.Initialize(onSummonClicked, onBattleClicked);
    }

    public void OnStateChanged(InGameState prevState, InGameState currState)
    {
        
    }

    public void SetGoldText(int gold) => _hudPanel.SetGoldText(gold);

    public Vector3 GetSummonButtonWorldPos()
    {
        var screenPos = _actionPanel.SummonButtonScreenPos;
        var worldPos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        worldPos.z = 0f;

#if UNITY_EDITOR
        // Debug.Log(screenPos);
        // Debug.Log(worldPos);
        Debug.DrawRay(worldPos, Vector3.up * 0.5f, Color.red, 1f);
        Debug.DrawRay(worldPos, Vector3.right * 0.5f, Color.green, 1f);
#endif

        return worldPos;
    }
}
